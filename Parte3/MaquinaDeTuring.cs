using System.Collections.Frozen;

namespace MaquinaDeTuring;


// Resultado de uma computação:
//   Aceita        — a MT parou em um estado de F.
//   Rejeita       — a MT parou (δ indefinida) fora de F.
//   LimiteExcedido — atingiu o teto de passos (provável laço infinito).
public enum ResultadoMT { Aceita, Rejeita, LimiteExcedido }


// Máquina de Turing determinística — 7-tupla formal (Q, Σ, Γ, δ, q0, B, F).

public sealed class MaquinaDeTuring
{
    // ── Constantes de movimento da cabeça ──────────────────────────────────
    public const char Esquerda  = 'E';
    public const char Direita   = 'D';
    public const char Permanece = 'P';

    // ── 7-tupla formal ─────────────────────────────────────────────────────

    /// <summary>Q — conjunto finito de estados.</summary>
    public FrozenSet<string> Q { get; }

    /// <summary>Σ — alfabeto de entrada.</summary>
    public FrozenSet<char> Sigma { get; }

    /// <summary>Γ — alfabeto da fita (contém B).</summary>
    public FrozenSet<char> Gamma { get; }

    public FrozenDictionary<(string Estado, char Lido), (string NovoEstado, char Escrito, char Movimento)> Delta { get; }

    /// <summary>q0 — estado inicial.</summary>
    public string Q0 { get; }

    /// <summary>B — símbolo branco.</summary>
    public char Branco { get; }

    /// <summary>F — conjunto de estados de aceitação.</summary>
    public FrozenSet<string> F { get; }

    /// <summary>qreject — estado explícito de rejeição (requisito d).</summary>
    public string QReject { get; }

    /// <summary>Rótulo descritivo para exibição.</summary>
    public string Nome { get; }

    // ── Construtor 
    // A função δ é recebida exatamente como
    //   Dictionary<(string, char), (string, char, char)>
    // e congelada (FrozenDictionary) para lookup O(1) imutável.
    public MaquinaDeTuring(
        HashSet<string> q,
        HashSet<char> sigma,
        HashSet<char> gamma,
        Dictionary<(string, char), (string, char, char)> delta,
        string q0,
        char branco,
        HashSet<string> f,
        string nome = "MT",
        string qReject = "qreject")
    {
        ArgumentNullException.ThrowIfNull(q);
        ArgumentNullException.ThrowIfNull(sigma);
        ArgumentNullException.ThrowIfNull(gamma);
        ArgumentNullException.ThrowIfNull(delta);
        ArgumentException.ThrowIfNullOrWhiteSpace(q0);
        ArgumentNullException.ThrowIfNull(f);

        Q       = q.ToFrozenSet();
        Sigma   = sigma.ToFrozenSet();
        Gamma   = gamma.ToFrozenSet();
        Delta   = delta.ToFrozenDictionary();
        Q0      = q0;
        Branco  = branco;
        F       = f.ToFrozenSet();
        Nome    = nome;
        QReject = qReject;
    }


    public ResultadoMT Simular(string entrada, bool transdutor = false, int atrasoMs = 150, int maxPassos = 10_000)
    {
        ArgumentNullException.ThrowIfNull(entrada);
        if (atrasoMs < 0) atrasoMs = 0;
        if (maxPassos < 1) maxPassos = 10_000;

        // ── Fita dinâmica: Dictionary<int, char> ────────────────────────────
        // Célula i recebe entrada[i]; demais células valem B implicitamente.
        Dictionary<int, char> fita = new();
        for (int i = 0; i < entrada.Length; i++)
            fita[i] = entrada[i];

        int cabeca   = 0;     // posição da cabeça de leitura/escrita
        string estado = Q0;   // estado corrente
        int passo    = 0;     // contador de passos

        ExibirConfiguracao(estado, fita, cabeca, passo);
        if (atrasoMs > 0) Thread.Sleep(atrasoMs);

        ResultadoMT resultado;
        while (true)
        {
            // Critério de aceitação: parou em estado final.
            if (F.Contains(estado))
            {
                resultado = ResultadoMT.Aceita;
                break;
            }

            char lido = LerFita(fita, cabeca);

            // δ indefinida para (estado, lido) → transita para qreject (estado explícito).
            if (!Delta.TryGetValue((estado, lido), out var t))
            {
                estado = QReject;
                ExibirConfiguracao(estado, fita, cabeca, passo + 1);
                resultado = ResultadoMT.Rejeita;
                break;
            }

            // Aplica a transição: escreve, troca de estado e move a cabeça.
            EscreverFita(fita, cabeca, t.Escrito);
            estado  = t.NovoEstado;
            cabeca += Deslocamento(t.Movimento);
            passo++;

            if (passo > maxPassos)
            {
                resultado = ResultadoMT.LimiteExcedido;
                break;
            }

            ExibirConfiguracao(estado, fita, cabeca, passo);
            if (atrasoMs > 0) Thread.Sleep(atrasoMs);
        }

        ExibirResultado(resultado, fita, transdutor);
        return resultado;
    }

    // ── Operações sobre a fita dinâmica ─────────────────────────────────────

    // Lê a célula; células ausentes do dicionário valem B.
    private char LerFita(Dictionary<int, char> fita, int pos) =>
        fita.TryGetValue(pos, out char c) ? c : Branco;

    // Escreve na célula. Branco é armazenado normalmente (a poda de exibição
    // remove os brancos das extremidades — ver TrechoVisivel).
    private static void EscreverFita(Dictionary<int, char> fita, int pos, char simbolo) =>
        fita[pos] = simbolo;

    // Converte o símbolo de movimento em deslocamento inteiro.
    private static int Deslocamento(char movimento) => movimento switch
    {
        Direita   => +1,
        Esquerda  => -1,
        Permanece =>  0,
        _ => throw new InvalidOperationException($"Movimento inválido: '{movimento}'."),
    };

    // ── Exibição da configuração instantânea ────────────────────────────────

    // Calcula o intervalo [min, max] de células a exibir: a união das células
    // já escritas com a posição da cabeça, podando brancos das extremidades
    // (mas nunca escondendo a célula sob a cabeça).
    private (int min, int max) TrechoVisivel(Dictionary<int, char> fita, int cabeca)
    {
        int min = cabeca, max = cabeca;
        if (fita.Count > 0)
        {
            min = Math.Min(min, fita.Keys.Min());
            max = Math.Max(max, fita.Keys.Max());
        }
        while (min < cabeca && LerFita(fita, min) == Branco) min++;
        while (max > cabeca && LerFita(fita, max) == Branco) max--;
        return (min, max);
    }

    private void ExibirConfiguracao(string estado, Dictionary<int, char> fita, int cabeca, int passo)
    {
        (int min, int max) = TrechoVisivel(fita, cabeca);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"  Passo {passo,4} │ ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"Estado: {estado,-8}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("│ Fita: ");

        // Reticências indicam a fita infinita de brancos à esquerda.
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("… ");

        for (int i = min; i <= max; i++)
        {
            char simbolo = LerFita(fita, i);
            if (i == cabeca)
            {
                // Célula sob a cabeça destacada (fundo claro).
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($"[{simbolo}]");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($" {simbolo} ");
            }
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(" …");
        Console.Write($"  (cabeça @ {cabeca})");
        Console.ResetColor();
        Console.WriteLine();
    }

    private void ExibirResultado(ResultadoMT resultado, Dictionary<int, char> fita, bool transdutor)
    {
        Console.WriteLine();
        switch (resultado)
        {
            case ResultadoMT.Aceita:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(transdutor
                    ? "  ✔  PAROU EM ESTADO FINAL — computação concluída"
                    : "  ✔  RESULTADO: ACEITA");
                break;
            case ResultadoMT.Rejeita:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(transdutor
                    ? "  ✘  PAROU SEM ESTADO FINAL — entrada mal-formada"
                    : "  ✘  RESULTADO: REJEITA");
                break;
            case ResultadoMT.LimiteExcedido:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  ⚠  LIMITE DE PASSOS EXCEDIDO (possível laço infinito)");
                break;
        }
        Console.ResetColor();

        if (transdutor && resultado == ResultadoMT.Aceita)
        {
            string saida = ConteudoFita(fita);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  ⇒  Saída (fita): {(saida.Length == 0 ? "ε (vazia)" : $"\"{saida}\"")}  |  valor unário = {saida.Length}");
            Console.ResetColor();
        }
    }

    // Reconstrói o conteúdo não-branco da fita, da esquerda para a direita.
    private string ConteudoFita(Dictionary<int, char> fita)
    {
        IEnumerable<int> ocupadas = fita
            .Where(kv => kv.Value != Branco)
            .Select(kv => kv.Key);
        if (!ocupadas.Any()) return string.Empty;

        int min = ocupadas.Min(), max = ocupadas.Max();
        char[] buffer = new char[max - min + 1];
        for (int i = min; i <= max; i++)
            buffer[i - min] = LerFita(fita, i);
        return new string(buffer);
    }

    // ── Exibição da definição formal ─────────────────────────────────────────
    public void ExibirDefinicaoFormal()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  Definição formal — {Nome}:");
        Console.ResetColor();
        Console.WriteLine($"  Q  = {{ {string.Join(", ", Q.Order())} }}");
        Console.WriteLine($"  Σ  = {{ {string.Join(", ", Sigma.Order())} }}");
        Console.WriteLine($"  Γ  = {{ {string.Join(", ", Gamma.Order())} }}");
        Console.WriteLine($"  q0 = {Q0}");
        Console.WriteLine($"  B  = {Branco}   (símbolo branco)");
        Console.WriteLine($"  F  = {{ {string.Join(", ", F.Order())} }}");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  Transições δ(estado, lido) = (novo_estado, escrito, movimento):");
        Console.ResetColor();

        foreach (var kv in Delta
            .OrderBy(kv => kv.Key.Estado)
            .ThenBy(kv => kv.Key.Lido))
        {
            char lido    = kv.Key.Lido    == Branco ? Branco : kv.Key.Lido;
            char escrito = kv.Value.Escrito;
            string mov = kv.Value.Movimento switch
            {
                Direita   => "D (→)",
                Esquerda  => "E (←)",
                Permanece => "P (•)",
                _ => kv.Value.Movimento.ToString(),
            };
            Console.WriteLine(
                $"    δ({kv.Key.Estado,-4}, {lido}) → ({kv.Value.NovoEstado,-4}, {escrito}, {mov})");
        }
    }
}


using System.Collections.Frozen;

namespace AutomatoDePilha;


public sealed record Configuracao(string Estado, string EntradaRestante, Stack<char> Pilha)
{
    /// <summary>Clona a pilha para evitar efeitos colaterais entre ramos não-det.</summary>
    public Stack<char> ClonaPilha()
    {
       
        char[] arr = Pilha.ToArray();
        Stack<char> nova = new(arr.Length);
        for (int i = arr.Length - 1; i >= 0; i--)
            nova.Push(arr[i]);
        return nova;
    }

    public string PilhaParaString() =>
        Pilha.Count == 0 ? "∅ (vazia)" : string.Concat(Pilha); 
}


public readonly record struct ChaveTransicao(string Estado, char Simbolo, char TopoPilha);


public readonly record struct DestinoTransicao(string NovoEstado, string EmpilharCadeia);


public sealed class AutomatoDePilha
{
    

    /// <summary>Q — conjunto finito de estados (FrozenSet para lookup O(1) imutável).</summary>
    public FrozenSet<string> Q { get; }

    /// <summary>Σ — alfabeto de entrada.</summary>
    public FrozenSet<char> Sigma { get; }

    /// <summary>Γ — alfabeto da pilha.</summary>
    public FrozenSet<char> Gamma { get; }

    /// <summary>
    /// δ : Q × (Σ ∪ {ε}) × Γ  →  P(Q × Γ*)
    /// Chave : (estado, símbolo — '\0' = ε, topo_da_pilha)
    /// Valor : lista de (novo_estado, cadeia_a_empilhar)
    /// FrozenDictionary garante lookup sem alocações extras (.NET 8).
    /// </summary>
    public FrozenDictionary<ChaveTransicao, List<DestinoTransicao>> Delta { get; }

    /// <summary>q0 — estado inicial.</summary>
    public string Q0 { get; }

    /// <summary>Z0 — símbolo inicial da pilha.</summary>
    public char Z0 { get; }

    

    /// <summary>Rótulo descritivo para exibição.</summary>
    public string Nome { get; }

    
    public AutomatoDePilha(
        HashSet<string> q,
        HashSet<char> sigma,
        HashSet<char> gamma,
        Dictionary<ChaveTransicao, List<DestinoTransicao>> delta,
        string q0,
        char z0,
        string nome = "AP")
    {
        ArgumentNullException.ThrowIfNull(q);
        ArgumentNullException.ThrowIfNull(sigma);
        ArgumentNullException.ThrowIfNull(gamma);
        ArgumentNullException.ThrowIfNull(delta);
        ArgumentException.ThrowIfNullOrWhiteSpace(q0);

        
        Q     = q.ToFrozenSet();
        Sigma = sigma.ToFrozenSet();
        Gamma = gamma.ToFrozenSet();
        Delta = delta.ToFrozenDictionary();
        Q0    = q0;
        Z0    = z0;
        Nome  = nome;
    }

    

    /// <summary>
    /// Simula o AP sobre <paramref name="cadeia"/>, exibindo cada configuração
    /// instantânea passo a passo.
    /// Retorna <see langword="true"/> se existe algum ramo de computação que
    /// esvazia a pilha após consumir toda a entrada (aceitação por pilha vazia).
    /// </summary>
    public bool Simular(string cadeia)
    {
        ArgumentNullException.ThrowIfNull(cadeia);

        Stack<char> pilhaInicial = new();
        pilhaInicial.Push(Z0);

        bool aceita = BuscaDFS(new Configuracao(Q0, cadeia, pilhaInicial), passo: 0);

        Console.WriteLine();
        if (aceita)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ✔  RESULTADO: ACEITA");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ✘  RESULTADO: REJEITA");
        }
        Console.ResetColor();
        return aceita;
    }

    
    private bool BuscaDFS(Configuracao cfg, int passo)
    {
        const int MaxPassos = 200; 
        if (passo > MaxPassos) return false;

        ExibirConfiguracao(cfg, passo);

      
        if (cfg.EntradaRestante.Length == 0 && cfg.Pilha.Count == 0)
            return true;

        if (cfg.Pilha.Count == 0) return false; // ramo morto

        char topo = cfg.Pilha.Peek();

        
        if (Delta.TryGetValue(new ChaveTransicao(cfg.Estado, '\0', topo), out List<DestinoTransicao>? epsDestinos))
        {
            foreach (DestinoTransicao dest in epsDestinos)
            {
                Stack<char> novaPilha = cfg.ClonaPilha();
                AplicarTransicao(novaPilha, dest.EmpilharCadeia);
                if (BuscaDFS(new Configuracao(dest.NovoEstado, cfg.EntradaRestante, novaPilha), passo + 1))
                    return true;
            }
        }

      
        if (cfg.EntradaRestante.Length == 0) return false;

        char sim = cfg.EntradaRestante[0];
        if (!Sigma.Contains(sim)) return false; // símbolo fora de Σ

        if (!Delta.TryGetValue(new ChaveTransicao(cfg.Estado, sim, topo), out List<DestinoTransicao>? destinos))
            return false; 

       
        string resto = cfg.EntradaRestante[1..];

        foreach (DestinoTransicao dest in destinos)
        {
            Stack<char> novaPilha = cfg.ClonaPilha();
            AplicarTransicao(novaPilha, dest.EmpilharCadeia);
            if (BuscaDFS(new Configuracao(dest.NovoEstado, resto, novaPilha), passo + 1))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Aplica uma transição à pilha:
    ///   1. Pop do topo (símbolo que "disparou" a transição).
    ///   2. Push dos símbolos de <paramref name="empilhar"/> da direita para a
    ///      esquerda, de modo que o primeiro caractere fique no topo.
    ///      String vazia ("") → apenas pop.
    /// </summary>
    private static void AplicarTransicao(Stack<char> pilha, string empilhar)
    {
        pilha.Pop();
      
        ReadOnlySpan<char> span = empilhar.AsSpan();
        for (int i = span.Length - 1; i >= 0; i--)
            pilha.Push(span[i]);
    }

    
    private static void ExibirConfiguracao(Configuracao cfg, int passo)
    {
        string entrada = cfg.EntradaRestante.Length > 0
            ? $"\"{cfg.EntradaRestante}\""
            : "ε";

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"  Passo {passo,3} │ ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"Estado: {cfg.Estado,-6}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("│ ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"Entrada: {entrada,-14}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("│ ");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write($"Pilha (topo→base): {cfg.PilhaParaString()}");
        Console.ResetColor();
        Console.WriteLine();
    }

   
    public void ExibirDefinicaoFormal()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  Definição formal — {Nome}:");
        Console.ResetColor();
        Console.WriteLine($"  Q  = {{ {string.Join(", ", Q.Order())} }}");
        Console.WriteLine($"  Σ  = {{ {string.Join(", ", Sigma.Order())} }}");
        Console.WriteLine($"  Γ  = {{ {string.Join(", ", Gamma.Order())} }}");
        Console.WriteLine($"  q0 = {Q0}");
        Console.WriteLine($"  Z0 = {Z0}");
        Console.WriteLine($"  F  = ∅  (aceitação por pilha vazia)");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  Transições δ:");
        Console.ResetColor();

        foreach (var kv in Delta
            .OrderBy(kv => kv.Key.Estado)
            .ThenBy(kv => kv.Key.Simbolo)
            .ThenBy(kv => kv.Key.TopoPilha))
        {
            string s = kv.Key.Simbolo == '\0' ? "ε" : kv.Key.Simbolo.ToString();
            foreach (DestinoTransicao dest in kv.Value)
            {
                string emp = string.IsNullOrEmpty(dest.EmpilharCadeia) ? "ε" : dest.EmpilharCadeia;
                Console.WriteLine($"    δ({kv.Key.Estado}, {s}, {kv.Key.TopoPilha}) → ({dest.NovoEstado}, {emp})");
            }
        }
    }
}

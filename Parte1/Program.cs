using System.Text.Json;

/* Q — conjunto de estados. usei HashSet porque não permite estados duplicados*/
HashSet<string> Q = new() { "q0", "q1", "q2" };

// Sigma — alfabeto
HashSet<char> Sigma = new() { 'a', 'b' };

// q0 — estado inicial
string q0 = "q0";

// F — estados de aceitação
HashSet<string> F = new() { "q2" };

// delta — função de transição: (estado, símbolo) -> próximo estado
Dictionary<(string estado, char simbolo), string> delta = new()
{
    { ("q0", 'a'), "q1" },
    { ("q0", 'b'), "q0" },
    { ("q1", 'a'), "q1" },
    { ("q1", 'b'), "q2" },
    { ("q2", 'a'), "q1" },
    { ("q2", 'b'), "q0" },
};

bool Aceitar(string cadeia, out List<string> rastro)
{
    string estado = q0;
    rastro = new List<string> { estado };

    foreach (char simbolo in cadeia)
    {
        if (!Sigma.Contains(simbolo))
        {
            Console.WriteLine($"Erro: Símbolo '{simbolo}' não pertence ao alfabeto.");
            return false;
        }

        estado = delta[(estado, simbolo)];
        rastro.Add(estado);
    }

    return F.Contains(estado);
}

void ExibirDiagrama()
{
    Console.WriteLine("\nTabela de Transições (δ): \n");
    Console.WriteLine("Estado   | 'a'      | 'b'");
    Console.WriteLine("------------------------------");

    foreach (string estado in Q)
    {
        string destinoA;
        string destinoB;

        if (delta.ContainsKey((estado, 'a')))
        {
            destinoA = delta[(estado, 'a')];
        }
        else
        {
            destinoA = "-";
        }

        if (delta.ContainsKey((estado, 'b')))
        {
            destinoB = delta[(estado, 'b')];
        }
        else
        {
            destinoB = "-";
        }

        string marca = " ";

        if (F.Contains(estado))
        {
            marca = "*";
        }
        else if (estado == q0)
        {
            marca = ">";
        }

        Console.WriteLine(marca + estado + "      | " + destinoA + "      | " + destinoB);
    }

    Console.WriteLine("(* = aceitação, > = inicial) \n");   
}

// processa o entradas,txt
void ProcessarArquivo(string caminho)
{
    if (!File.Exists(caminho))
    {
        Console.WriteLine($"Arquivo {caminho} não encontrado.");
        return;
    }

    Console.WriteLine($"Arquivo {caminho} encontrado.");

    string[] linhas = File.ReadAllLines(caminho);

    Console.WriteLine($"Processando {caminho}");
    Console.WriteLine();

    foreach (string linha in linhas)
    {
        string exibicao = linha.Length == 0 ? "ε (vazia)" : linha;

        List<string> rastro;
        bool aceita = Aceitar(linha, out rastro);

        string resultado = aceita ? "ACEITA" : "REJEITA";

        Console.WriteLine($"Cadeia : \"{exibicao}\"");
        Console.WriteLine($"Rastro : {string.Join(" -> ", rastro)}");
        Console.WriteLine($"Resultado : {resultado}");
        Console.WriteLine();
    }
}

void ProcessaJson(string caminhoJson, string caminhoEntradas)
{
    if (!File.Exists(caminhoJson))
    {
        Console.WriteLine($"Arquivo '{caminhoJson}' não encontrado.");
        return;
    }

    Console.WriteLine($"Carregando AFD de {caminhoJson}\n");
    string json = File.ReadAllText(caminhoJson);

    using JsonDocument doc = JsonDocument.Parse(json);
    JsonElement root = doc.RootElement;

    HashSet<string> qjson = root.GetProperty("estados").EnumerateArray().Select(e => e.GetString()!).ToHashSet();

    HashSet<char> sjson = root.GetProperty("alfabeto").EnumerateArray().Select(e => e.GetString()![0]).ToHashSet();

    string inicialJson = root.GetProperty("estadoInicial").GetString()!;

    HashSet<string> fJson = root.GetProperty("estadosAceitacao").EnumerateArray().Select(e => e.GetString()!).ToHashSet();

    Dictionary<(string, char), string> deltaJson = new();
    foreach (JsonElement t in root.GetProperty("transicoes").EnumerateArray())
    {
        string origem  = t.GetProperty("origem").GetString()!;
        char   simbolo = t.GetProperty("simbolo").GetString()![0];
        string destino = t.GetProperty("destino").GetString()!;
        deltaJson[(origem, simbolo)] = destino;
    }

    Console.WriteLine($"Estados       : {{ {string.Join(", ", qjson.OrderBy(e => e))} }}");
    Console.WriteLine($"Alfabeto      : {{ {string.Join(", ", sjson.OrderBy(c => c))} }}");
    Console.WriteLine($"Estado inicial: {inicialJson}");
    Console.WriteLine($"Aceitação     : {{ {string.Join(", ", fJson)} }}");
    Console.WriteLine($"Transições    : {deltaJson.Count}\n");

    if (!File.Exists(caminhoEntradas)) return;

    foreach (string linha in File.ReadAllLines(caminhoEntradas))
    {
        string estadoAtual = inicialJson;
        List<string> rastro = new() { estadoAtual };
        bool valida = true;

        foreach (char simbolo in linha)
        {
            if (!sjson.Contains(simbolo))
            {
                valida = false; break;
            }
            if (!deltaJson.TryGetValue((estadoAtual, simbolo), out string? proximo))
            {
                valida = false; break;
            }
            estadoAtual = proximo;
            rastro.Add(estadoAtual);
        }

        bool aceita   = valida && fJson.Contains(estadoAtual);
        string exibicao  = linha.Length == 0 ? "(vazia)" : linha;
        string resultado = aceita ? "ACEITA" : "REJEITA";

        Console.WriteLine($"  \"{exibicao}\" -> {string.Join(" -> ", rastro)} -> {resultado}");
    }
    Console.WriteLine();
}

Console.WriteLine("Simulador Genérico de AFD");
Console.WriteLine("L1 = cadeias sobre {a,b} que terminam com 'ab'");

ExibirDiagrama();
ProcessarArquivo("entradas.txt");
ProcessaJson("afd.json", "entradas.txt");

Console.WriteLine("Pressione qualquer tecla para sair...");
Console.ReadKey();
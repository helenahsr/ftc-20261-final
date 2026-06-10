

using AutomatoDePilha;

Console.OutputEncoding = System.Text.Encoding.UTF8;

ExibirCabecalho();

// ── L2: { aⁿbⁿ | n ≥ 1 } ─────────────────────────────────────────────────
Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║        AP — L2 = { aⁿbⁿ | n ≥ 1 }  (pilha vazia)           ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

AutomatoDePilha.AutomatoDePilha apL2 = AutomatoL2.Criar();
apL2.ExibirDefinicaoFormal();
Console.WriteLine();

string[] cadeias_L2 = LerArquivoOuPadrao(
    "entradas_ap.txt",
    ["ab", "aabb", "aaabbb", "aab", "abb", "ba", "", "abab"]
);

Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
Console.WriteLine("│                    CASOS DE TESTE — L2                       │");
Console.WriteLine("└──────────────────────────────────────────────────────────────┘");

foreach (string cadeia in cadeias_L2)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"  ▶ Testando cadeia: {(cadeia.Length == 0 ? "ε (vazia)" : $"\"{cadeia}\"")}");
    Console.ResetColor();
    Console.WriteLine(new string('─', 64));
    apL2.Simular(cadeia);
    Console.WriteLine(new string('─', 64));
}

// ── L3: palíndromos ──────────────────────────────────────────────────────
Console.WriteLine();
Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║   AP — L3 = { w ∈ {a,b}* | w = wᴿ, |w| ≥ 1 }  (palínd.)  ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

AutomatoDePilha.AutomatoDePilha apL3 = AutomatoL3.Criar();
apL3.ExibirDefinicaoFormal();
Console.WriteLine();

string[] cadeias_L3 = LerArquivoOuPadrao(
    "entradas_ap_palindromos.txt",
    ["a", "aba", "abba", "ab", "aab"]
);

Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
Console.WriteLine("│                    CASOS DE TESTE — L3                       │");
Console.WriteLine("└──────────────────────────────────────────────────────────────┘");

foreach (string cadeia in cadeias_L3)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"  ▶ Testando cadeia: {(cadeia.Length == 0 ? "ε (vazia)" : $"\"{cadeia}\"")}");
    Console.ResetColor();
    Console.WriteLine(new string('─', 64));
    apL3.Simular(cadeia);
    Console.WriteLine(new string('─', 64));
}

Console.WriteLine();
ExibirAnaliseComparativa();

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("  Execução concluída. Pressione qualquer tecla para sair.");
Console.ResetColor();
Console.ReadKey();

// ── Funções locais ────────────────────────────────────────────────────────

// Lê linhas do arquivo; se não existir, usa o array padrão.
static string[] LerArquivoOuPadrao(string caminho, string[] padrao)
{
    if (File.Exists(caminho))
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  [INFO] Lendo cadeias de '{caminho}'...");
        Console.ResetColor();
        return File.ReadAllLines(caminho);
    }
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($"  [INFO] Arquivo '{caminho}' não encontrado. Usando casos de teste padrão.");
    Console.ResetColor();
    return padrao;
}

static void ExibirCabecalho()
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("================================================================");
    Console.WriteLine("  Faculdade Cotemig — Fundamentos Teóricos da Computação");
    Console.WriteLine("  Trabalho Final — Parte 2: Autômato de Pilha");
    Console.WriteLine("================================================================");
    Console.ResetColor();
    Console.WriteLine();
}

static void ExibirAnaliseComparativa()
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("================================================================");
    Console.WriteLine("  ANÁLISE COMPARATIVA: L2 versus L3");
    Console.WriteLine("================================================================");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine("  L2 = { aⁿbⁿ | n ≥ 1 } — Complexidade do AP:");
    Console.WriteLine("  • Apenas 2 estados necessários (q0 e q1).");
    Console.WriteLine("  • A pilha empilha 'a's e desempilha ao ler 'b's.");
    Console.WriteLine("  • Não há ambiguidade: a transição entre fases é determinística.");
    Console.WriteLine("  • O AP para L2 é DETERMINÍSTICO (ADPV).");
    Console.WriteLine();
    Console.WriteLine("  L3 = { w = wᴿ } — Complexidade do AP:");
    Console.WriteLine("  • Requer 2 estados, mas AP NECESSARIAMENTE NÃO-DETERMINÍSTICO.");
    Console.WriteLine("  • Precisa de λ-movimentos para 'adivinhar' o meio da cadeia.");
    Console.WriteLine("  • Palíndromos PAR e ÍMPAR exigem tratamento distinto.");
    Console.WriteLine("  • Nenhum ADPV reconhece L3 — não-determinismo é inerente.");
    Console.WriteLine("  • L3 é LLC mas NÃO é linguagem livre de contexto determinística.");
    Console.WriteLine();
}

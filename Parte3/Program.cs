
using MaquinaDeTuring;

Console.OutputEncoding = System.Text.Encoding.UTF8;

ExibirCabecalho();

// Velocidade da animação passo a passo (ms entre configurações).
const int atraso = 150;

// ── L₄: { aⁿbⁿcⁿ | n ≥ 1 } ────────────────────────────────────────────────
Console.WriteLine("MT — L₄ = { aⁿbⁿcⁿ | n ≥ 1 }  (reconhecedora)");
Console.WriteLine();

MaquinaDeTuring.MaquinaDeTuring mtL4 = MaquinaL4.Criar();
mtL4.ExibirDefinicaoFormal();
Console.WriteLine();

string[] cadeias_L4 = LerArquivoOuPadrao(
    "entradas_mt.txt",
    ["abc", "aabbcc", "aaabbbccc", "aabbc", "ab", "abcabc", "aabbccc", "", "aaabbbcc"]
);

Console.WriteLine("CASOS DE TESTE — L₄");

foreach (string cadeia in cadeias_L4)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"  ▶ Testando cadeia: {(cadeia.Length == 0 ? "ε (vazia)" : $"\"{cadeia}\"")}");
    Console.ResetColor();
    Console.WriteLine(new string('─', 64));
    mtL4.Simular(cadeia, atrasoMs: atraso);
    Console.WriteLine(new string('─', 64));
}

// ── DESAFIO: f(n) = n + 1 em unário ────────────────────────────────────────
Console.WriteLine();
Console.WriteLine("DESAFIO — MT transdutora:  f(n) = n + 1  (unário)");
Console.WriteLine();

MaquinaDeTuring.MaquinaDeTuring mtInc = MaquinaIncremento.Criar();
mtInc.ExibirDefinicaoFormal();
Console.WriteLine();
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("  Convenção: n é representado por n símbolos '1' (ε = 0).");
Console.ResetColor();

string[] entradas_inc = LerArquivoOuPadrao(
    "entradas_incremento.txt",
    ["", "1", "111", "11111"]
);

Console.WriteLine("CASOS DE TESTE — INCREMENTO UNÁRIO");

foreach (string entrada in entradas_inc)
{
    int n = entrada.Length;
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"  ▶ Computando f({n}) = {n + 1}  |  entrada: {(entrada.Length == 0 ? "ε (n=0)" : $"\"{entrada}\"")}");
    Console.ResetColor();
    Console.WriteLine(new string('─', 64));
    mtInc.Simular(entrada, transdutor: true, atrasoMs: atraso);
    Console.WriteLine(new string('─', 64));
}

Console.WriteLine();
ExibirAnaliseHierarquia();

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("  Execução concluída. Pressione qualquer tecla para sair.");
Console.ResetColor();
Console.ReadKey();

// ── Funções locais ──────────────────────────────────────────────────────────

// Lê linhas do arquivo; se não existir, usa o array padrão.
static string[] LerArquivoOuPadrao(string caminho, string[] padrao)
{
    if (File.Exists(caminho))
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  [INFO] Lendo entradas de '{caminho}'...");
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
    Console.WriteLine("  Trabalho Final — Parte 3: Máquina de Turing  (.NET 8)");
    Console.WriteLine("================================================================");
    Console.ResetColor();
    Console.WriteLine();
}

static void ExibirAnaliseHierarquia()
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("================================================================");
    Console.WriteLine("  ANÁLISE: por que L₄ exige uma Máquina de Turing?");
    Console.WriteLine("================================================================");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine("  L₄ = { aⁿbⁿcⁿ | n ≥ 1 } na Hierarquia de Chomsky:");
    Console.WriteLine("  • NÃO é regular  — um AFD não conta n (memória finita).");
    Console.WriteLine("  • NÃO é livre de contexto — o Lema do Bombeamento para LLC");
    Console.WriteLine("    falha: uma pilha casa DUAS contagens (aⁿbⁿ), não TRÊS.");
    Console.WriteLine("  • É sensível ao contexto (Tipo 1) — reconhecida por MT que");
    Console.WriteLine("    relê e remarca a fita em múltiplas passadas (X/Y/Z).");
    Console.WriteLine();
    Console.WriteLine("  A MT como TRANSDUTORA (desafio):");
    Console.WriteLine("  • Além de reconhecer linguagens, a MT computa FUNÇÕES.");
    Console.WriteLine("  • f(n) = n + 1 em unário não exige 'vai-um': basta anexar '1'.");
    Console.WriteLine("  • Ilustra a Tese de Church-Turing: o que é efetivamente");
    Console.WriteLine("    computável é exatamente o que uma MT computa.");
    Console.WriteLine();
}

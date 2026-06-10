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
// Q — conjunto de estados
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
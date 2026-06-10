
namespace MaquinaDeTuring;

public static class MaquinaL4
{
    public static MaquinaDeTuring Criar()
    {
        const char E = MaquinaDeTuring.Esquerda;
        const char D = MaquinaDeTuring.Direita;
        const char P = MaquinaDeTuring.Permanece;
        const char B = '_';

        //  Q 
        HashSet<string> q = ["q0", "q1", "q2", "q3", "q4", "q5", "qac", "qreject"];

        //  Σ — alfabeto de entrada
        HashSet<char> sigma = ['a', 'b', 'c'];

        //  Γ — fita: entrada + marcadores X/Y/Z + branco 
        HashSet<char> gamma = ['a', 'b', 'c', 'X', 'Y', 'Z', B];

        //  F 
        HashSet<string> f = ["qac"];

        //  δ 
        Dictionary<(string, char), (string, char, char)> delta = new()
        {
            // q0: acha o 'a' mais à esquerda → marca X e parte para casar o 'b'.
            [("q0", 'a')] = ("q1", 'X', D),
            // Sem mais 'a's: o que vier é Y → inicia a verificação final.
            [("q0", 'Y')] = ("q4", 'Y', D),

            // q1: anda à direita sobre a's e Y's até o primeiro 'b' → marca Y.
            [("q1", 'a')] = ("q1", 'a', D),
            [("q1", 'Y')] = ("q1", 'Y', D),
            [("q1", 'b')] = ("q2", 'Y', D),

            // q2: anda à direita sobre b's e Z's até o primeiro 'c' → marca Z e volta.
            [("q2", 'b')] = ("q2", 'b', D),
            [("q2", 'Z')] = ("q2", 'Z', D),
            [("q2", 'c')] = ("q3", 'Z', E),

            // q3: retorna à esquerda até o X já marcado; depois reposiciona em q0.
            [("q3", 'a')] = ("q3", 'a', E),
            [("q3", 'b')] = ("q3", 'b', E),
            [("q3", 'Y')] = ("q3", 'Y', E),
            [("q3", 'Z')] = ("q3", 'Z', E),
            [("q3", 'X')] = ("q0", 'X', D),

            // q4: verificação — só podem restar Y's, depois Z's.
            [("q4", 'Y')] = ("q4", 'Y', D),
            [("q4", 'Z')] = ("q5", 'Z', D),

            // q5: verificação — só podem restar Z's; branco final → ACEITA.
            [("q5", 'Z')] = ("q5", 'Z', D),
            [("q5", B)]   = ("qac", B, P),
        };

        return new MaquinaDeTuring(
            q: q, sigma: sigma, gamma: gamma, delta: delta,
            q0: "q0", branco: B, f: f,
            nome: "MT para L₄ = { aⁿbⁿcⁿ | n ≥ 1 }");
    }
}

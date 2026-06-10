namespace MaquinaDeTuring;

public static class MaquinaIncremento
{
    public static MaquinaDeTuring Criar()
    {
        const char D = MaquinaDeTuring.Direita;
        const char P = MaquinaDeTuring.Permanece;
        const char B = '_';

        //  Q 
        HashSet<string> q = ["q0", "qac", "qreject"];

        //  Σ — alfabeto de entrada (apenas o dígito unário '1') ─
        HashSet<char> sigma = ['1'];

        //  Γ — fita: '1' + branco 
        HashSet<char> gamma = ['1', B];

        //  F 
        HashSet<string> f = ["qac"];

        //  δ 
        Dictionary<(string, char), (string, char, char)> delta = new()
        {
            // Avança sobre cada '1' já existente.
            [("q0", '1')] = ("q0", '1', D),
            // Primeiro branco à direita: grava o novo '1' e para (aceita).
            [("q0", B)]   = ("qac", '1', P),
        };

        return new MaquinaDeTuring(
            q: q, sigma: sigma, gamma: gamma, delta: delta,
            q0: "q0", branco: B, f: f,
            nome: "MT transdutora — f(n) = n + 1 (unário)");
    }
}





namespace AutomatoDePilha;

public static class AutomatoL2
{
    public static AutomatoDePilha Criar()
    {
      
        HashSet<string> q = ["q0", "q1"];

      
        HashSet<char> sigma = ['a', 'b'];

        
        HashSet<char> gamma = ['A', 'Z'];

       
        Dictionary<ChaveTransicao, List<DestinoTransicao>> delta = new()
        {
          
            [new ChaveTransicao("q0", 'a', 'Z')] =
                [new DestinoTransicao("q0", "AZ")],

           
            [new ChaveTransicao("q0", 'a', 'A')] =
                [new DestinoTransicao("q0", "AA")],

            
            [new ChaveTransicao("q0", 'b', 'A')] =
                [new DestinoTransicao("q1", "")],

           
            [new ChaveTransicao("q1", 'b', 'A')] =
                [new DestinoTransicao("q1", "")],

          
            [new ChaveTransicao("q1", '\0', 'Z')] =
                [new DestinoTransicao("q1", "")],
        };

        return new AutomatoDePilha(
            q: q, sigma: sigma, gamma: gamma, delta: delta,
            q0: "q0", z0: 'Z',
            nome: "AP para L2 = { aⁿbⁿ | n ≥ 1 }");
    }
}

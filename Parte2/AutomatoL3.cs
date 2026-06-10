


namespace AutomatoDePilha;

public static class AutomatoL3
{
    public static AutomatoDePilha Criar()
    {
      
        HashSet<string> q = ["q0", "q1"];

        
        HashSet<char> sigma = ['a', 'b'];

       
        HashSet<char> gamma = ['A', 'B', 'Z'];

        
        Dictionary<ChaveTransicao, List<DestinoTransicao>> delta = new()
        {
            
            [new ChaveTransicao("q0", 'a', 'Z')] =
                [new DestinoTransicao("q0", "AZ"), new DestinoTransicao("q1", "Z")],

         
            [new ChaveTransicao("q0", 'b', 'Z')] =
                [new DestinoTransicao("q0", "BZ"), new DestinoTransicao("q1", "Z")],

            
            [new ChaveTransicao("q0", 'a', 'A')] =
                [new DestinoTransicao("q0", "AA"), new DestinoTransicao("q1", "A")],

         
            [new ChaveTransicao("q0", 'a', 'B')] =
                [new DestinoTransicao("q0", "AB"), new DestinoTransicao("q1", "B")],

          
            [new ChaveTransicao("q0", 'b', 'A')] =
                [new DestinoTransicao("q0", "BA"), new DestinoTransicao("q1", "A")],

            [new ChaveTransicao("q0", 'b', 'B')] =
                [new DestinoTransicao("q0", "BB"), new DestinoTransicao("q1", "B")],

           
            [new ChaveTransicao("q0", '\0', 'A')] =
                [new DestinoTransicao("q1", "A")],

           
            [new ChaveTransicao("q0", '\0', 'B')] =
                [new DestinoTransicao("q1", "B")],

           
            [new ChaveTransicao("q0", '\0', 'Z')] =
                [new DestinoTransicao("q1", "Z")],

            
            [new ChaveTransicao("q1", 'a', 'A')] =
                [new DestinoTransicao("q1", "")],

            
            [new ChaveTransicao("q1", 'b', 'B')] =
                [new DestinoTransicao("q1", "")],

           
            [new ChaveTransicao("q1", '\0', 'Z')] =
                [new DestinoTransicao("q1", "")],

           
        };

        return new AutomatoDePilha(
            q: q, sigma: sigma, gamma: gamma, delta: delta,
            q0: "q0", z0: 'Z',
            nome: "AP para L3 = { w = wᴿ | w ∈ {a,b}*, |w| ≥ 1 }");
    }
}

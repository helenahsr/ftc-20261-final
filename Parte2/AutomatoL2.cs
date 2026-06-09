// =============================================================================
// AutomatoL2.cs  (.NET 8 / C# 12)
// Fábrica do AP para L2 = { aⁿbⁿ | n ≥ 1 }
//
// Estratégia:
//   Fase 1 (q0): a cada 'a' lido, empilha 'A' sobre Z0.
//   Fase 2 (q1): a cada 'b' lido, desempilha 'A'.
//   Transição q0→q1 ocorre ao ler o primeiro 'b'.
//   λ-movimento final em q1 remove Z0 → pilha vazia → ACEITA.
//
// Definição formal:
//   Q  = { q0, q1 }   Σ = { a, b }   Γ = { A, Z }
//   δ(q0, a, Z) = (q0, AZ)
//   δ(q0, a, A) = (q0, AA)
//   δ(q0, b, A) = (q1, ε)
//   δ(q1, b, A) = (q1, ε)
//   δ(q1, ε, Z) = (q1, ε)
// =============================================================================

namespace AutomatoDePilha;

public static class AutomatoL2
{
    public static AutomatoDePilha Criar()
    {
        // ── Q ────────────────────────────────────────────────────────────────
        HashSet<string> q = ["q0", "q1"];

        // ── Σ ────────────────────────────────────────────────────────────────
        HashSet<char> sigma = ['a', 'b'];

        // ── Γ — 'A' = marcador de 'a' empilhado; 'Z' = fundo de pilha (Z0) ─
        HashSet<char> gamma = ['A', 'Z'];

        // ── δ ────────────────────────────────────────────────────────────────
        Dictionary<ChaveTransicao, List<DestinoTransicao>> delta = new()
        {
            // δ(q0, a, Z) = (q0, AZ)
            // Primeiro 'a': pop Z, push Z, push A  →  topo: A, base: Z
            [new ChaveTransicao("q0", 'a', 'Z')] =
                [new DestinoTransicao("q0", "AZ")],

            // δ(q0, a, A) = (q0, AA)
            // Demais 'a's: pop A, push A, push A  →  mais um A no topo
            [new ChaveTransicao("q0", 'a', 'A')] =
                [new DestinoTransicao("q0", "AA")],

            // δ(q0, b, A) = (q1, ε)
            // Primeiro 'b': inicia desempilhamento; remove A. "" = ε (pop sem push).
            [new ChaveTransicao("q0", 'b', 'A')] =
                [new DestinoTransicao("q1", "")],

            // δ(q1, b, A) = (q1, ε)
            // Demais 'b's: remove A.
            [new ChaveTransicao("q1", 'b', 'A')] =
                [new DestinoTransicao("q1", "")],

            // δ(q1, ε, Z) = (q1, ε)
            // λ-movimento: após consumir todos os A's, desempilha Z0.
            // Pilha fica VAZIA → aceitação.
            [new ChaveTransicao("q1", '\0', 'Z')] =
                [new DestinoTransicao("q1", "")],
        };

        return new AutomatoDePilha(
            q: q, sigma: sigma, gamma: gamma, delta: delta,
            q0: "q0", z0: 'Z',
            nome: "AP para L2 = { aⁿbⁿ | n ≥ 1 }");
    }
}

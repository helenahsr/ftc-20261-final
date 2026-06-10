// =============================================================================
// AutomatoL3.cs  (.NET 8 / C# 12)
// Fábrica do AP para L3 = { w ∈ {a,b}* | w = wᴿ, |w| ≥ 1 }  (palíndromos)
//
// Estratégia (não-determinística, obrigatória):
//   Fase 1 (q0): empilha cada símbolo lido (A para 'a', B para 'b').
//   Transição ao centro — NÃO-DETERMINISMO:
//     • Par  : λ-movimento q0→q1 sem consumir entrada (mantém topo da pilha).
//     • Ímpar: ao ler o símbolo central x, consome x mas re-empilha o mesmo
//              topo (pop + push do mesmo símbolo), transitando para q1.
//              Ex.: δ(q0,'a','A') += (q1,'A') — "passa pelo centro".
//   Fase 2 (q1): desempilha verificando o espelho da primeira metade.
//
//   Por que NÃO-DETERMINÍSTICO?
//   Não há como saber deterministicamente quando se chegou ao meio.
//   O AP testa todos os ramos (DFS). Nenhum ADPV reconhece L3.
//
//   Q  = { q0, q1 }   Σ = { a, b }   Γ = { A, B, Z }
//   δ(q0, a, Z) = { (q0,AZ), (q1,Z) }    δ(q0, b, Z) = { (q0,BZ), (q1,Z) }
//   δ(q0, a, A) = { (q0,AA), (q1,A) }    δ(q0, b, B) = { (q0,BB), (q1,B) }
//   δ(q0, a, B) = { (q0,AB), (q1,B) }    δ(q0, b, A) = { (q0,BA), (q1,A) }
//   δ(q0, ε, A) = { (q1,A) }             δ(q0, ε, B) = { (q1,B) }
//   δ(q0, ε, Z) = { (q1,Z) }
//   δ(q1, a, A) = { (q1,ε) }             δ(q1, b, B) = { (q1,ε) }
//   δ(q1, ε, Z) = { (q1,ε) }
// =============================================================================

namespace AutomatoDePilha;

public static class AutomatoL3
{
    public static AutomatoDePilha Criar()
    {
        // ── Q ────────────────────────────────────────────────────────────────
        HashSet<string> q = ["q0", "q1"];

        // ── Σ ────────────────────────────────────────────────────────────────
        HashSet<char> sigma = ['a', 'b'];

        // ── Γ — A='a' empilhado, B='b' empilhado, Z=fundo (Z0) ──────────────
        HashSet<char> gamma = ['A', 'B', 'Z'];

        // ── δ ────────────────────────────────────────────────────────────────
        // Usa collection expressions (C# 12) para inicializar as listas.
        Dictionary<ChaveTransicao, List<DestinoTransicao>> delta = new()
        {
            // ── FASE 1: empilhamento em q0 ────────────────────────────────

            // δ(q0, a, Z) = { (q0,AZ), (q1,Z) }
            // Empilha A sobre Z0, OU 'a' é o único símbolo (centro único).
            [new ChaveTransicao("q0", 'a', 'Z')] =
                [new DestinoTransicao("q0", "AZ"), new DestinoTransicao("q1", "Z")],

            // δ(q0, b, Z) = { (q0,BZ), (q1,Z) }
            [new ChaveTransicao("q0", 'b', 'Z')] =
                [new DestinoTransicao("q0", "BZ"), new DestinoTransicao("q1", "Z")],

            // δ(q0, a, A) = { (q0,AA), (q1,A) }
            // Empilha, ou 'a' é centro ímpar: consome 'a', mantém A no topo.
            [new ChaveTransicao("q0", 'a', 'A')] =
                [new DestinoTransicao("q0", "AA"), new DestinoTransicao("q1", "A")],

            // δ(q0, a, B) = { (q0,AB), (q1,B) }
            // 'a' pode ser centro ímpar com B embaixo.
            [new ChaveTransicao("q0", 'a', 'B')] =
                [new DestinoTransicao("q0", "AB"), new DestinoTransicao("q1", "B")],

            // δ(q0, b, A) = { (q0,BA), (q1,A) }
            // 'b' pode ser centro ímpar com A embaixo.
            [new ChaveTransicao("q0", 'b', 'A')] =
                [new DestinoTransicao("q0", "BA"), new DestinoTransicao("q1", "A")],

            // δ(q0, b, B) = { (q0,BB), (q1,B) }
            [new ChaveTransicao("q0", 'b', 'B')] =
                [new DestinoTransicao("q0", "BB"), new DestinoTransicao("q1", "B")],

            // ── TRANSIÇÕES λ ao centro (comprimento PAR) ──────────────────

            // δ(q0, ε, A) = { (q1, A) }
            // "Aposta" não-det. que chegou ao meio; não consome entrada.
            [new ChaveTransicao("q0", '\0', 'A')] =
                [new DestinoTransicao("q1", "A")],

            // δ(q0, ε, B) = { (q1, B) }
            [new ChaveTransicao("q0", '\0', 'B')] =
                [new DestinoTransicao("q1", "B")],

            // δ(q0, ε, Z) = { (q1, Z) }
            [new ChaveTransicao("q0", '\0', 'Z')] =
                [new DestinoTransicao("q1", "Z")],

            // ── FASE 2: verificação (espelho) em q1 ───────────────────────

            // δ(q1, a, A) = { (q1, ε) }  — 'a' espelha A: desempilha.
            [new ChaveTransicao("q1", 'a', 'A')] =
                [new DestinoTransicao("q1", "")],

            // δ(q1, b, B) = { (q1, ε) }  — 'b' espelha B: desempilha.
            [new ChaveTransicao("q1", 'b', 'B')] =
                [new DestinoTransicao("q1", "")],

            // δ(q1, ε, Z) = { (q1, ε) }  — desempilha Z0 → pilha vazia → ACEITA.
            [new ChaveTransicao("q1", '\0', 'Z')] =
                [new DestinoTransicao("q1", "")],

            // NOTA: δ(q1, a, B) e δ(q1, b, A) são indefinidas → rejeição do ramo.
        };

        return new AutomatoDePilha(
            q: q, sigma: sigma, gamma: gamma, delta: delta,
            q0: "q0", z0: 'Z',
            nome: "AP para L3 = { w = wᴿ | w ∈ {a,b}*, |w| ≥ 1 }");
    }
}

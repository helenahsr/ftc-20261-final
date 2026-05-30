# Implementação de Máquinas Abstratas

Trabalho Final — Fundamentos Teóricos da Computação  
Faculdade Cotemig · Professor: Júlio César da Silva · 2026/1

---

## Grupo

| Nome Completo | Matrícula | Parte responsável |
|---|---|---|
| Helena Santos Rezende | 72400684 | Parte 1 — AFD |
| Gustavo Guimarães Aguiar Lima | 72400277 | Parte 2 — Autômato de Pilha |
| João Gabriel Rocha Rosa | 72400234 | Parte 3 — Máquina de Turing |

---

## Estrutura do Repositório

```
ftc-20261-final/
├── Parte1/          # AFD — Autômato Finito Determinístico
├── Parte2/          # Autômato de Pilha (pilha vazia)
├── Parte3/          # Máquina de Turing
├── docs/
│   └── relatorio.pdf
├── .gitignore
└── README.md
```

---

## Descrição das Partes

### Parte 1 — AFD (Autômato Finito Determinístico)
Simulador genérico de AFD que reconhece a linguagem:

> **L₁ = { w ∈ {a,b}* | w termina com "ab" }**

- Representa o AFD como a 5-tupla formal `(Q, Σ, δ, q0, F)`
- Função de transição `δ` implementada como `Dictionary<(string, char), string>`
- Lê cadeias de `entradas.txt` e exibe rastro de estados + resultado
- Suporta carregamento dinâmico de qualquer AFD via `afd.json`

### Parte 2 — Autômato de Pilha
Simulador de AP com **aceitação por pilha vazia** para a linguagem:

> **L₂ = { aⁿbⁿ | n ≥ 1 }**

- Representa o AP como a 7-tupla formal `(Q, Σ, Γ, δ, q0, Z0, ∅)`
- Pilha implementada com `Stack<char>`
- Exibe configuração instantânea a cada passo (estado, pilha, entrada restante)
- **Desafio:** segundo AP para palíndromos `L₃ = { w ∈ {a,b}* | w = wᴿ, |w| ≥ 1 }`

### Parte 3 — Máquina de Turing
Simulador de MT para a linguagem:

> **L₄ = { aⁿbⁿcⁿ | n ≥ 1 }**

- Fita dinâmica implementada como `Dictionary<int, char>`
- Função de transição `δ` como `Dictionary<(string, char), (string, char, char)>`
- Exibe configuração completa a cada passo com contador de passos
- **Desafio:** segunda MT que computa `f(n) = n + 1` em unário

---

## Como Compilar e Executar

> Requisito: [.NET 6 SDK](https://dotnet.microsoft.com/download) ou superior instalado.

### Parte 1 — AFD
```bash
cd Parte1
dotnet run
```

### Parte 2 — Autômato de Pilha
```bash
cd Parte2
dotnet run
```

### Parte 3 — Máquina de Turing
```bash
cd Parte3
dotnet run
```

---

## Vídeo de Defesa

> Link do vídeo: _await_

---

## Referências

- SIPSER, Michael. *Introduction to the Theory of Computation*. 3. ed. Cengage, 2013.
- HOPCROFT, J.; MOTWANI, R.; ULLMAN, J. *Introdução à Teoria de Autômatos, Linguagens e Computação*. Elsevier, 2003.
- MENEZES, Paulo Blauth. *Linguagens Formais e Autômatos*. 6. ed. Bookman, 2010.
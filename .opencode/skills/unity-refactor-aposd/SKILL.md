---
name: unity-refactor-aposd
description: Refactor Unity code to reduce complexity, hide implementation details, and improve module depth. Use when Unity code feels awkward, responsibilities leak across classes, one change spreads across files, or a MonoBehaviour has become hard to reason about.
---

# Unity Refactor Aposd

1. Diagnose leaked knowledge, temporal coupling, shallow interfaces, or scattered special cases in the affected owner and callers.
2. Compare plausible shapes when a boundary changes.
3. Choose the shape that reduces caller knowledge and keeps related state and invariants together.
4. Preserve behavior unless the request includes a behavior change.
5. Run the narrowest available deterministic check and inspect the diff for unrelated cleanup.

Report the complexity reduced, the ownership change, remaining intentional debt, and validation result.

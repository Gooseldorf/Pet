---
name: unity-refactor-aposd
description: Refactor Unity code to reduce complexity, hide implementation details, and improve module depth. Use when Unity code feels awkward, responsibilities leak across classes, one change spreads across files, or a MonoBehaviour has become hard to reason about.
---

# Unity Refactor Aposd

Refactor Unity code using `A Philosophy of Software Design` principles adapted for gameplay and scene-linked code.

## Goals

- reduce cognitive load for future changes
- make Unity code deeper and less leaky
- keep hidden details inside the class or module that owns them
- remove shallow wrappers and awkward decompositions

## When To Use

Use this skill when asked to:

- refactor Unity code
- clean up a large or awkward `MonoBehaviour`
- simplify an overgrown UI or gameplay script
- reduce coupling between classes or files
- improve code organization without changing behavior

## Workflow

1. Identify the current complexity: leaked knowledge, shallow abstraction, temporal coupling, or scattered special cases.
2. Compare at least two plausible shapes if the refactor changes boundaries or APIs.
3. Prefer the design that reduces the amount of information callers must know.
4. Pull messy detail downward into the owning module instead of spreading it across call sites.
5. Keep the refactor behavior-preserving unless the user also asked for a feature change.

## Decision Rules

- Prefer deep modules with small semantic interfaces over pass-through helpers and thin wrappers.
- Keep related state, invariants, and behavior together unless a new boundary clearly hides complexity.
- Do not split code just to make methods shorter if the new shape adds jumps and names without reducing reasoning cost.
- Do not split files or classes mechanically to satisfy style metrics or a dogmatic component count.
- Avoid exposing setup order, flags, or intermediate states when the owning class can define a stronger operation instead.
- Hide UI formatting, counter bookkeeping, lookup details, or special-case handling inside the module that owns them.
- Remove abstraction layers that add ceremony without isolating volatility or reducing caller knowledge.
- Use comments only for contracts, invariants, and non-obvious design decisions, not to narrate obvious code.
- Keep refactoring separate from unrelated cleanup.

## Output Expectations

When finishing a refactor:

1. state what complexity was reduced
2. name the boundary or ownership change that achieved it
3. mention any remaining design debt that was intentionally left in place

## Bundled References

- `references/smells-and-moves.md`

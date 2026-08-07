---
name: unity-architecture-review
description: Review general Unity code or proposed designs for ownership, coupling, initialization, and abstraction boundaries. Use for cross-cutting architecture reviews or non-trivial feature planning; prefer the UI or spider skill for those domains.
---

# Unity Architecture Review

Inspect the affected code, callers, serialized integration, and initialization path before judging the design.

1. Separate behavior rules from Unity, UI, scene, async, and networking glue.
2. Identify coupling, leaked setup order, framework spread, or abstractions that only forward calls.
3. Compare the smallest plausible shapes when a boundary changes.
4. Recommend only the lightest boundary justified by concrete volatility, reuse, or clarity.
5. Load multiplayer review if peer-visible state or authority is in scope.

Return findings first: `Assessment`, `Primary issue`, `Recommended shape`, and `Avoid`. Justify subjective concerns with repository evidence.

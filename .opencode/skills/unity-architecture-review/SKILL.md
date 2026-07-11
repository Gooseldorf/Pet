---
name: unity-architecture-review
description: Review Unity code and proposed designs for clean boundaries, appropriate abstraction level, and framework leakage. Use when the user asks for an architecture review, plans a non-trivial Unity feature, or wants to check whether gameplay logic, UI glue, and reusable code are separated sensibly.
---

# Unity Architecture Review

Review Unity code and designs for boundary quality without importing unnecessary enterprise layering.

## Goals

- keep gameplay rules independent from avoidable Unity and UI coupling
- prevent framework details from becoming the architecture
- introduce boundaries only when they buy clarity, testability, or volatility isolation
- push back on needless layers and speculative structure

## When To Use

Use this skill when asked to:

- review Unity architecture
- plan a non-trivial gameplay system
- evaluate whether to extract plain C# classes from `MonoBehaviour`
- check if UI, scene glue, and gameplay logic are separated well
- assess maintainability before implementation or refactor

## Review Workflow

1. Identify the gameplay rule, the Unity-specific glue, and any external detail such as UI, scene references, async flow, or networking.
2. Check whether important rules are trapped inside framework callbacks or view classes.
3. Check whether the proposed abstraction level matches actual volatility and reuse needs.
4. Recommend the lightest boundary that materially improves the design.
5. Call out where a simpler structure is better than a more layered one.

## Review Rules

- Gameplay and domain rules should not be inseparable from button handlers, TMP updates, or scene lookup details.
- Prefer plain C# types for reusable rules when that reduces Unity coupling and clarifies ownership.
- Keep `MonoBehaviour` classes as orchestration and lifecycle glue when possible, but do not extract logic mechanically.
- Do not introduce service, repository, presenter, or use-case layers unless they solve a concrete problem in this codebase.
- Framework details should stay near the edge of the behavior they support, not spread through the entire implementation.
- Organize code by concrete behavior and ownership before generic technical buckets.
- If a proposed layer only forwards data or wraps one implementation, treat it as suspect.
- Evaluate multiplayer implications separately whenever state flow or authority may be affected.

## Output Format

Return:

1. `Assessment`: acceptable, needs simplification, or needs boundary changes
2. `Primary issue`: the main architectural risk
3. `Recommended shape`: the lightest better structure
4. `Avoid`: layers or patterns that should not be introduced here

---
name: unity-feature-implementation
description: Implement or modify Unity gameplay, UI, or scene-linked code with clean boundaries and minimal complexity. Use when the user asks to add a Unity feature, change MonoBehaviour behavior, wire UI interactions, or extend authored runtime code under `Assets/_Root/`.
---

# Unity Feature Implementation

Implement Unity features with simple boundaries, editor-friendly wiring, and minimal abstraction.

## Goals

- keep authored Unity code easy to understand and change
- prefer serialized wiring over runtime lookup
- keep Unity glue separate from reusable gameplay logic when that split materially helps
- avoid speculative architecture and one-off abstractions

## When To Use

Use this skill when asked to:

- add a Unity feature
- change gameplay or UI behavior in `Assets/_Root/Scripts/`
- wire buttons, TMP fields, or other scene references
- extend scene-linked MonoBehaviour code
- implement a small gameplay flow in Unity

## Workflow

1. Identify the behavior change and the scene or prefab wiring it depends on.
2. Decide whether the logic belongs directly in a `MonoBehaviour` or in a small plain C# helper.
3. Prefer serialized references, config references, or authored wiring over runtime hierarchy search.
4. Keep the implementation as small as possible while preserving clear ownership.
5. If fixing a bug, check multiple plausible causes before locking onto one implementation path.
6. If code changes require prefab, scene, or Inspector wiring, state exactly what must be assigned in the Unity Editor.
7. After a meaningful code change, run the most relevant feasible verification step.

## Decision Rules

- Keep logic in one `MonoBehaviour` when the behavior is small and tightly coupled to a specific scene object.
- Prefer composition over inheritance for authored gameplay code, but do not split code mechanically when one focused owner is clearer.
- Extract plain C# logic only when it reduces cognitive load, isolates rules, or avoids mixing UI glue with gameplay rules.
- Prefer typed serialized fields over `GameObject` references plus `GetComponent`.
- Do not use `GameObject.Find`, `Transform.Find`, or similar hierarchy search unless authored wiring genuinely cannot solve it.
- Do not add defensive null-guard noise for required references that should be wired correctly in the Editor.
- `OnEnable` and `OnDisable` are acceptable for simple local event subscriptions, but explicit initialization wins when startup order matters.
- Do not introduce generic managers, services, or helper layers for a single concrete use.
- Touch only the code needed for the requested behavior and any cleanup directly caused by the change.
- If the requirement is ambiguous, clarify the missing behavior before locking in a design.

## Output Expectations

When finishing implementation work:

1. state the behavior that changed
2. name any required Unity Editor wiring
3. call out multiplayer implications if the feature can affect authority, sync, or host/client flow
4. mention the verification step that was run, or why verification was not feasible

## Bundled References

- `references/checklist.md`

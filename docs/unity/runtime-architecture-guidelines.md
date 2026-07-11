# Runtime Architecture Guidelines

## Purpose

This page defines the current preferred runtime code shape for authored Unity code in this repository.

## Scope

This is a project guideline for code structure and ownership.

It is not a claim that the repository already contains a large established gameplay architecture.

## Source Of Truth

- `AGENTS.md`
- `Assets/_Root/Scripts/`
- `docs/unity/project-structure.md`

## Current Project Reality

The current authored runtime surface is still small.

At the moment, the main authored script is:

- `Assets/_Root/Scripts/Test/MainMenuTester.cs`

That means these guidelines are intended to shape future growth, not to describe a mature existing architecture.

## Preferred Boundaries

### `MonoBehaviour`

Prefer `MonoBehaviour` classes for:

- scene lifecycle hooks
- serialized reference wiring
- direct interaction with Unity components
- orchestration of a small scene-specific behavior

Keep the behavior in one `MonoBehaviour` when splitting it would only add indirection.

### Plain C# Classes

Prefer plain C# classes when a behavior:

- represents reusable gameplay rules
- becomes harder to reason about because of Unity callback noise
- should be testable without scene or component setup
- does not need to inherit from Unity framework types

Do not extract helpers mechanically.

Only do it when the split reduces cognitive load or isolates a real ownership boundary.

## UI And Scene Glue

- UI classes should primarily coordinate presentation and input handling.
- Avoid letting UI-owning `MonoBehaviour` classes become the long-term home of gameplay rules.
- Prefer serialized references for TMP, buttons, and authored components.
- Avoid runtime hierarchy search for wiring except when authored setup genuinely cannot provide the reference.

## Reference Wiring

- Prefer typed serialized references over `GameObject` references plus `GetComponent`.
- Required references should usually fail loudly when not assigned correctly.
- Do not compensate for missing authored wiring with broad defensive null handling.

## Abstraction Rules

- Prefer the smallest design that keeps ownership clear.
- Avoid introducing managers, services, repositories, or generic utility layers without a concrete need.
- Prefer deeper modules that hide messy details over shallow wrappers that only forward calls.
- When a design decision is non-trivial, choose the shape that reduces what future readers must keep in their head.

## Multiplayer Expectations

When gameplay code can affect multiplayer behavior, always review:

- authority ownership
- host versus client execution flow
- RPC and synchronization path
- desync risk

Single-player assumptions should be stated explicitly when they are relied on.

## Unity Editor Boundaries

- Do not replace required prefab or scene integration with runtime hacks.
- If a code change requires Inspector, prefab, or scene setup, document exactly what must be wired in the Unity Editor.

## Related Files

- `AGENTS.md`
- `Assets/_Root/Scripts/Test/MainMenuTester.cs`
- `docs/unity/project-structure.md`

## Related Docs

- `project-structure.md`
- `../ai/coding-skills.md`

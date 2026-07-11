# Coding Skills

## Purpose

This page maps the local AI coding skills used for implementation and review work in this repository.

## Scope

This is a retrieval and navigation page for agents and humans.

It does not replace `AGENTS.md`.

## Source Of Truth

- `.opencode/skills/`
- `AGENTS.md`
- `docs/unity/runtime-architecture-guidelines.md`

## Current Skills

### `unity-feature-implementation`

Use for routine Unity code changes under `Assets/_Root/`.

Primary concerns:

- serialized wiring
- minimal abstraction
- clear ownership in `MonoBehaviour` or a small helper
- explicit Unity Editor follow-up when scene or prefab wiring is needed

Do not use as the main skill for architecture-heavy refactors or multiplayer audits.

### `unity-refactor-aposd`

Use for refactors where the code has become awkward, leaky, or harder to change.

Primary concerns:

- cognitive load reduction
- deep modules over shallow wrappers
- hiding details inside the owning class or module
- reducing change amplification across files

Do not use as the main skill for first-pass feature implementation unless cleanup is the main task.

### `unity-architecture-review`

Use for non-trivial structure decisions and architecture reviews.

Primary concerns:

- gameplay rules versus Unity glue
- avoiding framework leakage
- choosing the lightest useful boundary
- avoiding speculative layers

Do not use as a replacement for concrete implementation guidance.

### `unity-multiplayer-review`

Use whenever a feature can affect network authority, synchronization, or peer consistency.

Primary concerns:

- authority ownership
- host/client flow
- RPC and sync path completeness
- desync risk

Use even when the feature appears small if state can diverge between peers.

## Relationship To `AGENTS.md`

`AGENTS.md` remains the always-on baseline for project constraints.

The local coding skills add task-specific workflow and review pressure on top of that baseline.

## Related Files

- `.opencode/skills/unity-feature-implementation/SKILL.md`
- `.opencode/skills/unity-refactor-aposd/SKILL.md`
- `.opencode/skills/unity-architecture-review/SKILL.md`
- `.opencode/skills/unity-multiplayer-review/SKILL.md`
- `AGENTS.md`
- `docs/unity/runtime-architecture-guidelines.md`

## Related Docs

- `assistant-entrypoint.md`
- `retrieval-map.md`
- `../unity/project-structure.md`

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
- explicit startup order when a feature touches additive-scene initialization or scene handoff
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

### `unity-ui-flow-implementation`

Use for project-specific Unity UI work that should follow the shared screen, popup, HUD, and overlay flow already established under `Assets/_Root/Scripts/UI/`.

Primary concerns:

- choosing the correct UI primitive
- preserving config-driven prefab wiring and VContainer scopes
- keeping shared flow separate from scene-specific UI controllers
- explicit Unity Editor follow-up when prefabs, config assets, or Inspector references must be wired

Use this when the task is concretely about implementing or changing UI flow in this project, not just generic Unity feature work.

### `unity-ui-flow-review`

Use for project-specific UI architecture review and placement decisions.

Primary concerns:

- whether something should be a screen, popup, HUD, or overlay
- whether behavior belongs in shared flow or a scene slice
- whether a proposal fits the existing UI navigation, popup queue, back flow, and DI wiring
- avoiding duplicate managers and scene-local shortcuts that bypass the established UI layer

Use this before implementation when the main question is architectural fit.

### `unity-spider-player-implementation`

Use for planned work on the player-controlled spider, especially when the task touches wall or ceiling traversal, arbitrary surface orientation, spider jumping, or web attach, pull, and swing behavior.

Primary concerns:

- following the staged roadmap in `docs/unity/spider-player-controller-plan.md`
- preserving the component boundaries around `SpiderPlayerController`, surface, orientation, movement, jump, and web behavior
- keeping locomotion, camera, and IK ownership separate
- treating the feature as `single-player only` until multiplayer is explicitly designed

Use this when the task is specifically about the spider player controller rather than generic Unity gameplay implementation.

## Relationship To `AGENTS.md`

`AGENTS.md` remains the always-on baseline for project constraints.

The local coding skills add task-specific workflow and review pressure on top of that baseline.

## Related Files

- `.opencode/skills/unity-feature-implementation/SKILL.md`
- `.opencode/skills/unity-refactor-aposd/SKILL.md`
- `.opencode/skills/unity-architecture-review/SKILL.md`
- `.opencode/skills/unity-multiplayer-review/SKILL.md`
- `.opencode/skills/unity-ui-flow-implementation/SKILL.md`
- `.opencode/skills/unity-ui-flow-review/SKILL.md`
- `.opencode/skills/unity-spider-player-implementation/SKILL.md`
- `AGENTS.md`
- `docs/unity/runtime-architecture-guidelines.md`
- `docs/unity/spider-player-controller-plan.md`

## Related Docs

- `assistant-entrypoint.md`
- `retrieval-map.md`
- `../unity/project-structure.md`

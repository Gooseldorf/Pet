---
name: unity-spider-player-implementation
description: Implement or extend the player-controlled spider in this project. Use when the task is about spider traversal on floor, walls, or ceilings, arbitrary surface orientation, spider jump behavior, or web attach, pull, and swing systems tied to the staged roadmap in `docs/unity/spider-player-controller-plan.md`.
---

# Unity Spider Player Implementation

Implement the spider player controller by following the project roadmap instead of inventing a new movement architecture on each change.

## Goals

- follow the staged controller plan in `docs/unity/spider-player-controller-plan.md`
- preserve the component boundaries around controller root, surface, orientation, movement, jump, and web behavior
- keep locomotion core stable before adding camera-dependent polish or IK-driven presentation
- maintain explicit `single-player only` assumptions unless the user expands scope

## When To Use

Use this skill when asked to:

- add or refactor the spider player controller
- implement floor, wall, or ceiling traversal
- add arbitrary surface orientation or adhesion behavior
- add or change spider jump behavior
- add or change web `attach`, `pull`, or `swing`
- continue implementation from the staged spider controller roadmap

Do not use this as the main skill for unrelated UI flow, generic repo-wide refactors, or architecture-only review without implementation intent.

## Source Of Truth

- `docs/unity/spider-player-controller-plan.md`
- `docs/unity/runtime-architecture-guidelines.md`
- `AGENTS.md`
- `Assets/_Root/Scripts/Input/Player/IPlayerInputStreams.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- `Assets/_Root/Scripts/DI/GameplayScope.cs`
- `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`

## Workflow

1. Read `docs/unity/spider-player-controller-plan.md` and identify the current stage or milestone affected by the task.
2. Read the current authored files that own the relevant runtime boundary before changing code.
3. Keep the change inside the lightest owning component boundary that fits the roadmap.
4. Prefer extending the planned spider-specific components over scattering movement logic into unrelated `MonoBehaviour` classes.
5. If the task requires prefab, scene, or Inspector wiring, state exactly what must be done in the Unity Editor.
6. If the task reaches across milestones, stabilize the earlier milestone first instead of partially implementing multiple later systems.

## Stage Order

Default implementation order:

1. `SpiderPlayerController` skeleton and config
2. `SpiderSurfaceComponent`
3. `SpiderOrientationComponent`
4. `SpiderMovementComponent`
5. `SpiderJumpComponent`
6. `SpiderWebComponent` `attach`
7. `SpiderWebComponent` `pull`
8. `SpiderWebComponent` `swing`
9. camera integration
10. legs and IK presentation

Do not pull camera or IK concerns earlier unless the user explicitly changes the roadmap.

## Decision Rules

- use `Rigidbody` as the movement base and keep `useGravity = false` when implementing spider traversal
- model spider traversal as arbitrary-surface movement with authored adhesion, not as a standard ground controller plus a separate climb mode
- keep the root `SpiderPlayerController` as the scene-owned coordinator instead of a god-object full of movement math
- keep surface detection, orientation, movement, jump, and web logic in their owning spider components when that split is already part of the roadmap
- do not let web logic take ownership of baseline locomotion
- do not let IK or leg-placement logic own gameplay traversal rules
- treat the controller as `single-player only`; if a requested change would imply networking assumptions, state that multiplayer is not yet designed for this system
- prefer authored config values in `SpiderConfig` over embedded tuning constants when the values are part of controller behavior

## Project-Specific Cautions

- `Assets/_Root/Scripts/DI/GameplayScope.cs` currently registers gameplay UI but not a player controller stack; spider-player runtime wiring will need a deliberate home
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs` already provides the intended gameplay-facing input boundary; do not bypass it with direct `InputAction` usage unless there is a concrete reason
- `pf_Spider` and `pf_SpiderIkTargets` already exist, but procedural legs and IK should remain later-stage concerns until the movement core is stable
- camera rotation together with the spider is planned, but `Cinemachine` integration should not be used as a prerequisite for early traversal implementation

## Output Expectations

When finishing spider-player work:

1. state which roadmap stage or milestone the change advances
2. name the spider component boundaries that were changed
3. list required Unity Editor wiring, if any
4. state whether the change remains `single-player only`

## Related Files

- `Assets/_Root/Scripts/Input/Player/IPlayerInputStreams.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- `Assets/_Root/Scripts/DI/GameplayScope.cs`
- `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- `Assets/_Root/Prefabs/pf_Spider.prefab`
- `Assets/_Root/Prefabs/pf_SpiderIkTargets.prefab`

## Related Docs

- `docs/unity/spider-player-controller-plan.md`
- `docs/unity/runtime-architecture-guidelines.md`
- `docs/ai/coding-skills.md`
- `docs/project-map.md`

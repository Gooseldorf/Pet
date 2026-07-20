# Spider Player Controller Plan

## Purpose

This page is the source-of-truth implementation roadmap for the spider player controller.

## Scope

This plan covers the runtime controller architecture and staged implementation path for the player-controlled spider.

It does not define final camera tuning, prefab wiring details, or IK implementation specifics beyond where they fit in the roadmap.

## Source Of Truth

- `AGENTS.md`
- `docs/unity/runtime-architecture-guidelines.md`
- `Assets/_Root/Scripts/Input/Player/IPlayerInputStreams.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- `Assets/_Root/Scripts/DI/GameplayScope.cs`
- `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- `Assets/_Root/Prefabs/pf_Spider.prefab`
- `Assets/_Root/Prefabs/pf_SpiderIkTargets.prefab`

## Decisions Already Made

- the controller is `single-player only`
- the movement base should use `Rigidbody`, not `CharacterController`
- `Rigidbody.useGravity` should be disabled and replaced with authored surface adhesion and local-up movement rules
- the spider can always traverse floor, walls, and ceilings
- the jump should push away from the current surface with input bias
- the web feature set includes `attach`, `pull`, and `swing`
- camera integration comes later and should rotate with the spider through `Cinemachine`
- procedural legs and IK should follow the stable movement core instead of driving it
- initialization order should be explicit through spawn and bootstrap flow rather than `Start` or `OnEnable`
- the spider should be instantiated from `SpiderConfig.Prefab` through a small spawner instead of scene-owned controller references
- required authored wiring and DI dependencies should fail loudly instead of being wrapped in defensive null checks or custom exception guards

## Current Implementation Status

Completed so far:

- `Stage 1: Controller Skeleton`
- `Stage 2: Surface Detection`
- `Stage 4: Baseline Surface Movement`
- first-pass camera spawn and bind integration

Current authored runtime shape:

- `GameplayEntryPoint` is initialized explicitly by `SceneLoader` after `Gameplay` becomes the active scene, then spawns the spider through `SpiderPlayerSpawner`
- `GameplayEntryPoint` now also spawns a gameplay camera rig through `CameraSpawner` and binds it to the spawned spider targets
- `GameplayEntryPoint` also resolves the scene `Main Camera` from `GameplayScope` and assigns its transform to the spawned spider as the locomotion movement-reference frame
- `SpiderConfig` owns the spider prefab reference and probe tuning values
- `CameraConfig` owns the gameplay camera rig prefab reference
- `SpiderPlayerController` is the runtime root for input, fixed-update ordering, and current surface state
- `SpiderPlayerController` also exposes authored `cameraFollowTarget` and `cameraLookTarget` references for runtime camera binding
- `SpiderPlayerController` now stores an explicit movement-reference transform used by locomotion rules
- `SpiderSurfaceComponent` performs five surface probes per physics tick using center, forward, backward, left, and right offsets
- surface detection falls back from `SphereCast` to overlap sampling so already-touching surfaces can still be detected
- `SpiderMovementComponent` now maps move input through the scene `Main Camera` frame, then projects that frame onto the current traversal plane for floor, wall, and ceiling movement
- `GameplayTester` can log the current spider surface state on `Keypad1`

Current authored files:

- `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- `Assets/_Root/Scripts/DI/GameplayScope.cs`
- `Assets/_Root/Scripts/Gameplay/Camera/CameraConfig.cs`
- `Assets/_Root/Scripts/Gameplay/Camera/CameraRig.cs`
- `Assets/_Root/Scripts/Gameplay/Camera/CameraSpawner.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderConfig.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderPlayerController.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderPlayerSpawner.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/PlayerSpawnPoint.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceComponent.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceState.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceHit.cs`
- `Assets/_Root/Scripts/Test/GameplayTester.cs`

## Target Architecture

### `SpiderPlayerController`

Scene-owned `MonoBehaviour`.

Responsibilities:

- hold serialized references such as `Rigidbody`, collider data, probe origins, and visual roots
- read current input through `IPlayerInputStreams`
- own update ordering across the controller components
- apply final velocity, forces, and rotation to Unity components

### `SpiderSurfaceComponent`

Responsibilities:

- detect floor, wall, and ceiling support
- sample surface probes with raycasts or spherecasts
- produce stable `surfaceNormal`, `surfacePoint`, and support-state output
- handle outer-corner and inner-corner transition cases

### `SpiderOrientationComponent`

Responsibilities:

- align the spider body to the current surface
- smooth orientation changes across changing normals
- maintain the controller's authored local-up frame

### `SpiderMovementComponent`

Responsibilities:

- convert input into motion along the current surface plane
- separate grounded and airborne control rules
- manage speed, acceleration, and directional projection relative to camera space and local up

### `SpiderJumpComponent`

Responsibilities:

- decide when jumping is allowed
- calculate jump impulse away from the current surface
- bias jump direction toward current movement input
- provide a short anti-stick window after jumping

### `SpiderWebComponent`

Responsibilities:

- find valid web targets
- store and clear anchor state
- manage `attach`, `pull`, `swing`, and `detach`
- blend web behavior with the baseline movement controller without taking over unrelated systems

### `SpiderConfig`

Authored `ScriptableObject` config.

Responsibilities:

- hold tunable values for probing, adhesion, movement, jump, and web behavior
- hold feature toggles for staged rollout and testing
- keep tuning out of scene objects where the values are shared controller behavior

### Current Camera Integration

Current implementation boundary:

- camera runtime ownership lives in `Assets/_Root/Scripts/Gameplay/Camera/`
- `CameraSpawner` instantiates a gameplay camera rig prefab from `CameraConfig`
- `CameraRig` binds a spawned `CinemachineCamera` to `SpiderPlayerController.CameraFollowTarget` and `SpiderPlayerController.CameraLookTarget`
- camera input is expected to be authored on the camera prefab with `CinemachineInputAxisController`, not driven through `IPlayerInputStreams`
- the gameplay scene is expected to keep a single Unity `Main Camera` with `CinemachineBrain`
- baseline spider locomotion currently uses the scene `Main Camera` transform as its camera-relative movement frame

### Later Components

These should come after the movement core is stable:

- `SpiderLegsComponent`
- `SpiderIkComponent`

## Runtime Update Shape

Preferred controller flow:

1. Read current input state and one-shot events.
2. Update `SpiderSurfaceComponent`.
3. Update `SpiderOrientationComponent`.
4. Calculate baseline locomotion through `SpiderMovementComponent`.
5. Apply jump decisions through `SpiderJumpComponent` when requested.
6. Overlay web behavior through `SpiderWebComponent` when active.
7. Apply final results to `Rigidbody` and authored transforms.

## Implementation Principles

- prefer one scene-owned controller root plus focused plain C# rule components
- avoid generic manager or service layers for this controller
- keep visual IK and procedural leg logic out of the locomotion core
- keep web traversal additive to the movement core instead of replacing it
- prefer `ScriptableObject` config over static tuning values
- prefer typed serialized wiring over runtime hierarchy search
- treat multiplayer as out of scope until authority and sync are explicitly designed

## Feature Toggles To Reserve In Config

- `enableCeilingTraversal`
- `enableSurfaceAdhesion`
- `enableJump`
- `enableWeb`
- `enableWebPull`
- `enableWebSwing`
- `enableAirControl`

## Staged Implementation Plan

### Stage 1: Controller Skeleton

Goal:

- create the stable root structure for the spider controller

Implement:

- `SpiderPlayerController`
- `SpiderConfig`
- `IPlayerInputStreams` integration
- serialized controller references for `Rigidbody`, probes, and visual roots
- initial runtime update ordering

Done when:

- the spider object is owned by the new controller root
- input is available through the controller
- config data is injected or serialized in an authored way
- later behavior can be added without changing the root ownership model

Current result:

- done through `SpiderPlayerSpawner`, `SpiderConfig.Prefab`, and explicit `SpiderPlayerController.Initialize()` startup flow

### Stage 2: Surface Detection

Goal:

- reliably identify the surface the spider should adhere to

Implement:

- `SpiderSurfaceComponent`
- multi-probe raycast or spherecast sampling
- support-state outputs such as `hasSurface`, `isStableSurface`, `surfaceNormal`, and `surfacePoint`
- surface-normal aggregation rules
- initial edge and corner handling

Done when:

- floor, wall, and ceiling surfaces are detected consistently
- the controller keeps useful support data through simple transitions and corners

Current result:

- done at the first authored slice through five sphere-based probes and overlap fallback support for already-touching surfaces

### Stage 3: Orientation And Adhesion

Goal:

- make the spider align to and stay attached to surfaces

Implement:

- `SpiderOrientationComponent`
- smooth body alignment to the detected normal
- authored adhesion or fake-gravity force toward the surface
- loss-of-surface fallback behavior
- smoothing against abrupt normal jitter

Done when:

- the spider stays attached to valid surfaces without obvious jitter
- transitions between floor, wall, and ceiling are stable on simple geometry

### Stage 4: Baseline Surface Movement

Goal:

- support controlled movement on any traversable surface

Implement:

- `SpiderMovementComponent`
- movement projected onto the current surface plane
- grounded speed and acceleration
- separate airborne control rules
- movement relative to camera-facing input with arbitrary local up

Done when:

- the spider can move cleanly across floor, walls, and ceilings
- controls remain readable when the spider inverts onto the ceiling

Current result:

- done through `SpiderMovementComponent` using a camera-relative movement frame supplied by `GameplayEntryPoint` from the scene `Main Camera`, with projection onto the current surface plane and fallback handling when camera forward approaches the surface normal

### Stage 5: Jump

Goal:

- let the spider intentionally leave the current surface

Implement:

- `SpiderJumpComponent`
- jump impulse away from the current surface
- input-biased jump direction
- anti-stick and reattach-delay behavior
- basic jump gating rules

Done when:

- jumps from walls and ceilings behave predictably
- the spider does not immediately reattach on the same tick after a jump

### Stage 6: Web Attach

Goal:

- add anchor acquisition and web state ownership

Implement:

- base `SpiderWebComponent`
- valid target search and filtering
- anchor storage
- `attach` and `detach` flow
- controller state transitions for inactive, attached, pulling, and swinging

Done when:

- the spider can reliably attach to allowed targets
- detaching clears anchor state cleanly

### Stage 7: Web Pull

Goal:

- allow traversal by pulling the spider toward the anchor

Implement:

- pull behavior in `SpiderWebComponent`
- rope-length control
- pull forces or velocity adjustments
- compatibility with movement and orientation rules
- transition rules into and out of pull

Done when:

- pull movement feels controllable
- approaching surfaces during pull does not break orientation or support state

### Stage 8: Web Swing

Goal:

- add controllable swinging around the web anchor

Implement:

- swing state in `SpiderWebComponent`
- rope-length behavior during swing
- tangent acceleration from input
- release behavior
- transitions between pull, swing, detach, and surface reattach

Implementation note:

- it is acceptable to validate the first swing slice with `SpringJoint`
- if the result is too elastic or hard to control, replace the swing core with a custom tether constraint while keeping the same component boundary

Done when:

- swinging is controllable
- web release produces a predictable launch
- swing behavior does not feel excessively rubbery

### Stage 9: Camera Integration

Goal:

- make traversal readable once the movement core is stable

Current baseline:

- camera rig spawn and player-target binding are now implemented through `CameraConfig`, `CameraSpawner`, and `CameraRig`
- baseline camera-relative locomotion already uses the scene `Main Camera` transform as the movement reference for `SpiderMovementComponent`
- final traversal-oriented tuning is still pending

Implement later:

- camera rotation that follows the spider's orientation
- traversal-oriented `Cinemachine` tuning on walls and ceilings
- comfort tuning for floor, wall, and ceiling transitions

Done when:

- the camera supports traversal instead of fighting it
- orientation changes remain readable for the player

### Stage 10: Procedural Legs And IK

Goal:

- add presentation and spider-specific body behavior on top of the working controller

Implement later:

- `SpiderLegsComponent`
- foothold search
- step timing and scheduling
- body height and offset response
- IK solving integration

Done when:

- legs visually follow the stable controller state
- presentation does not own or destabilize the locomotion core

## Planned Milestones

### Milestone 1

- `SpiderPlayerController`
- `SpiderSurfaceComponent`
- `SpiderOrientationComponent`
- `SpiderMovementComponent`

Definition:

- the spider moves stably across floor, walls, and ceilings without jump or web behavior

### Milestone 2

- `SpiderJumpComponent`

Definition:

- the spider can jump from any traversable surface without immediate reattachment glitches

### Milestone 3

- `SpiderWebComponent` with `attach` and `pull`

Definition:

- the spider can anchor web targets and use them for controllable pull traversal

### Milestone 4

- full `swing` support

Definition:

- the spider can enter, control, and release web swings cleanly

## Explicit Boundaries

- `SpiderWebComponent` should not own baseline locomotion
- `SpiderJumpComponent` should not know about IK or procedural leg placement
- `SpiderSurfaceComponent` should not own camera behavior
- `SpiderOrientationComponent` should not make input decisions
- `SpiderPlayerController` should coordinate components instead of containing all controller math directly

## Out Of Scope For Early Stages

- multiplayer authority and synchronization
- final `Cinemachine` tuning
- procedural leg animation and IK correctness
- advanced authored scene or prefab polish beyond required controller wiring

## Related Files

- `Assets/_Root/Scripts/Input/Player/IPlayerInputStreams.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- `Assets/_Root/Scripts/DI/GameplayScope.cs`
- `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- `Assets/_Root/Prefabs/pf_Spider.prefab`
- `Assets/_Root/Prefabs/pf_SpiderIkTargets.prefab`

## Related Docs

- `../project-map.md`
- `runtime-architecture-guidelines.md`
- `project-structure.md`
- `../ai/coding-skills.md`
- `../history/milestones.md`

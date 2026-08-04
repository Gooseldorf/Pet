# Spider Player Controller Plan

## Purpose

This page is the source-of-truth status note for the player-controlled spider while its locomotion stack is being rewritten.

## Scope

This page describes what remains implemented today, what was intentionally removed, and which boundaries must stay stable during the rewrite.

It does not define the new locomotion architecture yet.

## Source Of Truth

- `AGENTS.md`
- `docs/unity/runtime-architecture-guidelines.md`
- `Assets/_Root/Scripts/DI/GameplayScope.cs`
- `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- `Assets/_Root/Scripts/Gameplay/Camera/CameraConfig.cs`
- `Assets/_Root/Scripts/Gameplay/Camera/CameraRig.cs`
- `Assets/_Root/Scripts/Gameplay/Camera/CameraSpawner.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderConfig.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderPlayerController.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderPlayerSpawner.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/PlayerSpawnPoint.cs`
- `Assets/_Root/Prefabs/pf_Spider.prefab`

## Current Implementation Status

Current state:

- previous spider movement, surface-detection, orientation, adhesion, and related debug code has been intentionally removed
- spider runtime ownership still starts from `GameplayEntryPoint`, which spawns the spider through `SpiderPlayerSpawner`
- gameplay camera ownership still starts from `GameplayEntryPoint`, which spawns a rig through `CameraSpawner` and binds it through `CameraRig`
- `SpiderConfig` now only owns the spider prefab reference used by the spawner
- `SpiderPlayerController` is currently a minimal runtime root that only exposes authored `cameraFollowTarget` and `cameraLookTarget` references
- `CameraRig` still assigns `CinemachineBrain.WorldUpOverride` to the spawned spider root and binds follow/look targets from `SpiderPlayerController`

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

Removed authored files:

- `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceComponent.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceState.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceHit.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderLookRotationComponent.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderMovementComponent.cs`
- `Assets/_Root/Scripts/Gameplay/Spider/SpiderMovementResult.cs`
- `Assets/_Root/Scripts/Test/GameplayTester.cs`

## Preserved Runtime Boundary

### `GameplayEntryPoint`

Responsibilities:

- spawn the spider through `SpiderPlayerSpawner`
- spawn the gameplay camera through `CameraSpawner`
- bind the camera rig to the spawned spider
- keep existing gameplay UI startup intact

### `SpiderConfig`

Responsibilities:

- hold `SpiderPlayerController Prefab`

### `SpiderPlayerController`

Responsibilities:

- act as the spawned spider root used by camera binding
- expose `CameraFollowTarget`
- expose `CameraLookTarget`

### `CameraRig`

Responsibilities:

- bind the spawned Cinemachine camera to `SpiderPlayerController.CameraFollowTarget`
- bind the spawned Cinemachine camera to `SpiderPlayerController.CameraLookTarget`
- assign `CinemachineBrain.WorldUpOverride` to the spawned spider root

## Rewrite Constraints

- keep explicit spawn flow through `SpiderPlayerSpawner`; do not move spider ownership back into the scene
- keep runtime camera ownership in `Assets/_Root/Scripts/Gameplay/Camera/`
- keep camera follow/look targets authored on the spider prefab
- do not reintroduce scene search or scene-owned target references as a shortcut for camera binding
- treat multiplayer as out of scope until authority and sync are explicitly designed

## Next Implementation Note

When locomotion work restarts, define the new movement architecture from scratch against the preserved spawn and camera boundary above instead of reviving the removed component set by default.

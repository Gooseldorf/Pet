# Unity Project Structure

## Purpose

This page documents the current Unity-authored structure of the repository.

## Scope

This is a project-layout document, not a full gameplay architecture document.

## Source Of Truth

- `Assets/_Root/`
- `Assets/Settings/`
- `Packages/manifest.json`
- `ProjectSettings/`

## Current Implementation

### Main Unity Directories

| Path | Purpose |
| --- | --- |
| `Assets/_Root/` | Main authored project content |
| `Assets/Settings/` | Shared project settings assets |
| `Packages/` | Unity package dependencies |
| `ProjectSettings/` | Unity editor and project configuration |

### Authored Content Under `Assets/_Root/`

| Path | Purpose |
| --- | --- |
| `Assets/_Root/Scenes/` | Scenes |
| `Assets/_Root/Prefabs/` | Prefabs |
| `Assets/_Root/Scripts/` | C# scripts |
| `Assets/_Root/Settings/` | Authored Unity settings assets |

## Current Known Assets

### Scenes

- `Assets/_Root/Scenes/Bootstrap.unity`
- `Assets/_Root/Scenes/MainMenu.unity`

### Prefabs

- `Assets/_Root/Prefabs/pf_MainMenuButton.prefab`

### Scripts

- `Assets/_Root/Scripts/Architecture/Bootstrap/Bootstrap.cs`
- `Assets/_Root/Scripts/Architecture/Bootstrap/GlobalScope.cs`
- `Assets/_Root/Scripts/Architecture/SceneLoading/SceneLoader.cs`
- `Assets/_Root/Scripts/Test/MainMenuTester.cs`

### Settings Assets

- `Assets/Settings/Build Profiles/` currently exists but has no committed build profile assets.

## Current Script Notes

Current authored scripts now include a bootstrap/composition-root slice and a separate test-style script.

Observed characteristics:

- `Assets/_Root/Scripts/Architecture/Bootstrap/GlobalScope.cs` is a VContainer `LifetimeScope`
- `Assets/_Root/Scripts/Architecture/Bootstrap/Bootstrap.cs` is an `IAsyncStartable` entry point
- `Assets/_Root/Scripts/Architecture/SceneLoading/SceneLoader.cs` wraps additive scene loading through `SceneManager`
- `Assets/_Root/Scripts/Test/MainMenuTester.cs` remains in the `Test` namespace and is not the runtime architecture entry point

## Startup Flow

- `ProjectSettings/EditorBuildSettings.asset` currently enables `Bootstrap.unity` first and `MainMenu.unity` second
- `Bootstrap.unity` is the startup scene
- a scene object in `Bootstrap.unity` hosts `GlobalScope`
- `GlobalScope` registers `SceneLoader` and the `Bootstrap` entry point through VContainer
- `Bootstrap` loads `MainMenu` additively

## Package Notes

`Packages/manifest.json` currently includes these notable packages:

- `com.cysharp.unitask`
- `jp.hadashikick.vcontainer`
- `com.unity.addressables`
- `com.unity.ai.navigation`
- `com.unity.cinemachine`
- `com.unity.inputsystem`
- `com.unity.render-pipelines.universal`
- `com.unity.test-framework`
- `com.unity.timeline`
- `com.unity.ugui`

This indicates the project already has package-level support for async gameplay flows, DI, asset delivery, navigation, camera/cinematic workflows, UI, input, URP, and testing, even though authored gameplay systems are still minimal.

## Conventions

- authored content is grouped under `Assets/_Root/`
- prefab names currently use the `pf_` prefix
- required object references should generally be wired through serialized fields
- project-specific Unity coding constraints are defined in `AGENTS.md`

## Related Files

- `Assets/_Root/Scripts/Architecture/Bootstrap/Bootstrap.cs`
- `Assets/_Root/Scripts/Architecture/Bootstrap/GlobalScope.cs`
- `Assets/_Root/Scripts/Architecture/SceneLoading/SceneLoader.cs`
- `Assets/_Root/Scripts/Test/MainMenuTester.cs`
- `Assets/_Root/Scenes/MainMenu.unity`
- `Assets/_Root/Scenes/Bootstrap.unity`
- `Assets/_Root/Prefabs/pf_MainMenuButton.prefab`
- `Packages/manifest.json`
- `ProjectSettings/EditorBuildSettings.asset`
- `ProjectSettings/ProjectVersion.txt`

## Related Docs

- `../project-map.md`
- `../systems/ci-cd.md`

## Open Questions

- There is not yet an established scene-local architecture for authored gameplay/UI code beyond the bootstrap and scene-loading slice.
- There are not yet formal documented scene/prefab ownership conventions beyond the currently observed naming pattern.

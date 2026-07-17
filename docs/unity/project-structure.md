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

- `Assets/_Root/Scripts/Bootstrap/Bootstrap.cs`
- `Assets/_Root/Scripts/Bootstrap/MainMenuEntryPoint.cs`
- `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- `Assets/_Root/Scripts/DI/GlobalScope.cs`
- `Assets/_Root/Scripts/DI/MainMenuScope.cs`
- `Assets/_Root/Scripts/DI/GameplayScope.cs`
- `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs`
- `Assets/_Root/Scripts/Configs/ProjectConfig.cs`
- `Assets/_Root/Scripts/Configs/UI/UIConfig.cs`
- `Assets/_Root/Scripts/Configs/UI/LoadingOverlayConfig.cs`
- `Assets/_Root/Scripts/Input/InputActionsProvider.cs`
- `Assets/_Root/Scripts/Input/InputActionObservableExtensions.cs`
- `Assets/_Root/Scripts/Input/Player/IPlayerInputStreams.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputState.cs`
- `Assets/_Root/Scripts/Input/InputSystem_Actions.cs`
- `Assets/_Root/Scripts/UI/UiRoot.cs`
- `Assets/_Root/Scripts/UI/LoadingOverlay.cs`
- `Assets/_Root/Scripts/UI/LoadingOverlayController.cs`
- `Assets/_Root/Scripts/Editor/BootstrapPlayModeRedirect.cs`
- `Assets/_Root/Scripts/Test/MainMenuTester.cs`
- `Assets/_Root/Scripts/Pet.Runtime.asmdef`
- `Assets/_Root/Scripts/Editor/Pet.Editor.asmdef`

### Settings Assets

- `Assets/Settings/Build Profiles/` currently exists but has no committed build profile assets.

## Current Script Notes

Current authored scripts now include separate bootstrap, DI, scene-loading, config, input, UI, editor, and test-style slices.

Observed characteristics:

- `Assets/_Root/Scripts/DI/GlobalScope.cs` is a VContainer `LifetimeScope`
- `Assets/_Root/Scripts/DI/MainMenuScope.cs` and `Assets/_Root/Scripts/DI/GameplayScope.cs` are scene-level VContainer scopes
- `Assets/_Root/Scripts/Bootstrap/Bootstrap.cs` is an `IAsyncStartable` entry point
- `Assets/_Root/Scripts/Bootstrap/MainMenuEntryPoint.cs` and `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs` switch active input maps for their scenes
- `Assets/_Root/Scripts/Configs/ProjectConfig.cs` is the current local `ScriptableObject` config root
- `Assets/_Root/Scripts/Configs/UI/UIConfig.cs` and `Assets/_Root/Scripts/Configs/UI/LoadingOverlayConfig.cs` are separate config asset types under the same local config layer
- `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs` wraps additive scene loading through `SceneManager`
- `Assets/_Root/Scripts/Input/` contains the Unity Input System integration and reactive player input streams
- `Assets/_Root/Scripts/UI/UiRoot.cs` exposes scene-authored UI references
- `Assets/_Root/Scripts/UI/LoadingOverlay.cs` is the authored loading overlay view driven by injected config
- `Assets/_Root/Scripts/UI/LoadingOverlayController.cs` is the plain C# wrapper used to drive the overlay
- `Assets/_Root/Scripts/Editor/BootstrapPlayModeRedirect.cs` is editor-only play mode startup glue
- `Assets/_Root/Scripts/Test/MainMenuTester.cs` remains a test-style runtime script and is not the runtime architecture entry point

Current authored namespace baseline:

- root namespaces stay short and use `Pet` or `Pet.X`
- current authored namespaces include `Pet`, `Pet.UI`, `Pet.Input`, `Pet.Configs`, `Pet.MainMenu`, `Pet.Gameplay`, and `Pet.Editor`

Current authored assembly baseline:

- `Assets/_Root/Scripts/Pet.Runtime.asmdef`
- `Assets/_Root/Scripts/Editor/Pet.Editor.asmdef`

## Startup Flow

- `ProjectSettings/EditorBuildSettings.asset` currently enables `Bootstrap.unity` first and `MainMenu.unity` second
- `Bootstrap.unity` is the startup scene
- a scene object in `Bootstrap.unity` hosts `GlobalScope`
- `GlobalScope` serializes and registers `ProjectConfig`
- `GlobalScope` registers `SceneLoader` and the `Bootstrap` entry point through VContainer
- `Bootstrap` loads `MainMenu` additively

## Local Config Layer

- local config root: `Assets/_Root/Scripts/Configs/ProjectConfig.cs`
- config folder: `Assets/_Root/Scripts/Configs/`
- current config asset files:
  - `Assets/_Root/Scripts/Configs/UI/UIConfig.cs`
  - `Assets/_Root/Scripts/Configs/UI/LoadingOverlayConfig.cs`
- registration site: `Assets/_Root/Scripts/DI/GlobalScope.cs`
- current usage: `Assets/_Root/Scripts/UI/LoadingOverlay.cs` reads `ProjectConfig.UI.LoadingOverlay.FadeDuration`

## Package Notes

`Packages/manifest.json` currently includes these notable packages:

- `com.cysharp.r3`
- `com.cysharp.unitask`
- `com.github-glitchenzo.nugetforunity`
- `jp.hadashikick.vcontainer`
- `com.unity.addressables`
- `com.unity.ai.navigation`
- `com.unity.cinemachine`
- `com.unity.inputsystem`
- `com.unity.render-pipelines.universal`
- `com.unity.test-framework`
- `com.unity.timeline`
- `com.unity.ugui`

Current package-source notes:

- `Packages/manifest.json` defines the `package.openupm.com` scoped registry for `jp.hadashikick.vcontainer`
- `com.cysharp.r3`, `com.cysharp.unitask`, and `com.github-glitchenzo.nugetforunity` are Git-based package dependencies

## Conventions

- authored content is grouped under `Assets/_Root/`
- prefab names currently use the `pf_` prefix
- required object references should generally be wired through serialized fields
- project-specific Unity coding constraints are defined in `AGENTS.md`

## Related Files

- `AGENTS.md`
- `Assets/_Root/Scripts/Bootstrap/Bootstrap.cs`
- `Assets/_Root/Scripts/Bootstrap/MainMenuEntryPoint.cs`
- `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- `Assets/_Root/Scripts/DI/GlobalScope.cs`
- `Assets/_Root/Scripts/DI/MainMenuScope.cs`
- `Assets/_Root/Scripts/DI/GameplayScope.cs`
- `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs`
- `Assets/_Root/Scripts/Configs/ProjectConfig.cs`
- `Assets/_Root/Scripts/Configs/UI/UIConfig.cs`
- `Assets/_Root/Scripts/Configs/UI/LoadingOverlayConfig.cs`
- `Assets/_Root/Scripts/Input/InputActionsProvider.cs`
- `Assets/_Root/Scripts/Input/Player/IPlayerInputStreams.cs`
- `Assets/_Root/Scripts/UI/UiRoot.cs`
- `Assets/_Root/Scripts/UI/LoadingOverlay.cs`
- `Assets/_Root/Scripts/UI/LoadingOverlayController.cs`
- `Assets/_Root/Scripts/Editor/BootstrapPlayModeRedirect.cs`
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

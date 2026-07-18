# Unity Project Structure

## Purpose

This page documents the current Unity-authored structure of the repository.

## Scope

This is a project-layout document, not a full gameplay architecture document.

## Source Of Truth

- `Assets/_Root/`
- `Packages/manifest.json`
- `ProjectSettings/`

## Current Implementation

### Main Unity Directories

| Path | Purpose |
| --- | --- |
| `Assets/_Root/` | Main authored project content |
| `Assets/Packages/` | Tool-imported NuGet package contents under `Assets/` |
| `Assets/Plugins/` | Third-party Unity plugin binaries |
| `Assets/Resources/` | Shared Unity resources |
| `Packages/` | Unity package manifest and lock file |
| `ProjectSettings/` | Unity editor and project configuration |

### Authored Content Under `Assets/_Root/`

| Path | Purpose |
| --- | --- |
| `Assets/_Root/Scenes/` | Scenes |
| `Assets/_Root/Prefabs/` | Prefabs |
| `Assets/_Root/Configs/` | `ScriptableObject` asset instances |
| `Assets/_Root/Scripts/` | C# scripts and asmdefs |
| `Assets/_Root/Settings/` | Render pipeline and volume settings assets |
| `Assets/_Root/Models/` | Models |
| `Assets/_Root/Materials/` | Materials |
| `Assets/_Root/Animations/` | Animations; currently empty |

## Current Known Assets

### Scenes

- `Assets/_Root/Scenes/Bootstrap.unity`
- `Assets/_Root/Scenes/MainMenu.unity`
- `Assets/_Root/Scenes/Gameplay.unity`

### Prefabs

- `Assets/_Root/Prefabs/pf_Spider.prefab`
- `Assets/_Root/Prefabs/pf_SpiderIkTargets.prefab`
- `Assets/_Root/Prefabs/UI/pf_UI_MainMenuScreenView.prefab`
- `Assets/_Root/Prefabs/UI/pf_UI_PauseMenuPopUpView.prefab`

### Config Assets

- `Assets/_Root/Configs/ProjectConfig.asset`
- `Assets/_Root/Configs/UI/_UIConfig.asset`
- `Assets/_Root/Configs/UI/LoadingOverlayConfig.asset`
- `Assets/_Root/Configs/UI/MainMenuConfig.asset`
- `Assets/_Root/Configs/UI/PauseMenuConfig.asset`

### Scripts

- `Assets/_Root/Scripts/Bootstrap/Bootstrap.cs`
- `Assets/_Root/Scripts/Bootstrap/MainMenuEntryPoint.cs`
- `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- `Assets/_Root/Scripts/DI/GlobalScope.cs`
- `Assets/_Root/Scripts/DI/MainMenuScope.cs`
- `Assets/_Root/Scripts/DI/GameplayScope.cs`
- `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs`
- `Assets/_Root/Scripts/Configs/ProjectConfig.cs`
- `Assets/_Root/Scripts/Input/InputActionsProvider.cs`
- `Assets/_Root/Scripts/Input/InputActionObservableExtensions.cs`
- `Assets/_Root/Scripts/Input/InputMapKind.cs`
- `Assets/_Root/Scripts/Input/InputSystem_Actions.cs`
- `Assets/_Root/Scripts/Input/Player/IPlayerInputStreams.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputState.cs`
- `Assets/_Root/Scripts/UI/UIRoot.cs`
- `Assets/_Root/Scripts/UI/UIConfig.cs`
- `Assets/_Root/Scripts/UI/UIInstanceFactory.cs`
- `Assets/_Root/Scripts/UI/UIScreenNavigator.cs`
- `Assets/_Root/Scripts/UI/UIPopupCoordinator.cs`
- `Assets/_Root/Scripts/UI/UIBackRouter.cs`
- `Assets/_Root/Scripts/UI/UIBackInputListener.cs`
- `Assets/_Root/Scripts/UI/IBackHandler.cs`
- `Assets/_Root/Scripts/UI/Base/UIViewBase.cs`
- `Assets/_Root/Scripts/UI/Base/UIScreenViewBase.cs`
- `Assets/_Root/Scripts/UI/Base/UIScreenConfigBase.cs`
- `Assets/_Root/Scripts/UI/Base/UIPopupViewBase.cs`
- `Assets/_Root/Scripts/UI/Base/UIPopupConfigBase.cs`
- `Assets/_Root/Scripts/UI/Enums/UILayerEnum.cs`
- `Assets/_Root/Scripts/UI/Enums/UICacheModeEnum.cs`
- `Assets/_Root/Scripts/UI/Enums/UIHistoryModeEnum.cs`
- `Assets/_Root/Scripts/UI/Enums/UiPopupQueueModeEnum.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlay.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayController.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayConfig.cs`
- `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuController.cs`
- `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuScreenView.cs`
- `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuConfig.cs`
- `Assets/_Root/Scripts/UI/Gameplay/UIGameplayController.cs`
- `Assets/_Root/Scripts/UI/Gameplay/Hud/UIHudController.cs`
- `Assets/_Root/Scripts/UI/Gameplay/Hud/UIHudView.cs`
- `Assets/_Root/Scripts/UI/Gameplay/Hud/UIHudConfig.cs`
- `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPauseMenuController.cs`
- `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPausePopupView.cs`
- `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPauseMenuConfig.cs`
- `Assets/_Root/Scripts/Utilities/InstantiateUtilities.cs`
- `Assets/_Root/Scripts/Utilities/TweenUtilities.cs`
- `Assets/_Root/Scripts/Editor/BootstrapPlayModeRedirect.cs`
- `Assets/_Root/Scripts/Test/MainMenuTester.cs`
- `Assets/_Root/Scripts/Pet.Runtime.asmdef`
- `Assets/_Root/Scripts/Editor/Pet.Editor.asmdef`

### Settings Assets

- `Assets/_Root/Settings/UniversalRenderPipelineGlobalSettings.asset`
- `Assets/_Root/Settings/PC_RPAsset.asset`
- `Assets/_Root/Settings/PC_Renderer.asset`
- `Assets/_Root/Settings/Mobile_RPAsset.asset`
- `Assets/_Root/Settings/Mobile_Renderer.asset`
- `Assets/_Root/Settings/MainProfile.asset`
- `Assets/_Root/Settings/DefaultVolumeProfile.asset`

### Models And Materials

- `Assets/_Root/Models/SpiderModel.fbx`
- `Assets/_Root/Materials/mat_TestEmmiting_Red.mat`

## Current Script Notes

Current authored scripts include separate bootstrap, DI, scene-loading, config, input, UI, editor, and test-style slices.

Observed characteristics:

- `Assets/_Root/Scripts/DI/GlobalScope.cs` is the global VContainer `LifetimeScope`
- `Assets/_Root/Scripts/DI/MainMenuScope.cs` and `Assets/_Root/Scripts/DI/GameplayScope.cs` are scene-level VContainer scopes
- `Assets/_Root/Scripts/Bootstrap/Bootstrap.cs` is an `IAsyncStartable` entry point
- `Assets/_Root/Scripts/Bootstrap/MainMenuEntryPoint.cs` and `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs` switch active input maps for their scenes
- `Assets/_Root/Scripts/Configs/ProjectConfig.cs` is the local config root type
- `Assets/_Root/Scripts/UI/UIConfig.cs` and `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayConfig.cs` define shared UI config branches used by `ProjectConfig`
- `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuConfig.cs`, `Assets/_Root/Scripts/UI/Gameplay/Hud/UIHudConfig.cs`, and `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPauseMenuConfig.cs` are scene-specific UI config types
- `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs` wraps additive scene loading and content-scene switching through `SceneManager`
- `Assets/_Root/Scripts/Input/` contains the Unity Input System integration and reactive player input streams
- `Assets/_Root/Scripts/UI/UIRoot.cs` exposes shared UI layer transforms plus the loading overlay reference
- `Assets/_Root/Scripts/UI/Base/` contains reusable view and config base types for screens and popups
- `Assets/_Root/Scripts/UI/LoadingScreen/` contains the shared loading overlay implementation
- `Assets/_Root/Scripts/UI/MainMenu/` contains main-menu screen flow
- `Assets/_Root/Scripts/UI/Gameplay/Hud/` and `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/` split gameplay HUD and pause flow
- `Assets/_Root/Scripts/Utilities/InstantiateUtilities.cs` grows or shrinks prefab-backed `MonoBehaviour` item lists to match data counts and toggles item visibility
- `Assets/_Root/Scripts/Utilities/TweenUtilities.cs` adds DOTween helpers for Z-axis rotation around a pivot, scale in-out animation, and awaiting tweener completion through `UniTask`
- `Assets/_Root/Scripts/Editor/BootstrapPlayModeRedirect.cs` is editor-only play mode startup glue
- `Assets/_Root/Scripts/Test/MainMenuTester.cs` remains a test-style runtime script and is not the runtime architecture entry point

Current authored namespace baseline:

- root namespaces stay short and use `Pet` or `Pet.X`
- current authored namespaces include `Pet`, `Pet.UI`, `Pet.Input`, `Pet.Configs`, `Pet.MainMenu`, `Pet.Gameplay`, and `Pet.Editor`

Current authored assembly baseline:

- `Assets/_Root/Scripts/Pet.Runtime.asmdef`
- `Assets/_Root/Scripts/Editor/Pet.Editor.asmdef`

## Startup Flow

- `ProjectSettings/EditorBuildSettings.asset` currently enables `Bootstrap.unity`, `MainMenu.unity`, and `Gameplay.unity`
- `Bootstrap.unity` is the startup scene
- a scene object in `Bootstrap.unity` hosts `GlobalScope`
- `GlobalScope` serializes and registers `ProjectConfig` and `UIRoot`
- `GlobalScope` registers `SceneLoader` and the `Bootstrap` entry point through VContainer
- `Bootstrap` loads `MainMenu` additively

## Local Config Layer

- root config type: `Assets/_Root/Scripts/Configs/ProjectConfig.cs`
- shared UI config types: `Assets/_Root/Scripts/UI/UIConfig.cs`, `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayConfig.cs`
- scene UI config types: `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuConfig.cs`, `Assets/_Root/Scripts/UI/Gameplay/Hud/UIHudConfig.cs`, `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPauseMenuConfig.cs`
- asset folder: `Assets/_Root/Configs/`
- root config asset: `Assets/_Root/Configs/ProjectConfig.asset`
- registration sites: `Assets/_Root/Scripts/DI/GlobalScope.cs`, `Assets/_Root/Scripts/DI/MainMenuScope.cs`, `Assets/_Root/Scripts/DI/GameplayScope.cs`
- current shared usage: `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlay.cs` reads `ProjectConfig.UI.LoadingOverlay.FadeDuration`

## Package Notes

`Packages/manifest.json` currently includes these notable packages:

- `com.cysharp.r3`
- `com.cysharp.unitask`
- `com.github-glitchenzo.nugetforunity`
- `jp.hadashikick.vcontainer`
- `com.unity.addressables`
- `com.unity.ai.navigation`
- `com.unity.animation.rigging`
- `com.unity.cinemachine`
- `com.unity.inputsystem`
- `com.unity.multiplayer.center`
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
- `Assets/_Root/Scripts/UI/UIConfig.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayConfig.cs`
- `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuConfig.cs`
- `Assets/_Root/Scripts/UI/Gameplay/Hud/UIHudConfig.cs`
- `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPauseMenuConfig.cs`
- `Assets/_Root/Scripts/Input/InputActionsProvider.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- `Assets/_Root/Scripts/UI/UIRoot.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlay.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayController.cs`
- `Assets/_Root/Scripts/Utilities/InstantiateUtilities.cs`
- `Assets/_Root/Scripts/Utilities/TweenUtilities.cs`
- `Assets/_Root/Scripts/Editor/BootstrapPlayModeRedirect.cs`
- `Assets/_Root/Scripts/Test/MainMenuTester.cs`
- `Assets/_Root/Scenes/Bootstrap.unity`
- `Assets/_Root/Scenes/MainMenu.unity`
- `Assets/_Root/Scenes/Gameplay.unity`
- `Assets/_Root/Configs/ProjectConfig.asset`
- `Assets/_Root/Prefabs/UI/pf_UI_MainMenuScreenView.prefab`
- `Assets/_Root/Prefabs/UI/pf_UI_PauseMenuPopUpView.prefab`
- `Packages/manifest.json`
- `ProjectSettings/EditorBuildSettings.asset`
- `ProjectSettings/ProjectVersion.txt`

## Related Docs

- `../project-map.md`
- `runtime-architecture-guidelines.md`
- `../systems/ci-cd.md`

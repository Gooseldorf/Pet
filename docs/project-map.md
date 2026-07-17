# Project Map

## Purpose

This page is the primary repository map.

Use it to locate authored Unity content, runtime code, documentation entrypoints, and delivery configuration.

## Project Summary

`Pet` is a Unity project with a bootstrap-scene startup flow, VContainer composition roots, a reactive input slice based on Unity Input System and R3, and a shared UI flow layer that now spans `MainMenu` and `Gameplay` scenes.

Long-term target platforms remain mobile and PC. The current automated delivery path is still WebGL through GitHub Pages.

## Top-Level Directories

| Path | Purpose |
| --- | --- |
| `Assets/` | Unity project assets, including authored content, imported packages, and third-party plugins |
| `Packages/` | Unity package manifest and lock file |
| `ProjectSettings/` | Unity project configuration and build settings |
| `.github/` | GitHub workflow configuration |
| `.opencode/` | Local OpenCode skills and agent support files |
| `docs/` | Technical knowledge base for humans and agents |
| `Data/` | Tool-managed package/plugin data generated for local dependencies |
| `ForAgents/` | Agent log output folders |
| `THIRD-PARTY-ASSETS.md` | Third-party asset attribution and licensing notes |

## Authored Unity Content

Authored project content lives under `Assets/_Root/`.

| Path | Purpose |
| --- | --- |
| `Assets/_Root/Scenes/` | Authored scenes |
| `Assets/_Root/Prefabs/` | Authored prefabs |
| `Assets/_Root/Configs/` | Authored `ScriptableObject` asset instances |
| `Assets/_Root/Scripts/` | Authored C# scripts and assembly definitions |
| `Assets/_Root/Settings/` | Render pipeline and volume settings assets |
| `Assets/_Root/Models/` | Authored models |
| `Assets/_Root/Materials/` | Authored materials |
| `Assets/_Root/Animations/` | Reserved authored animation folder; currently empty |

## Runtime Code

Current authored runtime code under `Assets/_Root/Scripts/` is split into focused top-level slices.

Current script slices:

- `Assets/_Root/Scripts/Bootstrap/` for startup entry points
- `Assets/_Root/Scripts/DI/` for VContainer lifetime scopes and composition roots
- `Assets/_Root/Scripts/SceneLoading/` for additive scene loading and content-scene switching
- `Assets/_Root/Scripts/Configs/` for root config types such as `ProjectConfig`
- `Assets/_Root/Scripts/Input/` for Unity Input System integration and player input streams
- `Assets/_Root/Scripts/UI/` for shared UI flow, base view types, UI config types, and scene-specific UI controllers
- `Assets/_Root/Scripts/Utilities/` for reusable runtime helpers around item instantiation and DOTween-based animations
- `Assets/_Root/Scripts/Test/` for test-style runtime scripts
- `Assets/_Root/Scripts/Editor/` for editor-only authored code

Known script paths:

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
- `Assets/_Root/Scripts/UI/UIInstanceFactory.cs`
- `Assets/_Root/Scripts/UI/UIScreenNavigator.cs`
- `Assets/_Root/Scripts/UI/UIPopupCoordinator.cs`
- `Assets/_Root/Scripts/UI/UIBackRouter.cs`
- `Assets/_Root/Scripts/UI/UIBackInputListener.cs`
- `Assets/_Root/Scripts/UI/UIConfig.cs`
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

Current observations:

- `Bootstrap.unity` remains the startup scene in build settings
- `SceneLoader` now supports both additive loading and switching between `MainMenu` and `Gameplay` while keeping `Bootstrap` loaded
- the UI layer is organized into shared flow code plus focused `MainMenu`, `Gameplay/Hud`, `Gameplay/PauseMenu`, and `LoadingScreen` slices
- the `Utilities` slice currently provides list population helpers for prefab-backed item collections and DOTween helper extensions for rotate/scale/wait flows
- config type definitions are no longer stored only under `Assets/_Root/Scripts/Configs/`; UI-related config types now live near their UI modules under `Assets/_Root/Scripts/UI/`
- config asset instances live under `Assets/_Root/Configs/`
- authored code compiles through `Pet.Runtime` and `Pet.Editor` assembly definitions

## Scenes

Known scenes:

- `Assets/_Root/Scenes/Bootstrap.unity`
- `Assets/_Root/Scenes/MainMenu.unity`
- `Assets/_Root/Scenes/Gameplay.unity`

Current startup flow:

- `ProjectSettings/EditorBuildSettings.asset` enables `Bootstrap`, `MainMenu`, and `Gameplay`
- `Assets/_Root/Scripts/DI/GlobalScope.cs` is the global VContainer lifetime scope
- `GlobalScope` serializes and registers a local `ProjectConfig` plus the persistent `UIRoot`
- `Assets/_Root/Scripts/Bootstrap/Bootstrap.cs` is registered as the initial entry point
- `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs` loads `MainMenu` additively on startup

## Local Config Layer

Current local config layer is split between config type definitions and config asset instances.

Config type definitions:

- root asset type: `Assets/_Root/Scripts/Configs/ProjectConfig.cs`
- shared UI branch type: `Assets/_Root/Scripts/UI/UIConfig.cs`
- loading overlay branch type: `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayConfig.cs`
- main menu screen config type: `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuConfig.cs`
- gameplay HUD config type: `Assets/_Root/Scripts/UI/Gameplay/Hud/UIHudConfig.cs`
- gameplay pause menu config type: `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPauseMenuConfig.cs`

Config asset instances:

- root config asset: `Assets/_Root/Configs/ProjectConfig.asset`
- UI config assets: `Assets/_Root/Configs/UI/_UIConfig.asset`, `Assets/_Root/Configs/UI/LoadingOverlayConfig.asset`, `Assets/_Root/Configs/UI/MainMenuConfig.asset`, `Assets/_Root/Configs/UI/PauseMenuConfig.asset`

Current registration and usage:

- global registration site: `Assets/_Root/Scripts/DI/GlobalScope.cs`
- scene-specific registrations: `Assets/_Root/Scripts/DI/MainMenuScope.cs`, `Assets/_Root/Scripts/DI/GameplayScope.cs`
- current shared consumer example: `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlay.cs`

## Prefabs

Known prefabs:

- `Assets/_Root/Prefabs/pf_Spider.prefab`
- `Assets/_Root/Prefabs/pf_SpiderIkTargets.prefab`
- `Assets/_Root/Prefabs/UI/pf_UI_MainMenuScreenView.prefab`
- `Assets/_Root/Prefabs/UI/pf_UI_PauseMenuPopUpView.prefab`

Observed naming convention:

- prefab names use the `pf_` prefix

## Build And Delivery

Current delivery path is WebGL to GitHub Pages.

Platform direction:

- long-term target platforms are mobile and PC
- the current WebGL path is an operational delivery workflow, not the long-term architectural target

Key files:

- workflow: `.github/workflows/deploy-pages.yml`
- Unity version: `ProjectSettings/ProjectVersion.txt`

## Package Highlights

Current notable packages in `Packages/manifest.json` include:

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

Current package-source notes:

- `Packages/manifest.json` defines an OpenUPM scoped registry for `jp.hadashikick.vcontainer`
- `com.cysharp.r3`, `com.cysharp.unitask`, and `com.github-glitchenzo.nugetforunity` are Git-based dependencies

## Unity Version

Current editor version:

- `ProjectSettings/ProjectVersion.txt` -> `6000.5.3f1`

## Tests

Current state:

- `com.unity.test-framework` is present in `Packages/manifest.json`
- there is no established authored automated test suite yet
- `Assets/_Root/Scripts/Test/MainMenuTester.cs` is still a runtime/test-style script, not a formal test assembly

Current authored assembly baseline:

- `Assets/_Root/Scripts/Pet.Runtime.asmdef`
- `Assets/_Root/Scripts/Editor/Pet.Editor.asmdef`

Current authored namespace baseline:

- `Pet` for bootstrap, composition-root, and scene-loading code
- `Pet.UI`, `Pet.Input`, `Pet.Configs`, `Pet.MainMenu`, `Pet.Gameplay`, and `Pet.Editor` for focused authored slices

## Key Conventions

- authored content is grouped under `Assets/_Root/`
- prefab names use the `pf_` prefix
- required Unity references should generally be assigned through serialized fields rather than runtime hierarchy search
- project-specific engineering constraints are defined in `AGENTS.md`

## Source Of Truth By Topic

| Topic | Source Of Truth |
| --- | --- |
| Repository map | `docs/project-map.md` |
| Third-party asset attribution | `THIRD-PARTY-ASSETS.md` |
| Agent navigation | `docs/ai/assistant-entrypoint.md` |
| Local coding skills | `docs/ai/coding-skills.md` |
| CI/CD | `docs/systems/ci-cd.md` |
| Platform strategy | `docs/systems/platform-strategy.md` |
| Unity authored layout | `docs/unity/project-structure.md` |
| Unity runtime code guidance | `docs/unity/runtime-architecture-guidelines.md` |
| Documentation maintenance workflow | `docs/workflows/updating-docs.md` |
| Notable project changes | `docs/history/milestones.md` |

## Related Docs

- `index.md`
- `ai/assistant-entrypoint.md`
- `ai/coding-skills.md`
- `ai/retrieval-map.md`
- `systems/ci-cd.md`
- `systems/platform-strategy.md`
- `unity/project-structure.md`
- `unity/runtime-architecture-guidelines.md`

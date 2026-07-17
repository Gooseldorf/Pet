# Project Map

## Purpose

This page is the primary map of the repository.

Use it to locate authored code, Unity content, build configuration, and documentation entrypoints.

## Project Summary

`Pet` is a Unity project currently centered around a small authored runtime surface, a bootstrap-scene startup flow with VContainer, a reactive input slice built on Unity Input System and R3, and a temporary WebGL deployment pipeline through GitHub Pages.

Long-term target platforms are mobile and PC.

## Top-Level Directories

| Path | Purpose |
| --- | --- |
| `Assets/` | Main Unity project content |
| `Packages/` | Unity package dependencies |
| `ProjectSettings/` | Unity project configuration |
| `.github/` | GitHub workflow configuration |
| `.opencode/` | Local OpenCode skills and related agent assets |
| `docs/` | Technical knowledge base for humans and agents |
| `THIRD-PARTY-ASSETS.md` | Third-party asset attribution and licensing notes |

## Authored Unity Content

Authored project content currently lives primarily under `Assets/_Root/`.

Key areas:

| Path | Purpose |
| --- | --- |
| `Assets/_Root/Scenes/` | Project scenes |
| `Assets/_Root/Prefabs/` | Authored prefabs |
| `Assets/_Root/Scripts/` | Authored C# scripts |
| `Assets/_Root/Settings/` | Authored project settings assets |

## Runtime Code

Current authored C# code under `Assets/_Root/Scripts/` is split into focused top-level slices.

Current script slices:

- `Assets/_Root/Scripts/Bootstrap/` for startup entry points
- `Assets/_Root/Scripts/DI/` for VContainer lifetime scopes and composition roots
- `Assets/_Root/Scripts/SceneLoading/` for additive scene loading
- `Assets/_Root/Scripts/Configs/` for local `ScriptableObject` config assets
- `Assets/_Root/Scripts/Input/` for Unity Input System integration and player input streams
- `Assets/_Root/Scripts/UI/` for authored UI runtime components
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
- `Assets/_Root/Scripts/Configs/UI/UIConfig.cs`
- `Assets/_Root/Scripts/Configs/UI/LoadingOverlayConfig.cs`
- `Assets/_Root/Scripts/Input/InputActionsProvider.cs`
- `Assets/_Root/Scripts/Input/InputActionObservableExtensions.cs`
- `Assets/_Root/Scripts/Input/Player/IPlayerInputStreams.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- `Assets/_Root/Scripts/UI/UiRoot.cs`
- `Assets/_Root/Scripts/UI/LoadingOverlay.cs`
- `Assets/_Root/Scripts/UI/LoadingOverlayController.cs`
- `Assets/_Root/Scripts/Editor/BootstrapPlayModeRedirect.cs`
- `Assets/_Root/Scripts/Test/MainMenuTester.cs`

Current observations:

- `Bootstrap.unity` is the startup scene in build settings
- VContainer is used to create the bootstrap entry point, lifetime scopes, and scene loader
- `ProjectConfig` is a local `ScriptableObject` config root registered in `GlobalScope`
- `MainMenu.unity` is loaded additively from the bootstrap flow
- authored code now compiles through `Pet.Runtime` and `Pet.Editor` assembly definitions
- authored namespaces are intentionally short and root at `Pet`
- `Assets/_Root/Scripts/Test/MainMenuTester.cs` remains a test-style runtime script and is not the project architecture entry point

## Scenes

Known scenes:

- `Assets/_Root/Scenes/Bootstrap.unity`
- `Assets/_Root/Scenes/MainMenu.unity`

Current startup flow:

- `ProjectSettings/EditorBuildSettings.asset` starts from `Assets/_Root/Scenes/Bootstrap.unity`
- `Assets/_Root/Scripts/DI/GlobalScope.cs` is the VContainer lifetime scope for startup
- `GlobalScope` serializes and registers a local `ProjectConfig`
- `Assets/_Root/Scripts/Bootstrap/Bootstrap.cs` is registered as the initial entry point
- `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs` loads `MainMenu` additively

## Local Config Layer

Current local config layer:

- root asset type: `Assets/_Root/Scripts/Configs/ProjectConfig.cs`
- current config asset types:
  - `Assets/_Root/Scripts/Configs/UI/UIConfig.cs`
  - `Assets/_Root/Scripts/Configs/UI/LoadingOverlayConfig.cs`
- current config branch: `UI -> LoadingOverlay -> FadeDuration`
- current bootstrap registration site: `Assets/_Root/Scripts/DI/GlobalScope.cs`
- current consumer: `Assets/_Root/Scripts/UI/LoadingOverlay.cs`

Current intent:

- shared authored configuration should move out of scene components and into the local config root when that improves ownership
- authored config types should be individual `ScriptableObject` assets rather than nested serializable classes
- the current config layer is local and bootstrap-wired, not a remote live-config system

## Prefabs

Known prefab:

- `Assets/_Root/Prefabs/pf_MainMenuButton.prefab`

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
- `com.unity.cinemachine`
- `com.unity.inputsystem`
- `com.unity.render-pipelines.universal`

Current package-source notes:

- `Packages/manifest.json` defines an OpenUPM scoped registry for `jp.hadashikick.vcontainer`
- `com.cysharp.r3` is pulled from the upstream Git repository at the Unity package path
- `com.cysharp.unitask` is pulled from the upstream Git repository at the Unity package path
- `com.github-glitchenzo.nugetforunity` is pulled from the upstream Git repository at the Unity package path

## Unity Version

Current editor version:

- `ProjectSettings/ProjectVersion.txt` -> `6000.5.3f1`

## Tests

Current state:

- `com.unity.test-framework` is present in `Packages/manifest.json`
- there is no established authored automated test suite yet
- `Assets/_Root/Scripts/Test/MainMenuTester.cs` is a runtime/test-style script, not a formal test assembly

Current authored assembly baseline:

- `Assets/_Root/Scripts/Pet.Runtime.asmdef`
- `Assets/_Root/Scripts/Editor/Pet.Editor.asmdef`

Current authored namespace baseline:

- `Pet` for bootstrap, composition-root, scene-loading, and shared runtime coordination
- `Pet.UI`, `Pet.Input`, `Pet.Configs`, `Pet.MainMenu`, `Pet.Gameplay`, and `Pet.Editor` for focused authored slices

## Key Conventions

- Authored content is grouped under `Assets/_Root/`.
- Prefabs use `pf_` naming.
- Required Unity references should generally be assigned through serialized fields rather than runtime hierarchy search.
- Project-specific engineering constraints are defined in `AGENTS.md`.

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
| Local config layer | `Assets/_Root/Scripts/Configs/` |
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

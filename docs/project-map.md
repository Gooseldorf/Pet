# Project Map

## Purpose

This page is the primary map of the repository.

Use it to locate authored code, Unity content, build configuration, and documentation entrypoints.

## Project Summary

`Pet` is a Unity project currently centered around a small authored runtime surface, a bootstrap-scene startup flow with VContainer, and a WebGL deployment pipeline through GitHub Pages.

The codebase is still compact, so this document acts as the top-level source of truth for where things live and which files to read first.

## Top-Level Directories

| Path | Purpose |
| --- | --- |
| `Assets/` | Main Unity project content |
| `Packages/` | Unity package dependencies |
| `ProjectSettings/` | Unity project configuration |
| `.github/` | GitHub workflow configuration |
| `.opencode/` | Local OpenCode skills and related agent assets |
| `docs/` | Technical knowledge base for humans and agents |

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

Current authored C# runtime code is still compact, but it now includes a bootstrap/composition-root slice under `Assets/_Root/Scripts/Architecture/`.

Known script paths:

- `Assets/_Root/Scripts/Architecture/Bootstrap/Bootstrap.cs`
- `Assets/_Root/Scripts/Architecture/Bootstrap/GlobalScope.cs`
- `Assets/_Root/Scripts/Architecture/SceneLoading/SceneLoader.cs`
- `Assets/_Root/Scripts/Test/MainMenuTester.cs`

Current observations:

- `Bootstrap.unity` is now the startup scene in build settings
- VContainer is used to create the bootstrap entry point and global scene loader
- `MainMenu.unity` is loaded additively from the bootstrap flow
- `Assets/_Root/Scripts/Test/MainMenuTester.cs` remains a test-style script and is not the project architecture entry point

## Scenes

Known scenes:

- `Assets/_Root/Scenes/Bootstrap.unity`
- `Assets/_Root/Scenes/MainMenu.unity`

Current startup flow:

- `ProjectSettings/EditorBuildSettings.asset` starts from `Assets/_Root/Scenes/Bootstrap.unity`
- `Assets/_Root/Scripts/Architecture/Bootstrap/GlobalScope.cs` is the VContainer lifetime scope for startup
- `Assets/_Root/Scripts/Architecture/Bootstrap/Bootstrap.cs` is registered as the entry point
- `Assets/_Root/Scripts/Architecture/SceneLoading/SceneLoader.cs` loads `MainMenu` additively

## Prefabs

Known prefab:

- `Assets/_Root/Prefabs/pf_MainMenuButton.prefab`

Observed naming convention:

- prefab names use the `pf_` prefix

## Build And Delivery

Current delivery path is WebGL to GitHub Pages.

Key files:

- workflow: `.github/workflows/deploy-pages.yml`
- Unity version: `ProjectSettings/ProjectVersion.txt`

## Package Highlights

Current notable packages in `Packages/manifest.json` include:

- `com.cysharp.unitask`
- `jp.hadashikick.vcontainer`
- `com.unity.addressables`
- `com.unity.ai.navigation`
- `com.unity.cinemachine`
- `com.unity.render-pipelines.universal`

These packages indicate planned or available support for async flows, DI, asset delivery, navigation, camera systems, and URP rendering.

## Unity Version

Current editor version:

- `ProjectSettings/ProjectVersion.txt` -> `6000.5.3f1`

## Tests

Current state:

- `com.unity.test-framework` is present in `Packages/manifest.json`
- there is no established authored automated test suite yet
- `Assets/_Root/Scripts/Test/MainMenuTester.cs` is a runtime/test-style script, not a formal test assembly

## Key Conventions

- Authored content is grouped under `Assets/_Root/`.
- Prefabs use `pf_` naming.
- Required Unity references should generally be assigned through serialized fields rather than runtime hierarchy search.
- Project-specific engineering constraints are defined in `AGENTS.md`.

## Source Of Truth By Topic

| Topic | Source Of Truth |
| --- | --- |
| Repository map | `docs/project-map.md` |
| Agent navigation | `docs/ai/assistant-entrypoint.md` |
| Local coding skills | `docs/ai/coding-skills.md` |
| CI/CD | `docs/systems/ci-cd.md` |
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
- `unity/project-structure.md`
- `unity/runtime-architecture-guidelines.md`

# Project Map

## Purpose

This page is the primary map of the repository.

Use it to locate authored code, Unity content, build configuration, and documentation entrypoints.

## Project Summary

`Pet` is a Unity project currently centered around a small authored runtime surface and a WebGL deployment pipeline through GitHub Pages.

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

Current authored C# runtime code is minimal.

Known script path:

- `Assets/_Root/Scripts/Test/MainMenuTester.cs`

Current observations:

- the only authored script is in a `Test` namespace
- serialized references are used for UI wiring
- current behavior is a simple UI counter flow

## Scenes

Known scene:

- `Assets/_Root/Scenes/MainMenu.unity`

## Prefabs

Known prefab:

- `Assets/_Root/Prefabs/pf_MainMenuButton.prefab`

Observed naming convention:

- prefab names use the `pf_` prefix

## Build And Delivery

Current delivery path is WebGL to GitHub Pages.

Key files:

- workflow: `.github/workflows/deploy-pages.yml`
- build profile: `Assets/Settings/Build Profiles/Web - Desktop - Release.asset`

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
| CI/CD | `docs/systems/ci-cd.md` |
| Unity authored layout | `docs/unity/project-structure.md` |
| Documentation maintenance workflow | `docs/workflows/updating-docs.md` |
| Notable project changes | `docs/history/milestones.md` |

## Related Docs

- `index.md`
- `ai/assistant-entrypoint.md`
- `ai/retrieval-map.md`
- `systems/ci-cd.md`
- `unity/project-structure.md`

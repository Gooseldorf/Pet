# Milestones

## Purpose

This page records notable technical milestones that should not live only in chat history.

## Milestones

### Bootstrap scene and VContainer startup established

Status: completed

Summary:

- The project now starts through a dedicated bootstrap scene and a VContainer composition root.

Key artifacts:

- scene: `Assets/_Root/Scenes/Bootstrap.unity`
- scope: `Assets/_Root/Scripts/Architecture/Bootstrap/GlobalScope.cs`
- entry point: `Assets/_Root/Scripts/Architecture/Bootstrap/Bootstrap.cs`
- scene loading: `Assets/_Root/Scripts/Architecture/SceneLoading/SceneLoader.cs`
- build settings: `ProjectSettings/EditorBuildSettings.asset`

Impact:

- application startup is no longer coupled to opening `MainMenu.unity` directly
- the repository now has an explicit composition root for global runtime wiring
- additive scene loading is now part of the documented runtime startup path

Related docs:

- `../project-map.md`
- `../unity/project-structure.md`
- `../unity/runtime-architecture-guidelines.md`

### CI/CD module established

Status: completed

Summary:

- A GitHub Actions workflow now builds a Unity WebGL player and deploys it to GitHub Pages.

Key artifacts:

- workflow: `.github/workflows/deploy-pages.yml`

Impact:

- the repository now has an automated WebGL delivery path
- CI/CD is now a documented project system and should be updated when the workflow changes

Related docs:

- `../systems/ci-cd.md`
- `../project-map.md`

### Project knowledge base established

Status: completed

Summary:

- A Markdown-based technical knowledge base was added under `docs/`.

Impact:

- project memory no longer depends only on chat history
- humans and coding agents now have shared documentation entrypoints
- documentation maintenance is now part of normal project work

Key artifacts:

- `docs/index.md`
- `docs/project-map.md`
- `docs/ai/assistant-entrypoint.md`
- `docs/systems/ci-cd.md`
- `docs/unity/project-structure.md`

Related docs:

- `../index.md`
- `../workflows/updating-docs.md`

### Platform strategy documented separately from the current WebGL pipeline

Status: completed

Summary:

- The knowledge base now distinguishes the temporary WebGL delivery workflow from the project's long-term mobile-plus-PC platform direction.

Impact:

- coding agents should no longer treat WebGL as the default long-term architectural target
- current deployment facts remain documented without overriding long-term platform intent

Key artifacts:

- `docs/systems/platform-strategy.md`
- `docs/ai/assistant-entrypoint.md`
- `docs/project-map.md`
- `docs/systems/ci-cd.md`

Related docs:

- `../systems/platform-strategy.md`
- `../systems/ci-cd.md`
- `../project-map.md`

### Local ScriptableObject config layer established

Status: completed

Summary:

- The runtime architecture now includes a local `ScriptableObject` config root for shared authored values.

Impact:

- shared authored values can start moving out of scene components into a bootstrap-wired config layer
- the current startup graph now includes a local config dependency registered in `GlobalScope`
- config branches are now modeled as separate `ScriptableObject` asset types instead of nested serializable classes

Key artifacts:

- `Assets/_Root/Scripts/Configs/ProjectConfig.cs`
- `Assets/_Root/Scripts/Configs/UI/UIConfig.cs`
- `Assets/_Root/Scripts/Configs/UI/LoadingOverlayConfig.cs`
- `Assets/_Root/Scripts/Architecture/Bootstrap/GlobalScope.cs`
- `Assets/_Root/Scripts/UI/LoadingOverlay.cs`

Related docs:

- `../project-map.md`
- `../unity/project-structure.md`
- `../unity/runtime-architecture-guidelines.md`

### Authored script folders reorganized into top-level slices

Status: completed

Summary:

- Authored runtime scripts under `Assets/_Root/Scripts/` were reorganized so bootstrap, config, UI, and test code now live in separate top-level folders.

Impact:

- docs and agent retrieval can now reason about script ownership from folder layout more directly
- config asset types are no longer documented as part of the bootstrap architecture folder
- UI runtime components now have an explicit home separate from startup code

Key artifacts:

- `Assets/_Root/Scripts/Architecture/`
- `Assets/_Root/Scripts/Configs/`
- `Assets/_Root/Scripts/UI/`
- `Assets/_Root/Scripts/Test/`

Related docs:

- `../project-map.md`
- `../ai/assistant-entrypoint.md`
- `../unity/project-structure.md`
- `../unity/runtime-architecture-guidelines.md`

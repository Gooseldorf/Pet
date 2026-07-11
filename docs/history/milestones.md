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

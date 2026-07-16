# Assistant Entrypoint

## Purpose

This is the shortest project-specific entrypoint for coding agents.

Read this file first when you need to orient quickly.

## Read By Task

| If the task is about... | Read this first |
| --- | --- |
| overall repository structure | `docs/project-map.md` |
| third-party asset attribution | `THIRD-PARTY-ASSETS.md` |
| CI/CD, build, or deployment | `docs/systems/ci-cd.md` |
| Unity authored layout | `docs/unity/project-structure.md` |
| Unity runtime code shape or coding skills | `docs/unity/runtime-architecture-guidelines.md` |
| documentation maintenance | `docs/workflows/updating-docs.md` |
| project history or recently established systems | `docs/history/milestones.md` |

## Current Project Facts

- The repository is a Unity project.
- Authored content currently lives primarily under `Assets/_Root/`.
- The project now starts through `Assets/_Root/Scenes/Bootstrap.unity`.
- VContainer is used for the bootstrap composition root under `Assets/_Root/Scripts/Architecture/Bootstrap/`.
- The project now has a local `ScriptableObject` config layer under `Assets/_Root/Scripts/Configs/`, with `ProjectConfig`, `UIConfig`, and `LoadingOverlayConfig` as separate config assets.
- Authored UI runtime components currently live under `Assets/_Root/Scripts/UI/`.
- `Assets/_Root/Scripts/Architecture/SceneLoading/SceneLoader.cs` currently performs additive scene loading into `MainMenu`.
- Long-term target platforms are mobile and PC.
- The current delivery path is a temporary WebGL deployment workflow through GitHub Pages.
- The current WebGL workflow is defined in `.github/workflows/deploy-pages.yml`.
- Third-party asset attributions are tracked in `THIRD-PARTY-ASSETS.md`.
- WebGL constraints should not dominate long-term architecture decisions unless the task explicitly targets the current deployment path.
- Local AI coding skills live under `.opencode/skills/`.

## Source Of Truth

- Project map: `docs/project-map.md`
- Third-party asset attribution: `THIRD-PARTY-ASSETS.md`
- CI/CD: `docs/systems/ci-cd.md`
- Platform strategy: `docs/systems/platform-strategy.md`
- Unity structure: `docs/unity/project-structure.md`
- Runtime architecture: `docs/unity/runtime-architecture-guidelines.md`
- Local config layer: `Assets/_Root/Scripts/Configs/`
- Project rules and constraints: `AGENTS.md`

## Retrieval Notes

- Prefer `docs/` over chat history when looking for settled project information.
- Prefer repository paths and concrete files over summaries.
- If a significant system changes, update the related doc and `docs/history/milestones.md`.

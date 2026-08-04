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
| external Unity composition reference | `docs/unity/unity-composition-guide.md` |
| spider player controller implementation | `docs/unity/spider-player-controller-plan.md` |
| external agent-style guidance reference | `docs/ai/external-agent-guidance.md` |
| documentation maintenance | `docs/workflows/updating-docs.md` |
| project history or recently established systems | `docs/history/milestones.md` |

## Current Project Facts

- The repository is a Unity project.
- Authored content lives under `Assets/_Root/`.
- `Assets/_Root/` now separates authored scenes, prefabs, configs, scripts, settings, models, materials, and animations.
- The project starts through `Assets/_Root/Scenes/Bootstrap.unity`.
- VContainer is used across `Assets/_Root/Scripts/Bootstrap/` and `Assets/_Root/Scripts/DI/`.
- The current build settings include `Bootstrap`, `MainMenu`, and `Gameplay` scenes.
- The project has a local `ScriptableObject` config layer with root type `Assets/_Root/Scripts/Configs/ProjectConfig.cs`, UI-related config types under `Assets/_Root/Scripts/UI/`, and config asset instances under `Assets/_Root/Configs/`.
- Authored UI runtime components currently live under `Assets/_Root/Scripts/UI/` and are split into shared flow, `LoadingScreen`, `MainMenu`, and `Gameplay` slices.
- `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs` performs additive scene loading, explicit `SetActiveScene` handoff, and content-scene startup.
- `Assets/_Root/Scripts/SceneLoading/ISceneEntryPoint.cs` is the explicit startup contract for `MainMenu` and `Gameplay` scene entry points.
- Runtime guidance prefers composition over inheritance for authored gameplay code, while still avoiding mechanical over-splitting.
- Authored namespaces stay short and root at `Pet`, while `asmdef` boundaries carry stronger architectural isolation.
- The current authored assembly baseline is `Assets/_Root/Scripts/Pet.Runtime.asmdef` plus `Assets/_Root/Scripts/Editor/Pet.Editor.asmdef`.
- Long-term target platforms are mobile and PC.
- The current delivery path is a temporary WebGL deployment workflow through GitHub Pages.
- The current WebGL workflow is defined in `.github/workflows/deploy-pages.yml`.
- Third-party asset attributions are tracked in `THIRD-PARTY-ASSETS.md`.
- Local AI coding skills live under `.opencode/skills/`.

## Source Of Truth

- Project map: `docs/project-map.md`
- Third-party asset attribution: `THIRD-PARTY-ASSETS.md`
- CI/CD: `docs/systems/ci-cd.md`
- Platform strategy: `docs/systems/platform-strategy.md`
- Unity structure: `docs/unity/project-structure.md`
- Runtime architecture: `docs/unity/runtime-architecture-guidelines.md`
- External Unity composition reference: `docs/unity/unity-composition-guide.md`
- External agent guidance reference: `docs/ai/external-agent-guidance.md`
- Project rules and constraints: `AGENTS.md`

## Retrieval Notes

- Prefer `docs/` over chat history when looking for settled project information.
- Prefer repository paths and concrete files over summaries.
- If a significant system changes, update the related doc and `docs/history/milestones.md`.

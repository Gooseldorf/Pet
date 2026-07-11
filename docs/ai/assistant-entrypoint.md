# Assistant Entrypoint

## Purpose

This is the shortest project-specific entrypoint for coding agents.

Read this file first when you need to orient quickly.

## Read By Task

| If the task is about... | Read this first |
| --- | --- |
| overall repository structure | `docs/project-map.md` |
| CI/CD, build, or deployment | `docs/systems/ci-cd.md` |
| Unity authored layout | `docs/unity/project-structure.md` |
| Unity runtime code shape or coding skills | `docs/unity/runtime-architecture-guidelines.md` |
| documentation maintenance | `docs/workflows/updating-docs.md` |
| project history or recently established systems | `docs/history/milestones.md` |

## Current Project Facts

- The repository is a Unity project.
- Authored content currently lives primarily under `Assets/_Root/`.
- Current delivery path is WebGL deployment to GitHub Pages.
- The current WebGL workflow is defined in `.github/workflows/deploy-pages.yml`.
- The workflow currently builds WebGL directly through `game-ci/unity-builder`.
- Local AI coding skills live under `.opencode/skills/`.

## Source Of Truth

- Project map: `docs/project-map.md`
- CI/CD: `docs/systems/ci-cd.md`
- Unity structure: `docs/unity/project-structure.md`
- Runtime architecture: `docs/unity/runtime-architecture-guidelines.md`
- Project rules and constraints: `AGENTS.md`

## Retrieval Notes

- Prefer `docs/` over chat history when looking for settled project information.
- Prefer repository paths and concrete files over summaries.
- If a significant system changes, update the related doc and `docs/history/milestones.md`.

# Retrieval Map

## Purpose

This page helps agents retrieve project context with minimal searching.

## Topic Index

| Topic | Read | Notes |
| --- | --- | --- |
| repository overview | `docs/project-map.md` | First stop for structure and paths |
| third-party asset attribution | `THIRD-PARTY-ASSETS.md` | Use for required asset credits and license references |
| build pipeline | `docs/systems/ci-cd.md` | Includes workflow path, triggers, secrets, failure modes |
| Unity structure | `docs/unity/project-structure.md` | Includes scenes, prefabs, scripts, naming conventions |
| Unity runtime code guidance | `docs/unity/runtime-architecture-guidelines.md` | Use for code ownership, MonoBehaviour boundaries, explicit additive-scene startup, and multiplayer review expectations |
| spider player controller roadmap | `docs/unity/spider-player-controller-plan.md` | Use for staged spider controller implementation, boundaries, and milestone order |
| local coding skills | `docs/ai/coding-skills.md` | Use to choose the right project skill for implementation or review work |
| documentation process | `docs/workflows/updating-docs.md` | Use when creating or updating docs |
| project milestones | `docs/history/milestones.md` | Use when you need historical context |
| project-wide constraints | `AGENTS.md` | Engineering and Unity-specific rules |

## Suggested Retrieval Order

1. Read `docs/ai/assistant-entrypoint.md`.
2. Read `docs/project-map.md`.
3. Read the topic-specific source-of-truth page.
4. Read repository files referenced by that page.

## Authoring Rule

If you create a new source-of-truth page, add it here and link it from `docs/project-map.md`.

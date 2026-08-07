# Project Map

Use this page only when the responsible repository area is not already clear. Current names, wiring, versions, and package details are repository facts and must be inspected directly.

| Area | Ownership or authority |
| --- | --- |
| Authored Unity content | `Assets/_Root/`; scenes, prefabs, configs, runtime scripts, and authored settings are separate subtrees. |
| Runtime startup and composition | `Assets/_Root/Scripts/Bootstrap/`, `DI/`, and `SceneLoading/`; durable intent: [runtime architecture](unity/runtime-architecture.md). |
| Gameplay and UI | `Assets/_Root/Scripts/Gameplay/` and `UI/`; inspect the affected slice and serialized integration. |
| Editor-only code | `Assets/_Root/Scripts/Editor/`. |
| Unity dependencies | `Packages/manifest.json` and `Packages/packages-lock.json`. |
| Unity project settings and build scenes | Relevant files under `ProjectSettings/`. |
| Automation | `.github/workflows/`; YAML is authoritative for exact CI and deployment behavior. |
| Agent workflows | `.opencode/skills/`; skill frontmatter is the discovery authority. |
| Documentation | `docs/`; durable topic owners are listed in [the index](index.md). |
| Asset attribution | `THIRD-PARTY-ASSETS.md` and upstream license evidence. |

Related durable decisions: [CI/CD](systems/ci-cd.md), [platform strategy](systems/platform-strategy.md), [runtime architecture](unity/runtime-architecture.md), and [spider player](unity/spider-player.md).

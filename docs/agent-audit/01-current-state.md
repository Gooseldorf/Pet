# Agent Setup Audit: Current State

## Current Context Hierarchy

- `AGENTS.md` is the only repository `AGENTS.md`; at repository root it applies to the whole tree. It contains project priorities, Unity/C# rules, multiplayer review requirements, and serialized-asset editing boundaries.
- `AGENTS.md` points agents to `docs/project-map.md`, `docs/ai/assistant-entrypoint.md`, `docs/ai/retrieval-map.md`, `docs/systems/ci-cd.md`, `docs/unity/project-structure.md`, and `docs/history/milestones.md`.
- `docs/index.md` is the human/agent documentation index. `docs/ai/assistant-entrypoint.md` is the shortest task-routing entrypoint. `docs/ai/retrieval-map.md` defines the suggested retrieval order.
- No nested `AGENTS.md` files were found under `Assets/`, `docs/`, `.github/`, or `.opencode/`.

## Files Involved

- Always-on repository rules: `AGENTS.md`.
- Agent navigation: `docs/index.md`, `docs/project-map.md`, `docs/ai/assistant-entrypoint.md`, `docs/ai/retrieval-map.md`, `docs/ai/coding-skills.md`.
- Agent-oriented secondary references: `docs/ai/external-agent-guidance.md`, `docs/unity/unity-composition-guide.md`.
- Local OpenCode package and skill library: `.opencode/package.json`, `.opencode/package-lock.json`, `.opencode/skills/`.
- Delivery automation: `.github/workflows/deploy-pages.yml`.
- Agent/build logs: `ForAgents/logs_*/`.
- No Codex configuration, MCP configuration, prompt files, Claude configuration, or additional agent rule files were found in the repository search.

## What Is Always Loaded

- The repository-level `AGENTS.md` is the only repository context identified as baseline/always-on by the local documentation (`docs/ai/assistant-entrypoint.md:45-55`, `docs/ai/coding-skills.md:120-124`).
- No repository file declares a global OpenCode agent, MCP server, or Codex instruction set. `.opencode/package.json` only declares `@opencode-ai/plugin`.
- The documentation index and agent entrypoint are discoverable but are not configured here as automatically loaded files.

## What Is Loaded On Demand

- `docs/ai/assistant-entrypoint.md` routes by task to `docs/project-map.md`, `docs/systems/ci-cd.md`, `docs/unity/project-structure.md`, `docs/unity/runtime-architecture-guidelines.md`, `docs/unity/spider-player-controller-plan.md`, `docs/ai/external-agent-guidance.md`, `docs/workflows/updating-docs.md`, and `docs/history/milestones.md`.
- `docs/ai/retrieval-map.md` recommends entrypoint, project map, topic source-of-truth, then referenced source files.
- `.opencode/skills/` contains 13 on-demand skills: `docs-architect`, `docs-auditor`, `docs-writer`, `knowledge-synthesizer`, `skill-creator`, `skill-tester`, `unity-architecture-review`, `unity-feature-implementation`, `unity-multiplayer-review`, `unity-refactor-aposd`, `unity-spider-player-implementation`, `unity-ui-flow-implementation`, and `unity-ui-flow-review`.
- Skill-specific reference files exist under `.opencode/skills/*/references/`, notably checklists, taxonomies, and review prompts. There are no skill scripts or MCP-backed workflows.

## Available Agent Tools/Workflows

- OpenCode-local workflow guidance is provided by the skills listed above; implementation/review skills specify retrieval, editor-wiring follow-up, multiplayer checks, and expected completion output.
- Documentation maintenance is described in `docs/workflows/updating-docs.md`; milestone capture is described in `.opencode/skills/knowledge-synthesizer/SKILL.md` and `docs/history/milestones.md`.
- GitHub Actions runs `Build and Deploy Pages` from `.github/workflows/deploy-pages.yml` on pushes to `WebGLBuild` or manual dispatch. It checks out LFS, caches `Library`, builds WebGL with `game-ci/unity-builder`, and deploys GitHub Pages.
- `ForAgents/logs_*/` preserves prior workflow output, but is evidence/logging rather than an executable agent workflow.
- No repository-local shell, PowerShell, batch, lint, formatter, or test orchestration scripts were found.

## Architecture/Project Context Available to Agents

- Repository and source-of-truth map: `docs/project-map.md`.
- Unity layout and known authored paths: `docs/unity/project-structure.md`.
- Runtime/startup and ownership guidance: `docs/unity/runtime-architecture-guidelines.md`.
- Platform distinction and WebGL interpretation: `docs/systems/platform-strategy.md` and `docs/systems/ci-cd.md`.
- Spider roadmap: `docs/unity/spider-player-controller-plan.md`.
- Historical changes: `docs/history/milestones.md`.
- Third-party README files are package documentation only: `Assets/Packages/Newtonsoft.Json.13.0.4/README.md` and `Assets/Packages/R3.1.3.1/README.md`. No project root README was found.

## Validation Capabilities

- Unity version is `6000.5.3f1` in `ProjectSettings/ProjectVersion.txt`; authored assemblies are `Assets/_Root/Scripts/Pet.Runtime.asmdef` and `Assets/_Root/Scripts/Editor/Pet.Editor.asmdef`.
- The committed CI path validates a WebGL build/deployment only; `.github/workflows/deploy-pages.yml` has no test, lint, format, or static-analysis step.
- Unity Test Framework is declared in `Packages/manifest.json` (`com.unity.test-framework: 1.7.0`). `Pet.Tests.EditMode.csproj` and `Pet.Tests.PlayMode.csproj` reference `Assets/_Root/Scripts/Test/EditMode/Pet.Tests.EditMode.asmdef` and `Assets/_Root/Scripts/Test/PlayMode/Pet.Tests.PlayMode.asmdef`, but those directories/files were not found.
- The only authored test-style source found is `Assets/_Root/Scripts/Test/MainMenuTester.cs`; `docs/project-map.md` describes it as non-formal. `ProjectSettings/ProjectSettings.asset` has `playModeTestRunnerEnabled: 0`.
- Generated `.csproj` files expose Unity compiler references, but no committed command or workflow establishes `dotnet build`, `dotnet test`, Unity batch-mode tests, linting, or formatting as supported validation.

## Obvious Duplication

- Repository facts and path inventories repeat across `docs/project-map.md`, `docs/unity/project-structure.md`, and `docs/ai/assistant-entrypoint.md`.
- Agent routing repeats across `docs/index.md`, `docs/ai/assistant-entrypoint.md`, `docs/ai/retrieval-map.md`, and `docs/ai/coding-skills.md`.
- Composition, initialization, serialized wiring, null handling, abstraction, and verification rules repeat between `AGENTS.md`, `docs/unity/runtime-architecture-guidelines.md`, `.opencode/skills/unity-feature-implementation/SKILL.md`, and the UI/spider skills.
- External guidance is retained in `docs/ai/external-agent-guidance.md` while its adopted subset is repeated in `AGENTS.md`, `docs/unity/runtime-architecture-guidelines.md`, and `docs/history/milestones.md`.

## Obvious Missing Context

- No single documented command matrix tells agents how to compile, run EditMode/PlayMode tests, validate scenes/prefabs, or perform static checks; `docs/systems/ci-cd.md` covers only deployment.
- The repository does not document a local Unity batch-mode workflow, test filters, log locations, or expected exit-code handling. `docs/systems/ci-cd.md:97-100` also records that release details are incomplete.
- Agents must inspect Unity assets/source to determine actual scene/prefab/config wiring; the docs list paths but do not provide a compact validation or wiring checklist beyond individual skills.
- The agent setup has no repository-specific MCP, Codex, prompt, or automation configuration beyond OpenCode skills and GitHub Actions.
- The project lacks a root README or agent-facing contribution/runbook document; existing READMEs are third-party package READMEs.

## Obvious Stale or Contradictory Documentation

- `docs/unity/spider-player-controller-plan.md` still names planned/removed runtime files such as `SpiderSurfaceComponent.cs`, `SpiderMovementComponent.cs`, and `GameplayTester.cs`; the current tree contains only `Assets/_Root/Scripts/Test/MainMenuTester.cs`, while `docs/project-map.md:126` and `docs/history/milestones.md:38-57` describe the removal.
- `docs/history/milestones.md` intentionally records historical entries, but later readers can mistake lines `91-136`, `465-484`, and `547-565` for current implementation because they reference deleted spider files without an explicit historical-only warning per entry.
- `docs/systems/ci-cd.md:73` says the workflow does not reference a committed build profile, while archived workflow output in `ForAgents/logs_78748913929/Build WebGL/5_Build WebGL player.txt:5` shows a `buildProfile` value. The log may come from a different workflow revision, so current behavior is uncertain.
- `.opencode/skills/unity-spider-player-implementation/SKILL.md:50-65` prescribes component stages whose files are absent, while its cautions at lines `81-84` describe a minimal/current baseline. These two contexts can lead agents to follow obsolete stage assumptions.

## Uncertainties Requiring Deeper Inspection

- Whether `.opencode/` is intentionally local/untracked is unclear: `.opencode/.gitignore` ignores its package files, while the repository contains the skills. Git history and ignore status should be checked before treating the skill library as durable shared configuration.
- Whether archived `ForAgents/logs_*/` are generated from this exact commit is unknown; timestamps and workflow inputs do not match all current documentation.
- The generated test `.csproj` files may be stale Unity/Rider artifacts rather than active test configuration; Unity Editor project metadata and Git history would be needed to establish intent.
- Automatic loading behavior outside repository files, including user-level OpenCode/Codex/MCP configuration, cannot be established from this repository inspection.
- Current authored Inspector references and serialized type identifiers need Unity Editor inspection; `docs/ai` skills explicitly warn that some serialized assets may retain old identifiers (`.opencode/skills/unity-ui-flow-implementation/SKILL.md:93-97`).

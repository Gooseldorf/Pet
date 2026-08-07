# Target Agentic Workflow And Context Architecture

## Decision

The target is a repository with:

- one always-loaded repository instruction file: root `AGENTS.md`
- host-advertised skill names and descriptions treated as baseline metadata, with skill bodies loaded on demand
- no nested `AGENTS.md` files
- no separate agent navigation document
- a small set of durable, topic-owned documents loaded only when relevant
- a smaller skill library containing workflows rather than copied policy or file inventories
- repository files as the authority for volatile facts
- one executable local validation entrypoint plus a CI adapter with the same result contract
- no generated repository map, generic ADR framework, task-template library, or duplicated Codex context

Root `AGENTS.md` becomes both the compact document/authority router and the owner of the few cross-cutting constraints worth paying for on every task. It does not duplicate the skill catalog: OpenCode skill frontmatter owns skill matching. This is more reliable and cheaper than requiring a second `docs/ai/assistant-entrypoint.md`, because the repository does not establish that the latter is loaded automatically.

This document defines the target. It does not implement the migration.

## Evidence Boundary

The design is based on `01-current-state.md`, `02-context-audit.md`, and `03-workflow-audit.md`, with targeted repository validation on 2026-08-07 against commit `34a6da5e011204b97605d30bf68e9012f74ff8ac`.

Important validated corrections to the earlier audits are:

- `docs/ai/assistant-entrypoint.md` is discoverable but not repository-configured as automatic context. Calling it `ALWAYS LOAD` does not make it so.
- Three ignored generated test projects, not two, refer to absent test assemblies: `Pet.Test.csproj`, `Pet.Tests.EditMode.csproj`, and `Pet.Tests.PlayMode.csproj`. None is a validation contract.
- `playModeTestRunnerEnabled: 0` is not sufficient evidence that normal custom PlayMode test assemblies are disabled. The decisive current gap is that no authored test asmdefs or test methods exist.
- The historical build-profile discrepancy is resolved. The old log came from an earlier workflow revision; the current workflow and current `docs/systems/ci-cd.md` agree that no committed build profile is referenced.
- `docs/unity/spider-player-controller-plan.md` intentionally names removed source files. The material contradiction is the spider skill, which still prescribes those absent components and incorrectly defers camera integration.
- Old serialized type identifiers are observable, but an old identifier alone does not prove a broken reference. More serious serialized residue also exists, including a missing `GameplayTester` script in `Gameplay.unity`; this justifies asset-integrity validation rather than more prose warnings.
- `.opencode/skills/` is tracked. The local `.opencode/.gitignore`, package manifest, lock file, and `node_modules` are ignored workspace state, not a durable shared project contract.
- OpenCode exposes available skill names and descriptions to the agent before a skill body is loaded in the current environment. That metadata has a baseline token cost even though `SKILL.md` bodies remain on demand; the target must measure it and must not copy the same catalog into `AGENTS.md`.
- The four files in `docs/agent-audit/` are currently untracked. They should be treated as temporary migration evidence unless deliberately committed for the migration.

## Optimization Model

The target context budget is behavioral rather than a fragile line-count target:

| Task stage | Context budget |
| --- | --- |
| Every task | Root `AGENTS.md` plus host-advertised skill name/description metadata when the host exposes it |
| Initial task routing | At most one primary skill body |
| Project intent or non-obvious architecture | At most one topic document initially |
| Current implementation | Targeted source, asset, manifest, settings, or workflow reads |
| Cross-cutting multiplayer work | One additional multiplayer review skill when authority or peer consistency is actually relevant |
| History | Nothing unless the user explicitly asks for history |

An agent may load more when evidence requires it, but broad orientation is not a prerequisite for focused work. `docs/index.md` and `docs/project-map.md` are not default reads. Baseline measurements must count both `AGENTS.md` and any host-advertised skill metadata; only skill bodies count as on-demand context.

Useful context has one of four jobs:

1. route the task
2. preserve a durable decision that code cannot explain
3. define a repeated workflow
4. provide executable feedback

Content that does none of these jobs should be deleted.

## Authority And Precedence

The target uses different authorities for policy and observable facts.

| Information | Authority | Retrieval rule |
| --- | --- | --- |
| Cross-cutting project constraints | `AGENTS.md` | Already loaded; do not restate elsewhere |
| Current C# behavior and ownership | Current source and tests under `Assets/_Root/` | Search the affected slice directly |
| Scene, prefab, and config wiring | Current `.unity`, `.prefab`, and `.asset` files, preferably inspected through Unity | Never infer wiring coverage from a prose inventory |
| Unity version | `ProjectSettings/ProjectVersion.txt` | Read only for Unity execution or compatibility work |
| Package versions and sources | `Packages/manifest.json` and `Packages/packages-lock.json` | Read only for dependency work |
| Build scenes and project settings | Relevant files under `ProjectSettings/` | Read the exact setting needed |
| Exact CI triggers, actions, inputs, and paths | `.github/workflows/*.yml` | Workflow YAML overrides prose summaries |
| Long-lived product or architecture intent | The narrow topic document that owns the decision | Load only when the task can be affected by that decision |
| Repeated task procedure | Matching skill and executable tool | Skills guide decisions; tools determine pass or fail |
| Third-party attribution | `THIRD-PARTY-ASSETS.md` and upstream license evidence | Load only for asset or licensing work |

Conflict resolution is explicit:

1. user requirements control the requested outcome
2. `AGENTS.md` controls repository-wide execution constraints
3. repository authorities control current observable facts
4. topic documents control durable intent not derivable from the repository
5. skills control task procedure but cannot override the preceding authorities
6. Git history and audit records explain the past but never override current state

## Context Layers

### Layer 0: Repository Evidence

Source, serialized assets, manifests, settings, and workflows are searched directly. They are not preloaded and are not mirrored into documentation.

### Layer 1: Always-Visible Routing Surface

Root `AGENTS.md` is the only always-loaded repository instruction file. It contains the authority rule, compact topic/authority routes, high-value project constraints, and the validation completion contract. In hosts that advertise project skills, skill names and descriptions are also baseline routing metadata and must be counted, but their bodies are not loaded yet.

### Layer 2: Task Workflow

One matching `.opencode` skill body is loaded for a repeated specialized job. Its frontmatter description, not a copied table in `AGENTS.md`, owns the trigger. The multiplayer review skill may be added when a task has authority, replication, or peer-consistency implications.

### Layer 3: Durable Topic Knowledge

A topic page is read only when a task depends on non-obvious project intent such as startup ownership, spider rewrite boundaries, platform direction, CI operation, or documentation policy.

### Layer 4: Historical Evidence

Git history is the default history store. The audit package exists only while the migration is active and is never part of normal task retrieval.

## Root Routing Contract

The target `AGENTS.md` says to use a host-advertised matching skill when one exists, but does not list skill names or descriptions. It routes only the non-obvious topic context and repository authority below. This table describes its intended content, not an additional page to maintain.

| Task | Optional topic context | Repository authority to inspect first |
| --- | --- | --- |
| Routine Unity implementation, UI work, architecture review, or refactor | None by default | Affected code, serialized integration, and adjacent tests under `Assets/_Root/` |
| Startup, DI, additive scenes, or ordered initialization | `docs/unity/runtime-architecture.md` | `Bootstrap/`, `DI/`, `SceneLoading/`, and relevant scene scopes |
| UI work that changes startup ownership | `docs/unity/runtime-architecture.md` | Relevant UI slice, DI scope, config, prefab, and scene |
| Spider traversal, jump, web, or controller design | `docs/unity/spider-player.md` | Current spider, camera, input, spawn, prefab, and config owners |
| Replicated state, authority, RPCs, or peer-visible gameplay | Relevant feature policy only | Current networking implementation, if one exists |
| Local validation execution or tooling | `docs/workflows/validation.md` | `tools/validate.ps1` and generated result artifacts |
| CI, build, Pages, credentials, or deployment | `docs/systems/ci-cd.md` | Relevant workflow YAML |
| Documentation creation or restructuring | `docs/workflows/updating-docs.md` | Relevant source files and existing topic owner |
| Documentation audit | `docs/index.md` | Current docs and their repository authorities |
| Repository ownership or unfamiliar area | `docs/project-map.md` | The mapped authoritative directory |
| Platform-sensitive decision | `docs/systems/platform-strategy.md` | Current platform-specific code or workflow |
| Third-party assets or licenses | `THIRD-PARTY-ASSETS.md` | Asset provenance and upstream license |

## Target Task Workflow

1. Parse the request and classify the affected ownership boundary before loading broad context.
2. Use host skill metadata to load at most one primary skill body, then use the `AGENTS.md` route to load only the topic page needed for a non-obvious decision.
3. Search the authoritative repository path for the current owner, callers, serialized integration points, and adjacent tests.
4. Check whether the task requires serialized Unity Editor work and whether it can affect multiplayer authority or peer consistency.
5. Implement the narrowest correct change against current repository evidence.
6. Run the narrowest deterministic validation that can disprove the change.
7. Inspect the deterministic log and result files, fix failures, and rerun the failed check.
8. Review the diff for scope, accidental asset changes, missing tests, stale documentation impact, and conditional multiplayer implications.
9. Complete with the changed behavior, exact validation command and result, and exact Unity Editor follow-up that remains.

Until the shared validation runner is verified, agents must state that supported automated Unity validation is unavailable. They must not present generated `.csproj` commands, ignored Rider settings, archived logs, or an unrun Unity command as validation.

## Validation Selection

`tools/validate.ps1` is the target human and agent entrypoint. Its final mode names must be proven against the pinned Unity version before documentation or skills depend on them.

| Changed area | Narrowest starting check | Escalation |
| --- | --- | --- |
| `AGENTS.md`, docs, or skills | `Context` | None unless executable code also changed |
| Pure or near-pure C# with a targeted test | Filtered `EditMode` | `Compile`, then broader EditMode scope if needed |
| Authored C# without a relevant test | `Compile` | Relevant build or PlayMode smoke check when runtime integration changed |
| Scene, prefab, or config integration | `AssetIntegrity` | Targeted `PlayMode` smoke test and required Editor inspection |
| Startup, DI, scene loading, or runtime wiring | `PlayMode` with a focused smoke test | Relevant player build |
| Package, ProjectSettings, rendering, or platform behavior | Relevant tests plus target build | Deployment workflow only for release behavior |
| CI or validation tooling | Runner self-check plus affected mode | Validation workflow dispatch |

The runner contract is:

- accept `Context`, `Compile`, `EditMode`, `PlayMode`, and `AssetIntegrity` modes plus an optional test filter
- read the Unity version from `ProjectSettings/ProjectVersion.txt`
- resolve Unity from an explicit environment variable first, then a documented local fallback
- place generated results under ignored `.artifacts/validation/<mode>/`
- produce `summary.txt` for every mode, `Editor.log` for every Unity mode, and result XML for every test mode; local and CI adapters use the same names and pass/fail meaning
- preserve a failing process exit code
- make an empty or absent test suite visible rather than claiming it passed meaningful coverage
- work in Windows PowerShell 5.1 locally; the CI adapter must preserve the same mode and output semantics even if GameCI requires a different launch mechanism
- provide command help from the script so prose does not become the option reference

## Proposed Repository Tree

Only agentic context, documentation, and validation-related paths are shown. Existing game content is omitted.

```text
AGENTS.md
THIRD-PARTY-ASSETS.md
.gitignore

.opencode/
  skills/
    docs-auditor/
      SKILL.md
    docs-maintenance/
      SKILL.md
    skill-authoring/
      SKILL.md
    unity-architecture-review/
      SKILL.md
    unity-feature-implementation/
      SKILL.md
    unity-multiplayer-review/
      SKILL.md
    unity-refactor-aposd/
      SKILL.md
    unity-spider-player-implementation/
      SKILL.md
    unity-ui-flow/
      SKILL.md

docs/
  index.md
  project-map.md
  systems/
    ci-cd.md
    platform-strategy.md
  unity/
    runtime-architecture.md
    spider-player.md
  workflows/
    updating-docs.md
    validation.md

tools/
  validate.ps1

Assets/_Root/Scripts/Editor/Validation/
  SerializedAssetValidator.cs

Assets/_Root/Tests/
  EditMode/
    Pet.Tests.EditMode.asmdef
    <first deterministic tests>
  PlayMode/                 # created only when a concrete smoke test is justified
    Pet.Tests.PlayMode.asmdef
    <first startup or wiring smoke tests>

.github/workflows/
  validate.yml
  deploy-pages.yml

.artifacts/                 # ignored, generated locally
  validation/
```

The test folders and asmdefs are created with their first real tests, not as empty scaffolding. Unity creates and maintains all corresponding `.meta` files.

`docs/agent-audit/` is intentionally absent from the steady-state tree. Keep `01` through `04` only until the migration is accepted, then rely on Git history or the migration pull request for this evidence.

## Context Artifact Contracts

### Always-On Artifact

| Artifact | Why it exists | When it is loaded | What it contains | What it must not contain | How it stays current |
| --- | --- | --- | --- | --- | --- |
| `AGENTS.md` | It is the only repository-evidenced automatic instruction file and therefore the cheapest reliable document/authority router. It also owns cross-cutting constraints that materially affect edits. | Every task. Host skill metadata may be visible beside it but is measured separately. | Authority and precedence rules; rule to use a matching host-advertised skill; compact task-to-topic routing; non-obvious C# and Unity constraints; serialized-asset boundary; conditional multiplayer trigger; validation and completion contract. | Skill catalog; project overview; current scenes, packages, versions, or file inventories; generic senior-engineering advice; architecture tutorials; host tool instructions; detailed workflows; history. | Change only when a repository-wide constraint or topic-routing category changes. `Context` validation checks all links. Every added line must justify its cost on unrelated tasks. |

### Durable On-Demand Documents

| Artifact | Why it exists | When it is loaded | What it contains | What it must not contain | How it stays current |
| --- | --- | --- | --- | --- | --- |
| `docs/index.md` | Humans need one browsable documentation catalog; documentation audits need one coverage surface. | Human browsing, broad docs audit, or when a topic owner is unknown. Not normal task startup. | One-line purpose and link for each durable page; documentation scope. | Agent routing matrix; skill catalog; project summary; duplicated documentation rules; current inventories. | Update only when a durable page is added, renamed, or removed. Local links are checked by `Context` mode. |
| `docs/project-map.md` | Stable ownership boundaries can prevent repeated broad searches without mirroring the tree. | Repository restructuring or when the responsible area cannot be located directly. | Top-level repository ownership; stable authored roots; authority-by-fact mapping; links to topic owners. | File-by-file scripts, assets, scenes, prefabs, packages, versions, current wiring, feature status, or recent changes. | Update only when an ownership boundary changes. Referenced directories and links are mechanically checked. |
| `docs/systems/ci-cd.md` | CI intent, permission concepts, release boundary, and known operational failure modes are not fully expressed by YAML syntax. | CI, release, Pages, credentials, or deployment diagnosis. | Validation-versus-deployment responsibility; operational intent; credential and permission concepts; known failure diagnosis; links to workflow authorities. | Mirrored triggers, action versions, input values, paths, secret inventories, or historical logs. | Workflow YAML remains authority for exact configuration. Update this page only when intent, ownership, or failure handling changes; links are checked. |
| `docs/systems/platform-strategy.md` | Mobile-plus-PC direction and temporary WebGL status are product decisions not derivable from current automation. | Platform, performance, rendering, deployment-target, or long-term architecture tradeoffs. | The approved target-platform decision and rules for when temporary WebGL constraints apply. | Package inventory; CI details; speculative ECS, jobs, threading, or optimization strategy. | Change only after an explicit product-direction decision. Exact delivery facts are read from workflow YAML. |
| `docs/unity/runtime-architecture.md` | The bootstrap, composition, scene-entry, DI, config ownership, and ordered-initialization decisions are expensive to rediscover and easy to implement inconsistently. | Tasks that touch startup, VContainer scopes, additive scene flow, runtime spawning, or ordered initialization. | Compact current ownership model; startup sequence; scene-entry contract; config and spawn ownership; repository authority entrypoints; decision rationale where code alone is ambiguous. | General C# style; composition tutorial; all runtime files; UI or spider manuals; package/version facts; copied `AGENTS.md` rules. | Update in the same change that intentionally alters one of these architecture decisions. Current implementation details are linked, not mirrored; links are checked. |
| `docs/unity/spider-player.md` | The intentional rewrite state, preserved spawn/camera boundary, and single-player scope cannot be safely inferred from the minimal current code. | Spider locomotion, orientation, jump, web, camera-binding, or controller architecture work. | Preserved ownership boundary; intentionally undefined rewrite area; approved scope; design gate before new architecture; relevant authority directories. | Obsolete stage order; prescribed absent components; full file lists; removed-file catalog; stale prefab/config field inventory; unapproved roadmap. | Update whenever a spider architecture or scope decision is approved. The spider skill must link to it, and context link validation catches path drift. |
| `docs/workflows/updating-docs.md` | Documentation needs one procedure that decides whether information deserves persistence and where it belongs. | Documentation creation, restructuring, or a change to durable project intent. | Documentation-worthiness test; policy-owner versus repository-authority distinction; update/delete procedure; discoverability rule; `Context` verification; narrow criteria for creating a new page. | Page boilerplate; instruction to update milestones; multiple navigation-update lists; file inventories; rules copied into documentation skills. | It is the sole docs procedure. Change it only when the documentation lifecycle changes; docs skills link to it rather than repeat it. |
| `docs/workflows/validation.md` | Agents need check-selection, prerequisites, artifact locations, and failure handling after the runner is proven. | Running, diagnosing, or changing repository validation. Ordinary tasks can use the short command contract in `AGENTS.md` and script help. | Verified mode-selection matrix; environment prerequisites; output contract; failure inspection; CI relationship; exact limitations. | Unverified commands; Unity installation folklore; script implementation details; duplicated CLI option reference; claims based on archived logs. | Create only after the runner succeeds against the pinned Unity version. Script help and CI are executable authorities; documented examples are exercised in CI where feasible. |
| `THIRD-PARTY-ASSETS.md` | Attribution and license obligations are durable external facts not recoverable from authored code. | Importing, replacing, distributing, or reviewing third-party assets. | Asset identity, source, license, attribution, and evidence links. | General project map; package dependency inventory; agent instructions. | Update in the same change that adds, replaces, or removes a covered asset; verify against upstream license evidence. |

### On-Demand Skills

All target skills are tracked `SKILL.md` packages. In OpenCode, the name and description in frontmatter are baseline discovery metadata; the table's load trigger refers to loading the skill body. Descriptions must therefore be short, mutually distinct, and sufficient for routing without a catalog in `AGENTS.md`. References are added only when they contain substantial detail that would obscure the main workflow; the current short duplicated checklists are folded into their parent skill or deleted.

| Artifact | Why it exists | When it is loaded | What it contains | What it must not contain | How it stays current |
| --- | --- | --- | --- | --- | --- |
| `.opencode/skills/unity-feature-implementation/SKILL.md` | Routine Unity changes repeat the same ownership, Editor-follow-up, and validation sequence. | General Unity gameplay or scene-linked implementation that is not better handled by UI or spider specialization. | Focused implementation sequence; targeted repository discovery; editor-wiring decision; test selection; completion output. | `AGENTS.md` conventions; architecture manual; static paths; generic design slogans; a duplicate completion checklist. | Uses root policy by reference and the validation runner by command. Update only when this workflow changes. |
| `.opencode/skills/unity-architecture-review/SKILL.md` | Non-trivial design review benefits from consistent evidence-based boundary questions and findings format. | Architecture review, planning, or boundary evaluation. | Questions about ownership, coupling, volatility, Unity leakage, initialization, and the lightest useful boundary; findings-first output. | Mandatory architecture patterns; copied style rules; current component lists; implementation workflow. | Review prompts are justified against repository evidence. Trigger and near-miss prompts are checked when the skill changes. |
| `.opencode/skills/unity-refactor-aposd/SKILL.md` | Behavior-preserving refactors need a distinct workflow centered on reducing caller knowledge and change amplification. | Refactor requests involving leaky ownership, temporal coupling, shallow wrappers, or difficult `MonoBehaviour` code. | Diagnosis questions; compare-shapes step; behavior-preservation requirement; focused refactor output. | A broad architecture tutorial; rules copied from `AGENTS.md`; a separate tiny smells reference that repeats the body. | Keep examples generic and small; update only from repeated repository refactor failures, not one-off preferences. |
| `.opencode/skills/unity-multiplayer-review/SKILL.md` | Authority and desync review is a specialized cross-cutting check that should not tax unrelated tasks. | Networked code, replicated state, RPCs, ownership, peer-visible events, or a feature being evaluated for future multiplayer behavior. | Authority owner; host/client/server paths; synchronization route; duplicate-execution and desync checks; explicit single-player verdict. | Assumption that networking already exists; unconditional loading for docs/editor-only work; copied general Unity rules. | Align with the actual networking stack when one is adopted. Until then, report single-player assumptions rather than inventing infrastructure. |
| `.opencode/skills/unity-spider-player-implementation/SKILL.md` | Spider work has a project-specific preserved boundary and an intentional architecture gap that generic Unity guidance cannot safely handle. | Spider traversal, orientation, adhesion, jump, web, camera binding, or locomotion implementation. | Mandatory read of `docs/unity/spider-player.md`; current-owner inspection; design-approval gate when architecture is undefined; Editor follow-up; relevant validation. | Obsolete component names; stage order; deferred camera claim; stale asset inventories; invented multiplayer support. | Updated with each approved spider architecture decision. `Context` mode checks its links; source inspection remains mandatory. |
| `.opencode/skills/unity-ui-flow/SKILL.md` | UI implementation and review share one stable project taxonomy and ownership model; one skill avoids maintaining two copies. | Implementing, reviewing, or placing screens, popups, HUD, overlays, navigation, back flow, UI config, or UI prefab wiring. | Intent branch for review versus implementation; screen/popup/HUD/overlay decision table; shared-versus-scene ownership questions; DI/config/prefab inspection; Editor follow-up. | Static UI file inventory; copied global wiring rules; unsupported claims about current asset completeness; separate duplicated review and implementation manuals. | Inspect current UI, DI, config, prefab, and scene owners on every task. Update only when the stable UI primitive or ownership model changes. |
| `.opencode/skills/docs-maintenance/SKILL.md` | Writing, restructuring, and synthesizing durable docs are one workflow after milestone logging is removed. | User requests to document, restructure, or preserve a durable project decision. | Trigger-specific evidence retrieval; mandatory use of `updating-docs.md`; execution and `Context` validation; concise output. | Its own documentation-worthiness or owner-selection rules; separate page template; milestone requirement; project taxonomy copied from docs; current path inventories. | The workflow page is the sole procedural authority. The skill stays trigger-focused and changes only when invocation or output behavior changes. |
| `.opencode/skills/docs-auditor/SKILL.md` | Auditing is a different job from writing and benefits from a findings-first coverage/staleness/overlap checklist. | Documentation audit, staleness review, retrieval review, or context-efficiency assessment. | Scope selection; evidence verification; overlap and stale-risk checks; findings-first output; deletion preference. | Mandatory reading of every map; milestone coverage check; broad project inventory; duplicated docs-maintenance procedure. | Audit against current repository paths. Package links and frontmatter are checked mechanically; audit judgment remains evidence-based. |
| `.opencode/skills/skill-authoring/SKILL.md` | The tracked skill library needs one occasional workflow for creation and review instead of overlapping creator and tester packages. | Creating, porting, reviewing, or changing a local skill. | OpenCode package shape; trigger boundary; direct/indirect/near-miss prompt checks; resource-worthiness test; context validation. | Generic host tool documentation; two separate review rubrics; unused `references/`, `scripts/`, or `assets/`; Unity policy. | `Context` mode verifies package shape, frontmatter, names, and links. Trigger behavior is manually tested with realistic prompts when changed. |

### Temporary Audit Artifacts

| Artifact | Why it exists | When it is loaded | What it contains | What it must not contain | How it stays current |
| --- | --- | --- | --- | --- | --- |
| `docs/agent-audit/01-current-state.md` through `04-target-design.md` | They provide evidence and rationale for this finite migration. | Only while implementing or reviewing the migration. | Inspection date and revision, observed problems, target decisions, and migration order. | Routine task routing, current feature documentation, or indefinite maintenance obligations. | They are immutable snapshots, not living docs. Correct material errors during migration, then delete the directory after acceptance and use Git history for provenance. |

## Executable Artifact Contracts

These artifacts reduce prose and failed iterations. They are executed or inspected only when relevant, not preloaded as context.

| Artifact | Why it exists | When used | What it contains | What it must not contain | How it stays current |
| --- | --- | --- | --- | --- | --- |
| `tools/validate.ps1` | One discoverable local command eliminates repeated Unity-command rediscovery and standardizes outputs. | After relevant changes and while diagnosing validation. | Mode parsing, Unity resolution, process invocation, deterministic output paths, exit propagation, and self-help. | Business logic; duplicated CI YAML; hard-coded user paths; claims of support for an unverified mode. | Every mode is run against the pinned Unity version before release. CI exercises shared semantics; script help is the CLI authority. |
| `SerializedAssetValidator.cs` | Compile checks cannot catch missing scripts and broken authored references that Unity exposes through its serialization APIs. | `AssetIntegrity` mode and relevant CI validation. | Read-only Editor scanning of `Assets/_Root/Scenes/`, `Assets/_Root/Prefabs/`, and `Assets/_Root/Configs/` for missing `MonoBehaviour` scripts and broken current object-reference properties exposed by Unity; concise failure output. | Runtime repair hacks; automatic asset mutation; raw-YAML claims about unknown former fields; failure on harmless old type-identifier text alone; broad style checks. | Covered by focused EditMode tests where practical and run in CI. New checks require a demonstrated defect and low false-positive risk. |
| `Pet.Tests.EditMode` | Fast deterministic tests provide the highest-value early regression loop. | Logic or editor validation changes. | Tests for pure or near-pure authored behavior and validation helpers. | Empty asmdef; broad scene startup tests; tests coupled to ignored generated projects. | Created with the first real tests; filtered and full runs execute in CI. |
| `Pet.Tests.PlayMode` | A few integration checks can prove startup, DI, scene entry, and critical wiring that compilation cannot. | Changes to runtime integration boundaries, after a concrete deterministic smoke case exists. | Narrow smoke tests with explicit setup and deterministic cleanup. | Empty suite; broad gameplay simulation; flaky timing-based coverage added only to increase counts. | The package is absent until the first useful smoke test is identified and created with its asmdef; runtime and flake rate are monitored before expansion. |
| `.github/workflows/validate.yml` | Normal branches need feedback independent of deployment. | Pull requests, pushes to `dev`, manual diagnosis, and exact-commit calls from deployment. | An always-safe context job; Unity jobs for trusted same-repository events with license access; normalized exit/results/artifacts; failure summary; `workflow_call` entrypoint for deployment. | `pull_request_target` execution of untrusted code with secrets; Pages deployment; duplicated prose configuration; dependence on ignored local files; passing empty tests as coverage. | Workflow YAML is self-authoritative. Fork or Dependabot changes without secrets receive context validation only. Unity status remains unavailable until the reviewed commit is pushed to a trusted same-repository branch, preserving the commit or exact candidate tree, and validation runs there. Local and GameCI adapters must produce the same named outputs and pass/fail meaning. |
| `.github/workflows/deploy-pages.yml` | Deployment remains a distinct release concern. | WebGL deployment only. | A reusable-workflow call that validates the exact caller commit, followed by WebGL build and Pages deployment with `needs` enforcing success. | General PR quality-gate implementation; test documentation; archived log retention as project context. | Exact behavior stays in YAML; `ci-cd.md` records only intent and operational failure knowledge. |

## Deletion And Consolidation

The following removals are part of the design, not optional cleanup.

| Current artifact | Target action | Unique information handling |
| --- | --- | --- |
| `docs/ai/assistant-entrypoint.md` | Delete | Move the compact task route into `AGENTS.md`, where loading is reliable. |
| `docs/ai/retrieval-map.md` | Delete | Discard the fixed broad retrieval order; `AGENTS.md` routes directly. |
| `docs/ai/coding-skills.md` | Delete | Skill frontmatter and the root route own discovery. |
| `docs/ai/external-agent-guidance.md` | Delete | Preserve no prompt text; compatible project rules already have local owners. |
| `docs/ai/` | Remove directory when empty | No separate agent-navigation layer remains. |
| `docs/unity/unity-composition-guide.md` | Delete | It contains no unique Pet decision and conflicts with local conventions. |
| `docs/unity/project-structure.md` | Delete after merge | Move only stable Unity folder ownership into the trimmed `project-map.md`; derive everything else. |
| `docs/unity/runtime-architecture-guidelines.md` | Replace with `runtime-architecture.md` | Preserve only startup, composition, scene-entry, config, spawn, and ordered-initialization decisions. |
| `docs/unity/spider-player-controller-plan.md` | Replace with `spider-player.md` | Preserve the rewrite intent and spawn/camera boundary; delete obsolete or volatile lists. |
| `docs/history/milestones.md` | Inventory unique rationale, extract only still-operative decisions, then delete | Verify each unique rationale against current code, user direction, or Git history. Move only durable operative intent to a topic owner; Git stores the remaining chronology. |
| `docs/history/` | Remove directory when empty | No routine historical context remains. |
| `docs/agent-audit/` | Delete after migration acceptance | The migration commit or pull request retains provenance. |
| `ForAgents/logs_*/` and all references | Remove by default | Delete the ignored local archives after confirming there is no explicit human archive requirement. Any opted-in archive must identify its workflow run and commit and must remain outside agent routing. |
| `docs-architect`, `docs-writer`, `knowledge-synthesizer` | Replace with `docs-maintenance` | Keep evidence-based writing and placement; remove milestone and repeated navigation rules. |
| `docs-auditor` references | Delete redundant checklist | Keep the distinct audit skill with a focused body. |
| `skill-creator`, `skill-tester` | Replace with `skill-authoring` | Keep package and trigger validation once, not in overlapping skills and rubrics. |
| `unity-ui-flow-implementation`, `unity-ui-flow-review` | Replace with `unity-ui-flow` | Keep one UI taxonomy and branch workflow by review versus implementation intent. |
| `unity-feature-implementation/references/checklist.md` | Delete | Its content repeats the parent skill and root rules. |
| `unity-refactor-aposd/references/smells-and-moves.md` | Fold any useful prompts into the skill, then delete | Twenty-one lines do not justify a second retrieval artifact. |
| Other short duplicated skill references | Fold or delete | Retain a reference only when it materially reduces the main skill without repeating it. |
| Local `.opencode/package*.json`, `node_modules`, and self-ignored `.opencode/.gitignore` | Exclude from the shared target unless a real shared dependency is adopted | Track skills only. Put any deliberate local ignores in root `.gitignore`; do not describe local package state as project infrastructure. |

## Explicit Non-Additions

### Nested `AGENTS.md`

Do not add one. A nested file is justified only by a stable subtree-specific constraint that narrows or conflicts with root policy and must be automatic. Local task routing belongs in a skill, not another always-loaded rule layer.

### Root `README.md`

Do not add an agent-facing README. `docs/index.md` serves human documentation discovery, and no verified human setup/run command currently exists to justify a separate onboarding page. Add a README later only for a real external human audience with unique onboarding needs.

### Generated Repository Documentation

Do not generate file, asset, scene, package, or class inventories. The project is small and targeted search is cheaper to produce and consume. Generate a map only if measured task traces show repeated expensive discovery that a compact generated artifact would actually eliminate.

### Generic Decision Record Framework

Do not add an ADR directory or template. Put a durable decision in its narrow topic document. Reconsider ADR infrastructure only after multiple decisions cannot be owned cleanly by existing topics.

### Task Templates

Do not add task or pull-request templates for agents. The root completion contract and task skills already define required outputs without forcing every request into the same shape.

### Codex Copies

Do not duplicate `.opencode/skills/` under a Codex-specific directory. `AGENTS.md` and `docs/` remain tool-neutral. Add a second packaging adapter only after Codex is actually adopted, its discovery behavior is verified, and shared source can prevent drift.

### MCP Or Host Tool Prompts

Do not add repository-local MCP configuration or copies of host tool instructions without a concrete shared integration. The repository cannot keep unknown host behavior current.

### Validation Skill

Do not add one. Validation is a command contract, not a separate agent job. Existing implementation skills invoke the runner; `validation.md` owns troubleshooting and selection detail.

### Generated Changelog Or Milestone Log

Do not replace `milestones.md` with another chronology. Git history and release artifacts own history; topic pages own current intent.

### Analyzer And Formatter Stack For Now

Do not add analyzers, `.editorconfig`, or a formatter as part of the context migration. After the compile runner is stable, inspect which Unity/Roslyn analyzers actually execute and add the smallest deterministic policy only if repeated defects justify it. A formatter requires an explicit team choice and CI-verifiable version before adoption.

## Ordered Migration Plan

1. **Freeze the evidence boundary.** Record the inspected commit in `01` through `04`, apply the validated corrections above, and keep the audit directory out of normal navigation.
2. **Define acceptance measurements before edits.** Record current `AGENTS.md` size, total host-advertised skill metadata size, typical loaded skill bodies and document read sets for one Unity, UI, spider, CI, and docs task, and current broken-link count. Measure again after migration; do not build telemetry infrastructure.
3. **Rewrite root `AGENTS.md` first.** Make it the sole document/authority router, defer skill matching to host-advertised descriptions, add authority precedence, retain only high-value project constraints, make multiplayer review conditional, and define the validation completion contract. Keep links to old targets temporarily until their replacements exist.
4. **Create the compact durable docs.** Trim `project-map.md`; replace the runtime and spider pages; narrow CI/CD, platform, docs workflow, and index pages to their artifact contracts. Do not create `validation.md` yet.
5. **Remove duplicate documentation.** Delete `docs/ai/`, `unity-composition-guide.md`, `project-structure.md`, and all backlinks. Inventory unique milestone rationale, verify it against current evidence, extract only still-operative decisions into topic owners, then delete `docs/history/`. Delete ignored `ForAgents/logs_*` unless the user explicitly retains a commit-linked human archive.
6. **Consolidate the skill library.** Create `docs-maintenance`, `skill-authoring`, and `unity-ui-flow`; sharpen retained Unity skills; correct the spider skill; delete superseded skills and redundant references. Ensure each trigger has direct, indirect, and near-miss examples used during review, not stored as routine context unless they remain necessary.
7. **Add context validation.** Implement the non-Unity `Context` mode to check Markdown links, root route targets, skill frontmatter, skill folder/name agreement, and bundled references. Add deterministic output under `.artifacts/` and root ignore rules.
8. **Prototype the Unity runner before documenting it.** Verify `Compile`, absent-suite reporting, deterministic output paths, and success/failure exit behavior against Unity `6000.5.3f1`. Resolve local-versus-GameCI launch differences behind the normalized output contract. Do not advertise a mode that has not failed and succeeded predictably.
9. **Add asset-integrity validation against the known failure.** Implement the read-only validator for the exact project-owned roots, run it while the missing `GameplayTester` component still exists, prove that it fails for the expected reason, and check false-positive behavior before exposing `AssetIntegrity`.
10. **Clean the known serialized baseline in the Unity Editor.** Remove the missing `GameplayTester` component, rerun `AssetIntegrity` to prove a clean result, and change other old spider/config serialization only when Unity inspection demonstrates a concrete current defect. This requires user-authorized Unity asset edits; do not patch YAML or `.meta` files manually.
11. **Add and verify the first real EditMode tests.** Create the EditMode asmdef with deterministic tests for existing high-value logic or validation helpers, then verify filtered and full `EditMode` behavior. Delete or relocate `MainMenuTester.cs` separately only after confirming it has no authored use.
12. **Add PlayMode only with a concrete smoke case.** If a deterministic startup, scene-entry, DI, or critical wiring test is identified, create it with the PlayMode asmdef and verify filtered and full behavior. Otherwise leave the package and mode unavailable rather than adding an empty suite.
13. **Add non-deployment CI with an explicit trust topology.** Make `validate.yml` support `pull_request`, pushes to `dev`, `workflow_dispatch`, and `workflow_call`. Run context checks without secrets; run Unity only for trusted events with license access; never execute untrusted fork code under `pull_request_target`. To validate an external contribution with Unity, push the reviewed commit to a trusted same-repository branch while preserving the commit or exact candidate tree, then run validation there. Upload normalized logs/XML/summary. Have `deploy-pages.yml` call validation for the exact commit and gate build through `needs`.
14. **Create `docs/workflows/validation.md`.** Document only commands and outputs proven by steps 8 through 13. Update `AGENTS.md` and relevant skills to invoke the verified runner contract.
15. **Run migration validation.** Run `Context`, the available Unity modes, a broken-link search, and a repository search for every deleted path and obsolete spider component instruction. Review the full diff and current serialized-asset status.
16. **Measure and prune again.** Compare the representative task read sets from step 2. Remove any new artifact that does not reduce repeated search, preserve a durable decision, or improve deterministic feedback.
17. **Retire the audit package.** After the target is accepted and the migration is in Git history, delete `docs/agent-audit/` and remove any remaining references to it.

## Migration Gates

The migration is complete only when all of these statements are true:

- Root `AGENTS.md` is the only always-loaded repository instruction file and the only document/authority router; host-advertised skill metadata is measured but not duplicated there.
- A focused task reaches current source after no more than one primary skill body and one topic page in the normal case.
- No maintained prose document inventories current scripts, assets, scenes, prefabs, packages, versions, workflow inputs, or serialized wiring.
- `docs/ai/`, `docs/history/`, external prompt/tutorial pages, redundant skill references, and obsolete spider workflow text are gone.
- Every durable context artifact has one owner and one explicit load trigger.
- All context links and skill packages pass deterministic `Context` validation.
- A verified local command produces a meaningful exit code plus deterministic logs for compilation and available test modes.
- CI validates ordinary changes independently from Pages deployment and retains useful failure evidence.
- Missing scripts and Unity-exposed broken current object references under `Assets/_Root/Scenes/`, `Prefabs/`, and `Configs/` are checked automatically; raw stale YAML and unavoidable Inspector work remain explicitly outside that proof.
- No test assembly exists without at least one useful test.
- No generated project map, generic ADR framework, task-template system, validation skill, Codex copy, MCP prompt layer, or replacement milestone log has been added.
- The temporary audit package is no longer part of the steady-state tree.

## Expected Result

The common task path becomes:

```text
request
  -> AGENTS.md plus host skill metadata
  -> one skill body when useful
  -> AGENTS.md topic route
  -> one durable topic page only when intent matters
  -> targeted repository evidence
  -> implementation
  -> deterministic validation
  -> diff review
  -> exact result and Editor follow-up
```

This removes context whose only value is reproducing searchable repository state, while preserving the small amount of project intent and repeated procedure that materially improves agent decisions. The largest reliability gain comes from replacing prose verification expectations with executable compile, test, and serialized-integrity feedback.

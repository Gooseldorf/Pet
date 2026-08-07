# Agentic Workflow Audit

## Purpose And Scope

This audit evaluates the end-to-end workflow for a typical coding-agent task in this repository:

`USER REQUEST -> context discovery -> documentation discovery -> code exploration -> implementation -> Unity validation -> tests -> error inspection -> self-review -> completion`

It extends `01-current-state.md` and `02-context-audit.md`. Findings are based on the tracked repository as inspected on 2026-08-07. No Unity Editor, build, or test run was started for this audit, so this page distinguishes observed capability from verified local execution.

Classification labels identify the primary failure source:

- **A. CONTEXT PROBLEM**: the agent loads too much, too little, or conflicting persistent context.
- **B. TOOLING PROBLEM**: a repeatable command, automation, or machine-readable result is missing.
- **C. DOCUMENTATION PROBLEM**: maintained guidance is stale, ambiguous, or incorrectly treated as an authority.
- **D. WORKFLOW PROBLEM**: ownership, ordering, or completion criteria permit avoidable failed iterations.
- **E. NOT WORTH FIXING**: the apparent gap should not be solved because cost, project size, or intentional boundaries outweigh the benefit.

## Evidence Summary

| Area | Current capability | Evidence |
| --- | --- | --- |
| Unity version | Pinned to Unity `6000.5.3f1` | `ProjectSettings/ProjectVersion.txt` |
| Compilation | A WebGL build compiles the project, but only on a push to `WebGLBuild` or manual dispatch | `.github/workflows/deploy-pages.yml` |
| EditMode tests | Test Framework package is installed; no authored test assembly or test attributes were found | `Packages/manifest.json`, `Assets/_Root/Scripts/Test/MainMenuTester.cs` |
| PlayMode tests | No authored test assembly or tests; play-mode test runner is disabled | `ProjectSettings/ProjectSettings.asset` (`playModeTestRunnerEnabled: 0`) |
| Targeted tests | No shared command, test filter, or test-result contract | repository search; only an ignored Rider workspace contains a local EditMode invocation |
| Batchmode | Used internally by `game-ci/unity-builder`; no tracked local wrapper exposes it | `.github/workflows/deploy-pages.yml` |
| Failure evidence | CI uploads the WebGL Pages artifact only; no Editor log, JUnit XML, coverage, or error summary is retained | `.github/workflows/deploy-pages.yml` |
| Static analysis and formatting | No tracked analyzer configuration, formatter configuration, or CI check | repository configuration search |
| Agent workflows | Thirteen on-demand skills provide useful task routing; they are Markdown-only and cannot execute validation | `.opencode/skills/*/SKILL.md` |
| Repository maps | Manually maintained maps exist and have drifted from the current asset tree | `docs/project-map.md`, `docs/unity/project-structure.md` |

Generated `*.csproj` and `*.sln` files must not be treated as portable validation infrastructure. They are ignored by `.gitignore`, and the generated test projects point to absent test assembly definitions.

## Lifecycle Audit

| Lifecycle stage | Current behavior | Weakness | Classification | Smallest effective change |
| --- | --- | --- | --- | --- |
| User request | `AGENTS.md` supplies global constraints and links to several documentation entrypoints. | The baseline mixes enforceable conventions with broad engineering preferences, and it routes agents to overlapping maps. | A | Keep the root rules to durable, project-specific constraints; keep one compact task router. |
| Context discovery | `assistant-entrypoint.md`, `retrieval-map.md`, `coding-skills.md`, `index.md`, and `project-map.md` all route agents. | Agents must select among overlapping route maps. `retrieval-map.md` also prescribes reading the broad project map before topic-specific evidence. | A | Make `docs/ai/assistant-entrypoint.md` the only agent router and route directly by task. |
| Relevant documentation discovery | Current maps contain detailed inventories, historical references, external guides, and architecture guidance. | Routing can load stale inventories or obsolete spider guidance before current source. Multiple pages duplicate the same policy. | A, C | Keep durable decisions in on-demand pages; derive volatile paths, package versions, assets, and workflow values from targeted repository reads. |
| Code exploration | The repository is small and has stable authored ownership under `Assets/_Root/`. | Long file inventories duplicate what a targeted glob or content search can establish, then drift. `project-map.md` and `project-structure.md` already omit current camera, HUD, and config assets. | C | Reduce maps to ownership boundaries and authoritative directories. Do not replace them with another manually maintained inventory. |
| Implementation | Distinct feature, UI, spider, architecture, refactor, multiplayer, and documentation skills are available. | Skills repeat baseline rules and ask for "the most relevant feasible verification" without a command to select or run. The spider skill directs work toward removed components and defers camera integration that now exists. | A, C, D | Retain distinct skills, remove copied baseline rules, correct the spider workflow, and have implementation skills call one executable validation entrypoint. |
| Unity validation | A successful `game-ci/unity-builder` WebGL build gives compilation feedback. | The build is a deployment workflow, not a normal change gate. It is unavailable as a documented local command and runs only after pushing the deployment branch or manually dispatching CI. | B, D | Create and verify a tracked batch-mode validation runner, then invoke it in a non-deployment CI validation job. |
| Tests | Unity Test Framework is installed. `MainMenuTester.cs` is not a formal test. | There are no test asmdefs, EditMode tests, PlayMode tests, filters, result files, or CI test job. | B | Establish test assemblies and a runner contract; begin with high-value deterministic EditMode tests, then add narrow PlayMode smoke coverage. |
| Error inspection | The deployment job exposes action output during its run. Historical `ForAgents` logs are ignored and may describe old workflow revisions. | Agents have no stable output location or machine-readable results after a failure. Historical logs are not trustworthy current evidence. | B | Publish Editor logs, test XML, and a concise error summary from the validation job. Do not route agents to archived logs. |
| Self-review | Skills request a verification statement and multiplayer consideration. | Completion is prose-driven: no required command output, changed-file review convention, or test selection rule proves the claimed verification is sufficient. | D | Require a validation result plus a changed-file/diff review; select the narrowest relevant check before a broad build. |
| Completion | Skills correctly require explicit Unity Editor follow-up for serialized wiring. | A code-only agent cannot independently prove Inspector wiring or runtime behavior when the task needs scene/prefab edits. | E | Retain the explicit Editor follow-up boundary. Add automated scene/prefab reference checks only after the test harness exists; do not bypass authoring with runtime lookup hacks. |

## Feedback Loop Assessment

### Compilation And Batchmode

The existing WebGL deployment build is useful proof that a Unity compilation path can run in CI. It is insufficient as the normal agent feedback loop because its trigger is limited to `WebGLBuild`, it deploys after building, and it exposes no local, targeted, or artifact-backed invocation.

**B. TOOLING PROBLEM**: create one tracked Unity batch-mode runner, preferably a PowerShell script because the working environment is Windows. It should:

- resolve the Unity executable from an explicit environment variable or a documented version-derived location;
- accept a mode for compile-only, EditMode, PlayMode, and a test filter;
- write each run's Editor log and test XML to a deterministic ignored output directory;
- return Unity's exit code unchanged; and
- be verified against this exact Unity version before it is named in any documentation or skill.

The runner is the authority. A short validation runbook may link to it after verification, but a prose command matrix without an executable wrapper would create another stale source.

### EditMode, PlayMode, And Targeted Tests

**B. TOOLING PROBLEM**: `com.unity.test-framework` is present, but no authored test assemblies exist. The ignored generated `Pet.Tests.EditMode.csproj` and `Pet.Tests.PlayMode.csproj` reference missing `Assets/_Root/Scripts/Test/EditMode/` and `PlayMode/` asmdefs, so they are not evidence of a usable suite.

Create formal test asmdefs only when the first tests are ready. Start with deterministic EditMode tests around pure or near-pure authored behavior. Add a small PlayMode smoke suite for scene startup, DI registration, and prefab/scene reference validation only where it catches real integration failures. Use category or namespace filters in the runner so an agent can execute the narrowest relevant test set rather than always running every test.

**E. NOT WORTH FIXING**: enabling PlayMode tests now, without authored tests or a runner, would only make an empty suite appear healthier. Do not add broad PlayMode coverage merely to claim coverage.

### CI, Logs, And Error Inspection

**D. WORKFLOW PROBLEM**: `.github/workflows/deploy-pages.yml` is a release/deployment workflow, not a pull-request quality gate. The tracked repository provides no Unity compile, test, or analysis result for normal change branches; GitHub branch-protection configuration is outside this repository and was not assessed.

After the runner is verified, add a separate validation workflow or validation job triggered by pull requests and the integration branch. It should run the appropriate compile and EditMode checks before any deployment concern. Add PlayMode coverage when the suite has meaningful tests and its runtime cost is known.

**B. TOOLING PROBLEM**: CI should upload the Unity Editor log and test-result XML on failure, and publish an error-only summary. This gives agents concrete failure evidence without asking them to search ignored `ForAgents/` archives. Retaining historical raw logs as a retrieval source is not a substitute.

### Static Analysis And Formatting

**B. TOOLING PROBLEM**: generated project files incidentally reference Unity analyzers, but they are ignored, machine-specific, and not run in CI. No tracked `.editorconfig`, analyzer policy, formatter, or formatting check exists.

Do not begin by adding multiple style tools. First decide whether a small `.editorconfig` plus the Unity/Roslyn analyzers already available in the verified CI compile can enforce the highest-value conventions. Introduce a formatter only if the team chooses an exact formatter and can run it deterministically in CI. This is lower priority than executable compilation and tests because style failures currently pose less gameplay risk than unobserved Unity failures.

### Generated Maps, Indexes, Skills, And Reusable Workflows

**C. DOCUMENTATION PROBLEM**: the manually curated detailed maps are stale. The correct remedy is to narrow them to stable ownership and use targeted repository discovery for volatile facts.

**E. NOT WORTH FIXING**: do not add a generated full repository map or index yet. The project is small, `glob` and content search already retrieve a subsystem precisely, and a generated inventory would still consume context if loaded indiscriminately. Revisit only if agent task logs show repeated expensive searches for the same stable relationships.

**A. CONTEXT PROBLEM**: the existing thirteen skills are a good reusable-workflow boundary, but their bodies repeat `AGENTS.md`, runtime guidance, and static file lists. Retain task-specific skills because their triggers differ; make each contain only task-specific retrieval, decisions, and completion criteria.

**E. NOT WORTH FIXING**: do not add a separate validation skill before there is a runnable validation entrypoint. Once the runner exists, the current Unity implementation skills can direct agents to it; another skill would add routing overhead without a distinct job.

## Cross-Cutting Weaknesses

| Weakness | Classification | Why it causes failed iterations | Recommended resolution |
| --- | --- | --- | --- |
| Five overlapping agent navigation surfaces | A. CONTEXT PROBLEM | Agents read duplicate maps, choose conflicting sources, and grow context before they know the task boundary. | One short entrypoint routes directly to task-specific policy, skill, and repository authority. |
| Global conventions are duplicated in runtime docs and skills | A. CONTEXT PROBLEM | Equivalent rules are loaded repeatedly and can drift into contradictory wording. | Keep global constraints in `AGENTS.md`; skills reference rather than reproduce them. |
| Fixed broad retrieval order | A. CONTEXT PROBLEM | A focused CI, UI, or documentation task pays for irrelevant project-map context. | Route from task type to exact authority, then inspect only related files. |
| Static source, asset, package, and workflow inventories | C. DOCUMENTATION PROBLEM | They become stale and can override a current targeted search with false confidence. | Keep stable ownership only; read YAML, manifests, asset folders, and source on demand. |
| Obsolete spider roadmap and skill instructions | C. DOCUMENTATION PROBLEM | Spider work can be implemented against deleted components and incorrect camera assumptions. | Align the plan and skill with the preserved current boundary before additional spider work. |
| No supported Unity validation command | B. TOOLING PROBLEM | Agents cannot select, execute, or report a repeatable check; each task requires rediscovery of local setup. | Add and verify a shared runner before writing instructions around it. |
| No formal tests despite the installed framework | B. TOOLING PROBLEM | Regressions in deterministic behavior and scene integration lack fast feedback. | Add test asmdefs with the first high-value tests, then grow targeted coverage. |
| Deployment workflow is the only CI Unity execution | D. WORKFLOW PROBLEM | Quality feedback arrives late, may require deployment credentials, and is not required for ordinary changes. | Add a non-deployment PR/integration validation workflow. |
| CI produces no retained test/log/error evidence | B. TOOLING PROBLEM | A failed iteration leaves agents with incomplete, transient diagnosis data. | Upload logs and XML and generate a failure summary. |
| Skills require verification but lack selection criteria | D. WORKFLOW PROBLEM | Agents can accurately state that validation was infeasible without revealing that no appropriate check exists. | Use runner modes and changed-area rules to require the narrowest meaningful check. |
| Human scene/prefab wiring is unavoidable for code-only changes | E. NOT WORTH FIXING | The restriction prevents unsafe serialized-asset edits and runtime workaround hacks. | Preserve the boundary; give exact Editor follow-up and later automate only inspectable reference checks. |
| No MCP configuration or host-tool prompt files | E. NOT WORTH FIXING | Repository-local copies would not reliably configure the host and would increase maintenance. | Keep host tooling outside repository documentation unless a shared, tracked integration is actually adopted. |

## Target Workflow

The following order minimizes context and maximizes feedback without requiring broad documentation reads:

1. Read `AGENTS.md` and the compact agent router.
2. Classify the task as documentation, CI, general Unity, UI, spider, multiplayer-sensitive, refactor, or review.
3. Load only the matching skill and policy page, if one exists.
4. Use a targeted path/content search to identify the current owner and adjacent tests; treat source, assets, manifests, and workflow YAML as the authority for volatile facts.
5. Implement the narrowest correct change and identify any serialized Unity Editor follow-up.
6. Run the narrowest runner mode that covers the changed behavior: filtered EditMode test, filtered PlayMode test, compile-only, then broad build only when relevant.
7. Inspect the deterministic log/result output. Fix failures before continuing.
8. Review the diff for scope, conventions, authority/multiplayer implications when applicable, and missing tests.
9. Complete with the behavior change, the exact validation result, and any unavoidable Unity Editor work.

Until the runner and tests exist, completion should state that supported automated validation is unavailable. It should not imply that an unrun local Unity command or archived log verified the change.

## Prioritized Top 10

Scores use `impact on output quality (1-5) x task frequency (1-5) / implementation cost (1-5)`. They are relative prioritization estimates; dependency order breaks ties.

| Rank | Action | Primary classification | Impact x frequency / cost | Score | Reason |
| --- | --- | --- | --- | --- | --- |
| 1 | Make `docs/ai/assistant-entrypoint.md` the sole compact agent router; remove fixed broad retrieval routing. | A | 4 x 5 / 1 | 20.0 | Removes unnecessary reads on nearly every task at very low cost. |
| 2 | Add and verify one tracked Unity batch-mode validation runner with compile, EditMode, PlayMode, and filter modes. | B | 5 x 5 / 2 | 12.5 | Converts uncertain, repeated local discovery into a reliable feedback loop. |
| 3 | Add a non-deployment PR/integration CI validation workflow that invokes the runner. | D | 5 x 4 / 2 | 10.0 | Creates a repository-defined quality gate and separates feedback from Pages deployment. |
| 4 | Remove duplicate global rules from skills and runtime guidance; retain only task-specific skill content. | A | 4 x 5 / 2 | 10.0 | Reduces recurring context growth and future policy drift. |
| 5 | Correct the spider plan and spider skill to the present rewrite baseline and camera boundary. | C | 5 x 2 / 1 | 10.0 | Prevents high-cost implementation against deleted architecture. |
| 6 | Publish Unity Editor logs, JUnit XML, and a concise failure summary from CI. | B | 4 x 4 / 2 | 8.0 | Shortens diagnosis and prevents dependence on stale archived logs. |
| 7 | Replace detailed manual maps with stable ownership maps and targeted repository discovery. | C | 4 x 4 / 2 | 8.0 | Eliminates stale inventory reads while preserving useful navigation. |
| 8 | Establish formal EditMode test asmdefs and add the first deterministic, high-value tests. | B | 5 x 3 / 2 | 7.5 | Creates fast regression feedback where Unity scene startup is unnecessary. |
| 9 | Add a small `.editorconfig` and run the selected analyzer configuration in CI, after the compile runner is stable. | B | 3 x 4 / 2 | 6.0 | Automates high-value convention checks without prematurely adopting a formatter stack. |
| 10 | Add narrow PlayMode smoke/reference tests for critical startup and authored wiring after the test harness proves useful. | B | 5 x 2 / 3 | 3.3 | Reduces human wiring mistakes, but has higher runtime and setup cost than EditMode coverage. |

## Explicit Non-Recommendations

- Do not add more documentation to compensate for the absence of executable Unity validation; build and verify the runner first.
- Do not create a generated full repository inventory while targeted discovery is cheap and the project remains small.
- Do not add a validation skill until it owns a workflow distinct from invoking the shared runner.
- Do not use ignored generated `.csproj` files as a `dotnet build` or `dotnet test` contract.
- Do not turn agent-side serialized-asset restrictions into runtime lookup workarounds to eliminate Unity Editor follow-up.
- Do not route agents to `ForAgents/` logs as current build evidence.

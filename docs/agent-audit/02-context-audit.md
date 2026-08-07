# Context-Efficiency Audit

## Scope And Method

This audit follows `01-current-state.md` and inspects the repository's persistent agent context. It does not assess gameplay or code architecture except to verify whether documented facts still match the repository.

Classification describes the intended retrieval behavior after the recommended cleanup, not automatic loading behavior in the current OpenCode environment. The repository currently has one root `AGENTS.md` and no nested `AGENTS.md` files.

## Classification Inventory

| Persistent context | Classification | Audit disposition |
| --- | --- | --- |
| `AGENTS.md` | ALWAYS LOAD | Retain only non-obvious project constraints and asset-editing boundaries. |
| Nested `AGENTS.md` files | DERIVE FROM REPOSITORY | None exist; do not add them without a genuinely local rule set. |
| `docs/ai/assistant-entrypoint.md` | ALWAYS LOAD | Make this the one compact task router; remove repeated project facts. |
| `docs/index.md` | LOAD ON DEMAND | Keep as the human documentation index, linked from the entrypoint. |
| `docs/ai/retrieval-map.md` | REMOVE | Duplicates the entrypoint's routing role and prescribed retrieval order. |
| `docs/ai/coding-skills.md` | REMOVE | Duplicates skill descriptions and routing already held by skill frontmatter. |
| `docs/project-map.md` | LOAD ON DEMAND | Retain source-of-truth ownership and stable top-level locations; remove detailed inventories. |
| `docs/unity/project-structure.md` | LOAD ON DEMAND | Retain layout conventions only; derive current assets and scripts from the tree. |
| `docs/unity/runtime-architecture-guidelines.md` | LOAD ON DEMAND | Retain Unity-specific guidance not in `AGENTS.md`; remove copied global rules and volatile inventories. |
| `docs/unity/spider-player-controller-plan.md` | LOAD ON DEMAND | Retain the current baseline and rewrite boundary; remove obsolete staged implementation prescriptions. |
| `docs/unity/unity-composition-guide.md` | REMOVE | Archived external tutorial conflicts with local conventions and is unnecessarily large. |
| `docs/ai/external-agent-guidance.md` | REMOVE | Archived external prompt is contradictory, generic, and explicitly superseded. |
| `docs/systems/ci-cd.md` | LOAD ON DEMAND | Retain operational intent and known failure modes; derive exact workflow settings from YAML. |
| `docs/systems/platform-strategy.md` | LOAD ON DEMAND | Retain the product-direction decision; remove duplicated workflow facts. |
| `docs/workflows/updating-docs.md` | LOAD ON DEMAND | Keep as the sole documentation-maintenance procedure. |
| `docs/history/milestones.md` | LOAD ON DEMAND | Keep historical facts only; clearly distinguish superseded milestones from current state. |
| `docs/agent-audit/01-current-state.md` and this audit | LOAD ON DEMAND | Retain as time-stamped audit evidence, not task-routing context. |
| `docs-architect`, `docs-auditor`, `docs-writer`, and `knowledge-synthesizer` skills | LOAD ON DEMAND | Retain; consolidate repeated documentation workflow rules. |
| `skill-creator` and `skill-tester` skills | LOAD ON DEMAND | Retain; consolidate overlapping skill-authoring checklists. |
| `unity-feature-implementation`, `unity-architecture-review`, `unity-refactor-aposd`, and `unity-multiplayer-review` skills | LOAD ON DEMAND | Retain for distinct task triggers; remove rules duplicated from `AGENTS.md`. |
| `unity-ui-flow-implementation` and `unity-ui-flow-review` skills | LOAD ON DEMAND | Retain for project-specific UI behavior; replace static file lists with a few ownership entrypoints. |
| `unity-spider-player-implementation` skill | LOAD ON DEMAND | Retain only after it is aligned with the actual spider baseline. |
| Skill reference files and checklists | LOAD ON DEMAND | Retain only where they add detail beyond the parent skill; do not load by default. |
| `.github/workflows/deploy-pages.yml`, `Packages/manifest.json`, `ProjectSettings/`, source files, and serialized assets | DERIVE FROM REPOSITORY | Treat repository files as the authority for volatile names, versions, dependencies, paths, and wiring coverage. |
| Local tool descriptions | DERIVE FROM REPOSITORY | No repository-local agent tool configuration or tool descriptions exist. Host tool instructions should not be copied into project docs. |
| `ForAgents/logs_*/` | REMOVE | Archived output is not reliable current workflow context and should not be routed to agents. |

## Recommendations

### 1. Reduce The Global Baseline To Enforceable Project Rules

**CURRENT**

`AGENTS.md` is the sole always-loaded repository context. It combines project-specific conventions with generic engineering advice, repeated Unity guidance, implementation preferences, and asset-editing boundaries.

**PROBLEM**

Several rules are restated in `docs/unity/runtime-architecture-guidelines.md` and Unity skills: composition, avoiding mechanical splitting, explicit initialization, serialized wiring, null handling, diagnosis, verification, and multiplayer review. Some global statements are broad preferences rather than mechanically verifiable constraints, including "prefer the best architectural solution" and "prefer the simplest design." The unconditional multiplayer wording is ambiguous for documentation, editor tooling, and isolated presentation changes.

**CLASSIFICATION**

ALWAYS LOAD

**PROPOSED CHANGE**

Keep only concise, project-specific constraints that cannot be cheaply inferred: C# naming and namespace rules, assembly baseline, config and `CreateAssetMenu` rules, Unity wiring and initialization restrictions, `UniTask`, serialized-asset boundaries, and an explicit conditional multiplayer review trigger. Remove generic senior-engineering advice and leave detailed rationale, examples, and subsystem rules to on-demand documents or skills. State one precedence rule: `AGENTS.md` overrides all repository documentation and skills.

**EXPECTED EFFECT ON TOKEN USAGE**

Moderate reduction on every task by eliminating repeated policy from the global prompt and downstream context.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. Agents see fewer competing formulations while retaining the constraints that materially affect Unity changes.

### 2. Establish One Compact Agent Router

**CURRENT**

`docs/index.md`, `docs/ai/assistant-entrypoint.md`, `docs/ai/retrieval-map.md`, `docs/ai/coding-skills.md`, and `docs/project-map.md` all route agents to context. The entrypoint repeats current scenes, packages, assemblies, startup details, UI layout, platform direction, and workflow paths.

**PROBLEM**

Multiple entrypoints force agents to choose between near-identical maps. The fixed retrieval order in `retrieval-map.md` requires reading `project-map.md` even for focused UI, CI, documentation, or spider work. The duplicated facts already drifted: `project-structure.md` omits current camera and spider scripts from its inventory, and the two inventories do not consistently list current HUD, camera, and spider assets.

**CLASSIFICATION**

ALWAYS LOAD

**PROPOSED CHANGE**

Make `docs/ai/assistant-entrypoint.md` the only compact agent router. Limit it to task-to-source mapping, a short precedence rule, and links to `AGENTS.md`, `docs/index.md`, and the relevant task-specific skill. Remove `docs/ai/retrieval-map.md` and `docs/ai/coding-skills.md` after moving any unique routing information into the entrypoint. Keep `docs/index.md` as an on-demand human index rather than a second agent router.

**EXPECTED EFFECT ON TOKEN USAGE**

High reduction for orientation work by replacing several overlapping navigation documents with one short route map.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. Agents have one deterministic starting point and avoid irrelevant repository-map reads.

### 3. Stop Maintaining Detailed Repository Inventories In Documentation

**CURRENT**

`docs/project-map.md` and `docs/unity/project-structure.md` enumerate script files, assets, prefabs, config assets, packages, assembly definitions, and implementation observations. The current tree already differs from these lists: it contains `UIScreenHandle.cs`, `UIPopupHandle.cs`, camera and spider files, `pf_FreeLookCamera.prefab`, `pf_UI_HUD.prefab`, `CameraConfig.asset`, `SpiderConfig.asset`, and `HUDConfig.asset` that are not consistently documented.

**PROBLEM**

File-by-file lists are expensive to read, easy to stale, and no more authoritative than a targeted file search. They create false confidence because an agent may trust an incomplete list rather than inspect the affected area.

**CLASSIFICATION**

DERIVE FROM REPOSITORY

**PROPOSED CHANGE**

Reduce `project-map.md` to stable top-level ownership, source-of-truth mapping, and non-obvious repository conventions. Reduce `project-structure.md` to stable Unity folder ownership and naming conventions. Replace all volatile file inventories, package lists, scene/prefab/config enumerations, and current-code summaries with the authoritative directory or configuration path to inspect.

**EXPECTED EFFECT ON TOKEN USAGE**

High reduction whenever an agent loads either map; targeted searches cost less than loading long, mostly irrelevant inventories.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. Agents retrieve current repository state instead of depending on manually synchronized snapshots.

### 4. Separate Global Rules From Unity Runtime Guidance

**CURRENT**

`docs/unity/runtime-architecture-guidelines.md` repeats much of `AGENTS.md` and the Unity implementation skills across composition, lifecycle methods, serialized references, null handling, config types, namespaces, assemblies, verification, editor boundaries, and multiplayer.

**PROBLEM**

The 265-line page is both a current-runtime description and a broad coding manual. Repetition creates multiple policy sources, while examples of current paths and components are volatile. Some guidance is not mechanically testable, such as when a module is "deep" enough or a split reduces cognitive load.

**CLASSIFICATION**

LOAD ON DEMAND

**PROPOSED CHANGE**

Constrain this page to Unity-specific decisions that require local context: the startup and scene-entry pattern, DI and config ownership, scene/prefab wiring boundary, and the rule for handling ordered initialization. Link to `AGENTS.md` for global conventions, and to subsystem skills for UI, spider, multiplayer, or refactoring. Mark subjective design heuristics as review questions rather than mandatory rules.

**EXPECTED EFFECT ON TOKEN USAGE**

Moderate reduction for Unity tasks and substantial reduction in duplicate rule loading.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. The page becomes an authoritative local supplement rather than a competing restatement of every coding rule.

### 5. Remove Superseded External Prompt And Tutorial Material

**CURRENT**

`docs/ai/external-agent-guidance.md` preserves an external prompt containing contradictory requirements, including Russian explanations, macOS assumptions, underscore-prefixed private fields, arbitrary file-size limits, and permission for speculation. `docs/unity/unity-composition-guide.md` is a very large external Russian-language tutorial with generic 2D examples and rules that conflict with local conventions.

**PROBLEM**

Both pages explicitly defer to local policy but remain linked from multiple primary navigation surfaces. They consume large amounts of context, contradict `AGENTS.md`, and invite agents to apply irrelevant generic patterns such as mandatory `Component` suffixes or private-field underscores.

**CLASSIFICATION**

REMOVE

**PROPOSED CHANGE**

Remove both files from agent navigation and delete them after preserving any genuinely project-specific decision in the authoritative local rule. Do not replace them with another generic tutorial. If provenance is useful, retain a one-line historical note in the relevant milestone rather than the original prompt or examples.

**EXPECTED EFFECT ON TOKEN USAGE**

Very high reduction when an agent follows the current navigation links; these are the largest low-value context sources.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. Removes direct contradictions and reduces the risk of generic patterns overriding project conventions.

### 6. Align Spider Context With The Current Rewrite Baseline

**CURRENT**

`docs/unity/spider-player-controller-plan.md` correctly states that previous locomotion was removed and that no new architecture is defined. However, the spider skill still mandates a stage order containing removed `SpiderSurfaceComponent`, `SpiderOrientationComponent`, and `SpiderMovementComponent` files, asks agents to extend planned components, and states that camera integration is later even though the current baseline already includes camera spawning and binding.

**PROBLEM**

The plan and skill give incompatible instructions for the same task. An agent can follow the skill's obsolete staged design instead of designing the requested rewrite against the documented preserved boundary.

**CLASSIFICATION**

LOAD ON DEMAND

**PROPOSED CHANGE**

Keep one concise spider baseline document: current preserved spawn/camera boundary, intentionally removed code, explicit single-player scope, and the requirement to propose the next locomotion design before implementation. Rewrite the spider skill to retrieve that baseline and current files, without prescribing absent component names or obsolete stage order. Do not retain a roadmap until a new one is explicitly approved.

**EXPECTED EFFECT ON TOKEN USAGE**

Moderate reduction for spider work by removing obsolete stages and repeated file lists.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Much higher. Prevents implementation against deleted architecture and makes the rewrite decision explicit.

### 7. Make Milestone History Unambiguously Historical

**CURRENT**

`docs/history/milestones.md` contains current-looking spider implementation claims after the later milestone that removed those files. It also repeats current runtime details, documents the rationale for the external guidance, and is linked as a routine source of recently established systems.

**PROBLEM**

Earlier completed milestones can be misread as present behavior, especially where they name deleted spider files. Milestones are an inefficient source for current implementation details and duplicate the source-of-truth pages.

**CLASSIFICATION**

LOAD ON DEMAND

**PROPOSED CHANGE**

Add a clear historical-only statement at the top and label superseded milestones as superseded with links to the replacing current-state document. Keep entries concise: decision, date or revision reference when available, and link to the current source. Remove duplicated current-state explanations from history and stop routing ordinary implementation work to milestones.

**EXPECTED EFFECT ON TOKEN USAGE**

Moderate reduction when historical context is needed and avoids unnecessary history retrieval for current work.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. Agents can distinguish current behavior from the path used to reach it.

### 8. Keep CI/CD Guidance Operational Rather Than A YAML Mirror

**CURRENT**

`docs/systems/ci-cd.md` restates workflow triggers, jobs, secrets, cache paths, targets, and artifact paths from `.github/workflows/deploy-pages.yml`. The repository has no committed local build, test, lint, formatter, or Unity batch-mode validation command. The current audit also records uncertainty about historical build logs and a previous build-profile observation.

**PROBLEM**

Workflow settings are cheap to inspect and will drift when YAML changes. Conversely, the absence of supported local validation is a durable and important fact that agents repeatedly need to rediscover. Archived logs are not a trustworthy authority for current configuration.

**CLASSIFICATION**

LOAD ON DEMAND

**PROPOSED CHANGE**

Keep CI/CD documentation for intent, ownership, credentials/permissions concepts, deployment limitations, known failure diagnosis, and the explicit statement that no supported local validation command is currently documented. Link to the YAML for exact settings. When a command is verified, add a small on-demand validation runbook that records the exact command, prerequisites, expected log location, and exit behavior; do not invent commands before verification.

**EXPECTED EFFECT ON TOKEN USAGE**

Small reduction for CI tasks; a small future on-demand cost replaces repeated repository investigation for validation.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. Agents will not mistake stale YAML summaries or archived logs for supported validation workflows.

### 9. Keep Platform Direction As A Narrow Product Decision

**CURRENT**

`docs/systems/platform-strategy.md` says mobile and PC are long-term targets and WebGL Pages is temporary. The same facts appear in the project map, entrypoint, and CI/CD documentation.

**PROBLEM**

The decision is valuable for platform-sensitive work but does not affect most tasks. Repeating it in general routing pages consumes baseline context and makes product direction appear more settled or comprehensive than its brief source allows.

**CLASSIFICATION**

LOAD ON DEMAND

**PROPOSED CHANGE**

Retain this page as the sole source for the mobile-plus-PC versus temporary-WebGL decision. Remove repeated wording from navigation and project maps, leaving only a link for platform, performance, or delivery work. Keep only decisions made by the project, not generic claims about ECS, jobs, or multithreading.

**EXPECTED EFFECT ON TOKEN USAGE**

Small reduction for general tasks and no meaningful increase for platform-sensitive work.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. Agents retrieve the decision only when it is relevant and do not infer unsupported technical strategy from it.

### 10. Deduplicate Documentation-Maintenance Instructions

**CURRENT**

`docs/workflows/updating-docs.md`, `docs/index.md`, `docs/ai/assistant-entrypoint.md`, the docs skills, and `knowledge-synthesizer` all repeat source-of-truth updates, navigation updates, concrete-path requirements, and milestone capture.

**PROBLEM**

The same workflow is phrased several ways, including an ambiguous instruction to record "notable" or "major" work in milestones without criteria. Agents may update several navigation pages solely because they are all mentioned, increasing churn and drift.

**CLASSIFICATION**

LOAD ON DEMAND

**PROPOSED CHANGE**

Make `docs/workflows/updating-docs.md` the sole procedural authority. Let documentation skills link to it rather than repeat it. Define a small, observable threshold for milestone entries, such as a user-visible system, durable cross-cutting decision, or delivery workflow change. Keep navigation updates limited to the single agent router and the human index when a new authoritative page is created.

**EXPECTED EFFECT ON TOKEN USAGE**

Moderate reduction for documentation tasks and skill loads.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. Clear ownership prevents duplicate documentation and arbitrary milestone entries.

### 11. Retain Skills, But Remove Their Copied Baseline Rules

**CURRENT**

The thirteen skills have useful task-specific triggers and are correctly on demand. The generic Unity skills repeat `AGENTS.md` and runtime-guideline rules. The feature checklist repeats the feature skill. Documentation and skill-authoring skills also duplicate their reference checklists.

**PROBLEM**

The skill library is structurally healthy but not context-efficient: loading a skill often reloads broad rules that an agent should already have from the global baseline or can retrieve from its dedicated authority. Repetition makes future policy changes require updates in many files.

**CLASSIFICATION**

LOAD ON DEMAND

**PROPOSED CHANGE**

Keep one skill per distinct job and retain their trigger-focused descriptions. In each skill, keep only task-specific workflow, retrieval targets, decision points, and output requirements. Replace copied global constraints with a short reference to `AGENTS.md`; replace copied documentation workflow with a reference to `docs/workflows/updating-docs.md`. Remove a bundled checklist when it contains no material detail beyond its parent skill.

**EXPECTED EFFECT ON TOKEN USAGE**

Moderate reduction per skill load, compounded across Unity implementation and review tasks.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. Skills remain sharply triggered while policy changes have one authoritative location.

### 12. Trim Static Inventories And Unverified Cautions From UI Skills

**CURRENT**

The UI implementation and review skills provide useful UI primitive choice and ownership guidance, but both repeat UI folder layouts, DI registration paths, long related-file inventories, generic Unity restrictions, and unverified cautions about old serialized identifiers and incomplete HUD asset coverage.

**PROBLEM**

Long static lists duplicate the repository tree and become stale. The caution that serialized assets "still show old type identifiers" has no linked evidence or validation procedure, so an agent cannot determine when it applies. The existing HUD prefab and config assets demonstrate why an unqualified incompleteness claim can mislead.

**CLASSIFICATION**

LOAD ON DEMAND

**PROPOSED CHANGE**

Retain the UI primitive taxonomy, ownership decision rules, and UI-specific editor-wiring follow-up. Replace long file lists with a small set of ownership entrypoints, and require current asset inspection for wiring facts. Replace unsupported cautions with a concrete verification step or remove them. Let the global baseline own generic null, search, async, and serialized-reference policy.

**EXPECTED EFFECT ON TOKEN USAGE**

Moderate reduction for UI work.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. The skills focus on UI decisions and do not make unsupported claims about current serialized state.

### 13. Keep Review Skills As Checklists, Not Architecture Manuals

**CURRENT**

`unity-architecture-review`, `unity-refactor-aposd`, and `unity-multiplayer-review` each provide a short, purpose-specific review workflow. Their rules overlap with the global conventions and runtime-guideline heuristics.

**PROBLEM**

The overlap is smaller than in implementation skills, but repeated wording such as composition preference, initialization, and multiplayer evaluation can diverge. Some terms, including "framework leakage," "deep modules," and "suspect," are useful prompts but not mechanically verifiable rules.

**CLASSIFICATION**

LOAD ON DEMAND

**PROPOSED CHANGE**

Retain the three skills separately because their triggers and outputs are distinct. Limit each to its review questions and required findings format. Mark subjective concepts as questions to justify with repository evidence, not pass/fail requirements. Reference `AGENTS.md` for shared conventions rather than restating them.

**EXPECTED EFFECT ON TOKEN USAGE**

Small reduction per review task.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. Review results become evidence-based rather than driven by duplicated style slogans.

### 14. Treat Repository Files As The Authority For Volatile Facts

**CURRENT**

Documentation frequently labels itself "source of truth" while listing facts whose real authority is a Unity asset, YAML workflow, package manifest, project settings file, or source file. This includes version, package inventory, build scene list, current scripts, prefab coverage, and DI registrations.

**PROBLEM**

"Source of truth" is used for both policy documents and derivative summaries. That ambiguity caused documented inventories to drift and makes it unclear whether an agent should update a document or verify the actual repository before acting.

**CLASSIFICATION**

DERIVE FROM REPOSITORY

**PROPOSED CHANGE**

Use "policy owner" for durable decisions and "repository authority" for facts directly inspectable in code or configuration. Each current-state page should name the exact authoritative path and avoid reproducing it. Require a targeted repository check before acting on volatile paths, versions, dependencies, serialized references, or workflow values.

**EXPECTED EFFECT ON TOKEN USAGE**

Moderate reduction by replacing broad summaries with focused reads of the files relevant to the task.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Much higher. Agents have a clear conflict-resolution rule and act on current configuration.

### 15. Do Not Add Nested Rules Or Local Tool Prompts Prematurely

**CURRENT**

There are no nested `AGENTS.md` files, no repository-local MCP configuration, no agent prompt files, and no local tool descriptions. `.opencode/skills/` is tracked, while `.opencode/package.json` is intentionally ignored by `.opencode/.gitignore`.

**PROBLEM**

Adding nested rules for existing `Assets/`, `docs/`, or `.opencode/` folders would duplicate the root baseline and increase automatic context. Copying host tool descriptions into the repository would become stale and cannot improve tool behavior. The ignored package manifest should not be treated as durable shared workflow configuration.

**CLASSIFICATION**

DERIVE FROM REPOSITORY

**PROPOSED CHANGE**

Keep one root `AGENTS.md`. Add a nested file only when a subtree needs a short rule that conflicts with or narrows the root policy and cannot live in an on-demand skill. Keep host tool guidance outside repository documentation. If OpenCode package dependencies become a shared requirement, decide explicitly whether their manifest should be tracked; until then, document neither as a project contract.

**EXPECTED EFFECT ON TOKEN USAGE**

Prevents future automatic-context growth.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. Agents avoid conflicting scope rules and do not depend on untracked local setup.

### 16. Remove Archived Workflow Logs From Agent Retrieval

**CURRENT**

`ForAgents/logs_*/` is described as retained prior workflow output. The current-state audit already notes that its build-profile information may come from a different workflow revision than the committed YAML.

**PROBLEM**

Logs are historical evidence, not instructions. Their names and freshness do not establish relationship to the current commit, so linking or searching them as default agent context creates avoidable uncertainty.

**CLASSIFICATION**

REMOVE

**PROPOSED CHANGE**

Remove these logs from all agent navigation and documentation source lists. Retain them only if an explicit debugging or archival requirement exists, with a date, workflow run URL or identifier, and revision association. Otherwise, remove the generated archive from the repository separately from this documentation audit.

**EXPECTED EFFECT ON TOKEN USAGE**

Small reduction, primarily by eliminating unproductive investigation during CI diagnosis.

**EXPECTED EFFECT ON AGENT RELIABILITY**

Higher. The committed workflow and live CI results remain the operative evidence.

## Priority Order

1. Consolidate the global baseline and agent router.
2. Remove the external prompt and composition tutorial from navigation.
3. Correct the spider plan and skill before more spider work is attempted.
4. Replace static project and Unity inventories with repository-derived retrieval.
5. Consolidate documentation-maintenance and skill rules.
6. Create a validation runbook only after supported commands are actually verified.

## Expected Net Result

The recommended always-loaded context is a compact `AGENTS.md` plus a short task router. All Unity subsystems, skills, CI/CD, history, and documentation workflows become selective reads. Volatile repository facts are read from their source files, removing the largest source of stale context while preserving durable project decisions.

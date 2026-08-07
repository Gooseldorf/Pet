# Final Agentic Workflow Review

## Verdict

The migration is accepted with targeted corrections. The active context system now has one reliable repository router, distinct on-demand workflows, narrow durable topic owners, and an executable context check. No remaining documentation or routing issue justifies another redesign.

This review made three high-confidence corrections that measurably improve normal agent work:

- narrowed the UI skill's multiplayer trigger to authority or peer-visible gameplay;
- distinguished general architecture review from specialized UI and spider work in host-visible skill metadata; and
- fixed `Context` validation so a skill directory without `SKILL.md` fails instead of being silently skipped.

A root task-to-skill table was not added. The host already advertises skill names and descriptions, so copying that catalog into `AGENTS.md` would increase every task's context and create a second trigger authority. The overlapping null-guard statements in `AGENTS.md` were also left alone because merging them would not measurably improve normal work.

## Before

### Approximate Persistent Context Size

At the audited `HEAD` baseline, root `AGENTS.md` was approximately 1,004 tokens and the 13 host-advertised skill names and descriptions were approximately 988 tokens. Persistent repository context was therefore approximately 2,000 tokens before host wrapper overhead. Estimates use character count divided by four and are directional rather than tokenizer-exact.

The larger cost occurred after routing: agents could be led through several overlapping maps and then into long inventories, duplicated skill guidance, historical material, or the 2,882-line generic composition guide.

### Major Duplication

- Agent routing was repeated across `docs/index.md`, `docs/ai/assistant-entrypoint.md`, `docs/ai/retrieval-map.md`, `docs/ai/coding-skills.md`, and `docs/project-map.md`.
- Current scripts, assets, scenes, packages, and wiring were mirrored across project and Unity structure documents.
- Unity composition, initialization, serialized-reference, null-handling, multiplayer, and completion rules were repeated in root policy, runtime guidance, skills, and checklists.
- Documentation authoring, skill authoring, and UI review/implementation each had overlapping skill packages.

### Major Discovery Problems

- No repository evidence established that the declared assistant entrypoint loaded automatically.
- A fixed retrieval order encouraged broad map reads before task classification.
- Skill descriptions and a separate skill catalog competed as discovery authorities.
- Stale inventories and historical spider instructions could be mistaken for current implementation.

### Major Workflow Problems

- There was no executable context-integrity check.
- There was no supported local Unity compile, test, or asset-integrity command.
- The deployment workflow was the only Unity execution path and was not a normal change gate.
- Skills requested validation without a supported command or deterministic result contract.
- Archived workflow logs could be mistaken for current validation evidence.

## After

### Approximate Persistent Context Size

Root `AGENTS.md` is approximately 1,256 tokens and the nine host-advertised skill names and descriptions are approximately 550 tokens after the final description correction. Persistent repository context is approximately 1,800 tokens before host wrapper overhead, a reduction of roughly 10 percent from the measured baseline.

The more important reduction is selective context: skill bodies are short and on demand, broad navigation layers are gone, and focused work normally reaches repository evidence after at most one primary skill and one topic page.

### Context Routing Model

```text
request
  -> root AGENTS.md plus host-advertised skill metadata
  -> one matching skill body when a repeated workflow applies
  -> one topic document only when durable intent affects the task
  -> targeted source, asset, manifest, setting, or workflow authority
  -> implementation, diff review, and completion report
```

`AGENTS.md` owns repository-wide constraints and narrow topic routes. Skill frontmatter owns workflow discovery. Topic pages own durable intent that cannot be derived safely from current files. Repository files own observable facts. `docs/index.md` is reserved for human browsing and broad documentation audits.

### Validation Workflow

For `AGENTS.md`, documentation, and skills, run:

```powershell
powershell -ExecutionPolicy Bypass -File tools/validate.ps1 -Mode Context
```

The command writes `.artifacts/validation/Context/summary.txt`, returns a failing exit code, checks active Markdown links and repository paths, validates skill frontmatter and folder/name agreement, and now detects skill packages missing `SKILL.md`. `.github/workflows/validate.yml` runs the same contract for pull requests, pushes to `dev`, and manual dispatches.

Unity compile and test validation remains unsupported. During this final review, the pinned Unity editor completed a clean batch-mode import with exit code 0, but an intentional compiler-error probe did not terminate reliably under `-quit`. Advertising that invocation would reduce rather than improve agent reliability. The probe was removed completely and production source is unchanged.

### Remaining Intentional Tradeoffs

- `AGENTS.md` retains approximately 1,250 tokens because Unity naming, assembly, config, wiring, initialization, async, and serialized-asset constraints have enough cross-task impact to justify their baseline cost.
- Skill discovery depends on host-advertised metadata. Hosts may snapshot skill packages at session start; regression tests should use a fresh session after skill changes.
- There is no duplicate task-to-skill catalog, nested `AGENTS.md`, generated repository map, ADR framework, task-template layer, validation skill, or host-specific Codex copy.
- Unity compilation, EditMode, PlayMode, and asset-integrity checks remain absent until each has deterministic success and failure behavior against the pinned editor. Empty test assemblies are intentionally not used as a substitute.
- Serialized scene, prefab, and config changes remain an explicit Unity Editor follow-up unless the user authorizes those asset edits.
- `docs/agent-audit/` remains temporary review evidence. Delete it after acceptance is captured in Git history; it is not part of normal routing.

## Regression Tasks

Use these prompts in a fresh agent session whenever root context, skill metadata, skill bodies, topic documents, or validation routing changes. A pass means the agent reaches the listed evidence without loading unrelated maps or skills and preserves the stated boundary.

| Representative task | Expected routing and behavior |
| --- | --- |
| "Fix a null reference in a gameplay MonoBehaviour." | Load `unity-feature-implementation`; inspect the affected owner, callers, serialized integration, and adjacent tests. Do not read broad docs unless startup or ownership is unclear. |
| "Review whether this UI navigation should be a screen or popup." | Load `unity-ui-flow`, not the general architecture skill; inspect current UI owners and return a review without editing unless requested. |
| "Add a pause-menu button that changes only local single-player state." | Load `unity-ui-flow`; do not load multiplayer review; identify exact prefab or scene follow-up and state that Unity validation is unavailable. |
| "Add a lobby ready button whose state is visible to every peer." | Load `unity-ui-flow` plus `unity-multiplayer-review`; identify authority and synchronization before implementation. |
| "Change scene startup so a service initializes before the gameplay entry point." | Load `unity-feature-implementation` and `docs/unity/runtime-architecture.md`; inspect bootstrap, DI, scene-loading, and scene-scope owners. |
| "Implement wall traversal for the spider." | Load `unity-spider-player-implementation` and `docs/unity/spider-player.md`; inspect current spider/camera/input/spawn owners and stop for design approval if a new locomotion architecture is required. |
| "Why did the Pages deployment fail after an action update?" | Read `docs/systems/ci-cd.md` and the current workflow YAML; do not use archived logs or mirror exact YAML values into prose. |
| "Audit the docs for stale package and scene inventories." | Load `docs-auditor` and `docs/index.md`; verify claims against manifests, settings, assets, and source; prefer deletion of derivative inventories. |
| "Create a skill for recurring localization checks." | Load `skill-authoring`; establish direct, indirect, and near-miss triggers, avoid copying root policy, and run `Context` validation. |
| "Import a third-party animation pack and document its license." | Read `THIRD-PARTY-ASSETS.md` and upstream license evidence; do not load Unity architecture or general documentation maps unless ownership is unclear. |

# Agentic Context Migration Report

## Scope

Implemented the approved context redesign without changing production game code or serialized Unity assets. Root `AGENTS.md` is now the only repository instruction router; durable topic pages and skill bodies are on-demand.

The audit package remains temporarily because this requested report is part of the active migration review. It is excluded from normal context validation and routing. Delete `docs/agent-audit/` after migration acceptance and retain the migration commit for provenance.

## Implemented Structure

- Replaced the four `docs/ai/` routing pages with compact routing in `AGENTS.md`.
- Reduced `docs/index.md` to a human/audit catalog and `docs/project-map.md` to stable ownership boundaries.
- Replaced the broad runtime and spider documents with `docs/unity/runtime-architecture.md` and `docs/unity/spider-player.md`.
- Consolidated documentation work into `docs-maintenance`, skill work into `skill-authoring`, and UI work into `unity-ui-flow`.
- Kept the six distinct Unity workflows: feature implementation, architecture review, refactoring, multiplayer review, spider implementation, and UI flow.
- Added `tools/validate.ps1 -Mode Context`, deterministic `.artifacts/validation/Context/summary.txt`, root artifact ignores, and a context-only GitHub Actions adapter.

## Deleted Context

| Deleted artifact | Why removal was safe |
| --- | --- |
| `docs/ai/assistant-entrypoint.md` | Its routing role moved to the automatically loaded root `AGENTS.md`; its current-fact inventory was repository-derivable. |
| `docs/ai/retrieval-map.md` | It duplicated routing and imposed an unnecessary broad retrieval order. |
| `docs/ai/coding-skills.md` | Skill frontmatter is the discovery authority; the page duplicated skill triggers and rules. |
| `docs/ai/external-agent-guidance.md` | It was an external generic prompt with contradictory platform, naming, and speculation guidance; no unique Pet decision remained. |
| `docs/history/milestones.md` | It was chronology rather than current intent, and its historical spider claims could be misread as present behavior. Current durable decisions were retained in their topic owners; Git retains history. |
| `docs/unity/project-structure.md` | Its folder, asset, package, scene, and script inventories were volatile repository facts. Stable ownership moved to `docs/project-map.md`. |
| `docs/unity/runtime-architecture-guidelines.md` | Startup, DI, scene-entry, config, spawn, and ordered-initialization intent moved to `docs/unity/runtime-architecture.md`; copied global rules were removed. |
| `docs/unity/spider-player-controller-plan.md` | The preserved spawn/camera boundary and rewrite intent moved to `docs/unity/spider-player.md`; obsolete stage and removed-file lists were discarded. |
| `docs/unity/unity-composition-guide.md` | It was a generic external tutorial with no unique project decision and conflicts with local conventions. |
| `.opencode/skills/docs-architect/SKILL.md` | Its documentation placement procedure merged into `docs-maintenance`. |
| `.opencode/skills/docs-architect/references/doc-taxonomy.md` | Its taxonomy repeated the new documentation workflow and project map. |
| `.opencode/skills/docs-auditor/references/audit-checklist.md` | Its short checklist duplicated the retained auditor skill body. |
| `.opencode/skills/docs-writer/SKILL.md` | Its evidence-based writing workflow merged into `docs-maintenance`. |
| `.opencode/skills/docs-writer/references/page-template.md` | It was boilerplate, not durable project knowledge. |
| `.opencode/skills/knowledge-synthesizer/SKILL.md` | Its documentation capture and milestone workflow merged into `docs-maintenance`; milestones are retired. |
| `.opencode/skills/knowledge-synthesizer/references/synthesis-checklist.md` | It repeated the deleted milestone and documentation workflow. |
| `.opencode/skills/skill-creator/SKILL.md` | Creation and review are one `skill-authoring` workflow. |
| `.opencode/skills/skill-creator/references/openai-opencode-notes.md` | It duplicated host-specific authoring guidance and was not project knowledge. |
| `.opencode/skills/skill-creator/references/skill-review-checklist.md` | It overlapped the consolidated skill-authoring workflow. |
| `.opencode/skills/skill-tester/SKILL.md` | Its review procedure merged into `skill-authoring`. |
| `.opencode/skills/skill-tester/references/review-rubric.md` | It duplicated the consolidated review procedure. |
| `.opencode/skills/skill-tester/references/test-prompts.md` | Trigger examples are review-time checks, not permanent retrieval context. |
| `.opencode/skills/unity-feature-implementation/references/checklist.md` | It repeated the parent skill and root policy. |
| `.opencode/skills/unity-refactor-aposd/references/smells-and-moves.md` | Its useful prompts were folded into the parent workflow; the separate artifact added no retrieval value. |
| `.opencode/skills/unity-ui-flow-implementation/SKILL.md` | Implementation and review now share one stable UI taxonomy in `unity-ui-flow`. |
| `.opencode/skills/unity-ui-flow-review/SKILL.md` | Implementation and review now share one stable UI taxonomy in `unity-ui-flow`. |
| `ForAgents/logs_78592028035/` | Ignored historical workflow output had no declared human archive requirement and was not reliable current CI evidence. |
| `ForAgents/logs_78743974738/` | Ignored historical workflow output had no declared human archive requirement and was not reliable current CI evidence. |
| `ForAgents/logs_78748913929/` | Ignored historical workflow output had no declared human archive requirement and was not reliable current CI evidence. |

The empty `docs/ai/`, `docs/history/`, `ForAgents/`, and superseded skill directories were removed.

## Verification

- Ran `powershell -ExecutionPolicy Bypass -File tools/validate.ps1 -Mode Context`: passed. The result is `.artifacts/validation/Context/summary.txt`.
- `Context` validates Markdown links in active context, explicit repository paths, skill package frontmatter, and skill folder/name agreement. It deliberately excludes the temporary historical audit package.
- Searched active context for deleted route names, obsolete spider component instructions, unconditional multiplayer instructions, milestone requirements, and `ForAgents` routing. Remaining matches occur only in audit evidence or the root ignore rule; no active context references them.
- Reviewed the resulting root router, durable docs, and nine skill bodies for contradictory authority. No active contradictions were found.
- Reviewed scope: only instructions, documentation, skills, CI configuration, ignore rules, the validation tool, and ignored local archives changed. No production code or Unity serialized assets changed.

## Deferred Gates

Only the non-Unity `Context` validator is implemented and advertised. Unity compile, EditMode, PlayMode, and asset-integrity modes remain unavailable because the approved design requires each mode to be proven against the pinned Unity editor before documentation or skills rely on it. The migration did not add unverified commands, empty test assemblies, production validation code, or serialized-asset changes.

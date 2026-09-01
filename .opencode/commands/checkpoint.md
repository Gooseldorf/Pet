---
description: Save a concise, verified handoff for the active task.
---

Create or replace `.opencode/handoff/current.md` as a handoff for the active task.

Before writing it, inspect the actual repository state with `git status --short`, `git diff --stat`, and the relevant diffs. Do not change source, assets, settings, or documentation. The handoff is the only file this command may edit.

Keep the handoff below 350 words. Do not include raw diffs, command logs, chat history, secrets, or speculative alternatives. Prefer repository references such as `path:line` over copied code. Identify unrelated worktree changes rather than assigning them to the active task.

Use exactly this structure:

```md
# Active Task Handoff

## Goal And Acceptance Criteria

## Current State
- Branch:
- Related worktree changes:
- Unrelated worktree changes:

## Confirmed Decisions And Constraints

## Completed Work

## Validation
- `<exact command>`: <result, or not run>

## Blockers And Risks

## Next Smallest Step
```

Record only facts confirmed by the repository or the current task. If a check was not run, say so. Finish by reporting the handoff path and its next smallest step.

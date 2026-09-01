---
description: Verify and continue the active task from its local handoff.
---

Continue the interrupted task using @.opencode/handoff/current.md.

Treat the handoff as a compact pointer, not as the source of truth. First inspect `git status --short`, `git diff --stat`, and the relevant current files and diffs. Reconcile every material handoff claim with repository evidence before editing. Preserve unrelated worktree changes.

If the handoff is missing, stale, or contradicts the repository, state the discrepancy and ask for the smallest clarification needed before making task changes. Otherwise, briefly confirm the recovered goal and start with the recorded next smallest step. Do not recover or paste chat history.

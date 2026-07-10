---
name: knowledge-synthesizer
description: Synthesize completed work into durable project knowledge. Use when a feature, workflow, or infrastructure module was just completed and the result should be captured in `docs/` and project milestones.
---

# Knowledge Synthesizer

Turn completed work into durable project documentation.

## Goals

- capture important technical outcomes before they stay only in chat
- update the right source-of-truth pages
- record notable milestones

## When To Use

Use this skill when asked to:

- document what was just finished
- summarize implementation results into docs
- capture project decisions or milestones
- update project memory after a substantial change

## Workflow

1. Identify what changed.
2. Identify the repository files that prove the change.
3. Map the change to one or more source-of-truth pages.
4. Update milestone history if the change is significant.
5. Update agent navigation if a new source-of-truth page was added.

## Decision Rules

- use `docs/history/milestones.md` for durable project milestones
- use topic-specific pages for current implementation details
- do not store system details only in a milestone page

## Output Expectations

Return or produce:

1. the project fact that should be preserved
2. the target doc pages
3. whether a milestone entry is needed

## Bundled References

- `references/synthesis-checklist.md`

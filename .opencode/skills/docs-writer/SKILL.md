---
name: docs-writer
description: Write or update project documentation from repository reality. Use when the user asks to document a system, summarize how something is currently implemented, or update `docs/` with concrete paths and zero-hallucination technical content.
---

# Docs Writer

Write precise technical documentation for this repository.

## Goals

- document current reality, not guesses
- use real repository paths as evidence
- write clearly for both humans and coding agents

## When To Use

Use this skill when asked to:

- document a subsystem
- update an existing page in `docs/`
- explain current implementation in durable markdown
- turn completed work into source-of-truth documentation

## Workflow

1. Read the relevant repository files first.
2. Identify the target source-of-truth page.
3. Write only claims supported by current files or workflow config.
4. Include related paths and related docs.
5. If the change is significant, update `docs/history/milestones.md`.

## Writing Rules

- Prefer short structured sections.
- Prefer file paths over vague references.
- Prefer "Current Implementation" over speculative architecture.
- Avoid generic boilerplate and invented metrics.

## Output Expectations

Most pages should include:

- purpose
- scope
- source of truth
- current implementation
- related files
- related docs

## Bundled References

- `references/page-template.md`

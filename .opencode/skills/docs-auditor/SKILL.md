---
name: docs-auditor
description: Audit the project knowledge base for gaps, staleness, and overlap. Use when the user asks what is missing from `docs/`, whether documentation is outdated, or how to improve retrieval quality for humans and coding agents.
---

# Docs Auditor

Review the quality of the local documentation set.

## Goals

- find stale or weak source-of-truth pages
- find missing links and missing entrypoints
- detect overlap and ambiguity between docs

## When To Use

Use this skill when asked to:

- audit the docs
- check for stale documentation
- identify missing project knowledge
- improve agent retrieval quality

## Workflow

1. Review `docs/index.md`, `docs/project-map.md`, and `docs/ai/assistant-entrypoint.md` first.
2. Check whether key project topics have source-of-truth pages.
3. Check whether claims are backed by real repository paths.
4. Flag overlap, outdated content, and missing cross-links.

## Output Format

Return:

1. `Status`: pass, pass with fixes, or fail
2. `Coverage`: what is covered and what is missing
3. `Staleness`: likely outdated pages or claims
4. `Navigation`: missing links or entrypoints
5. `Recommended changes`: smallest useful fixes

## Bundled References

- `references/audit-checklist.md`

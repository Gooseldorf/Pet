---
name: docs-architect
description: Organize or extend the project documentation structure. Use when the user asks where new technical documentation should live, wants to restructure `docs/`, or needs a clean information architecture for humans and coding agents.
---

# Docs Architect

Design and maintain the information architecture of the local `docs/` knowledge base.

## Goals

- keep documentation easy to navigate
- prevent overlapping source-of-truth pages
- ensure both humans and agents can find the right document quickly

## When To Use

Use this skill when asked to:

- decide where a new document should live
- restructure `docs/`
- split an overloaded document into smaller pages
- add navigation links or entrypoints for agents

## Workflow

1. Identify the documentation topic and audience.
2. Check whether an existing page already owns that topic.
3. Prefer updating the existing source-of-truth page when possible.
4. If a new page is needed, place it in the smallest fitting section.
5. Update navigation documents so the new page is discoverable.

## Decision Rules

- `docs/project-map.md` owns repository structure and source-of-truth mapping.
- `docs/ai/` owns agent-oriented navigation.
- `docs/systems/` owns runtime, infrastructure, and delivery systems.
- `docs/unity/` owns Unity layout and asset organization.
- `docs/workflows/` owns procedural guides.
- `docs/history/` owns milestone-style historical facts.

## Output Expectations

Return:

1. the target document path
2. whether to update or create
3. any required cross-links

## Bundled References

- `references/doc-taxonomy.md`

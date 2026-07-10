---
name: skill-tester
description: Review, validate, and improve OpenCode or Claude-compatible skills in this repo. Use when the user wants to audit a `SKILL.md`, check whether a skill will trigger correctly, verify packaging under `.opencode/skills/`, or tighten a skill's structure, references, and quality before relying on it.
---

# Skill Tester

Review and improve the quality of local skills.

## Goals

- catch weak or vague trigger descriptions
- detect oversized or unfocused `SKILL.md` files
- verify bundled resources are coherent
- identify when a skill should be split, simplified, or better structured

## When To Use

Use this skill when asked to:

- review a skill
- audit a `SKILL.md`
- validate a newly added skill
- improve skill trigger quality
- check whether references or scripts are organized correctly

## Validation Workflow

### 1. Check structure

Verify the skill lives in a valid project path:

```text
.opencode/skills/<skill-name>/SKILL.md
```

Then verify:

- folder name matches frontmatter `name`
- `SKILL.md` exists
- all referenced files exist
- optional directories exist only when needed

### 2. Check trigger quality

Read the `description` as if it were the only signal the model sees.

Ask:

- does it state the job clearly?
- does it say when to use the skill?
- does it include realistic user wording?
- is it too broad and likely to over-trigger?
- is it too vague and likely to under-trigger?

### 3. Check body quality

Verify that `SKILL.md` contains:

- a clear purpose
- a usable workflow
- decision rules or heuristics where needed
- references to bundled files when they matter

Flag these issues:

- generic filler text
- repeated content that belongs in `references/`
- instructions tied only to one historical example
- commands or tools that do not fit the actual environment

### 4. Check package design

Decide whether the skill should be:

- kept as one file
- split into `references/`
- extended with a helper script
- split into multiple smaller skills

### 5. Check practical usefulness

Draft realistic prompts and judge:

- should trigger
- might trigger but needs clearer wording
- should not trigger

If the trigger boundary is muddy, rewrite the description.

## Output Format

When reviewing a skill, return:

1. `Status`: pass, pass with fixes, or fail
2. `Triggering`: what is strong or weak in the description
3. `Structure`: packaging and file organization issues
4. `Content`: workflow clarity and reuse issues
5. `Recommended changes`: the smallest set of edits that materially improves the skill

## Heuristics

- Prefer smaller, sharper skills over giant omnibus skills.
- Prefer moving bulk into references instead of bloating `SKILL.md`.
- Prefer examples that sound like real user requests.
- Prefer local project terminology when the skill is project-specific.

## Bundled References

Read these when doing deeper QA:

- `references/review-rubric.md`
- `references/test-prompts.md`

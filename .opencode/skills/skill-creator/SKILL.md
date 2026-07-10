---
name: skill-creator
description: Create new OpenCode or Claude-compatible skills, adapt existing skills for this repo, and improve skill descriptions, structure, triggers, references, and testing approach. Use when the user wants to create a skill from scratch, turn a repeated workflow into a skill, port a skill into `.opencode/skills/`, or refine an existing `SKILL.md` for better triggering and reuse.
---

# Skill Creator

Create or improve reusable skills for OpenCode.

## Goals

- Turn repeated workflows into reusable `SKILL.md` packages.
- Keep skills easy to trigger, easy to maintain, and small enough to load efficiently.
- Prefer OpenCode-native project skills in `.opencode/skills/<name>/SKILL.md`.

## When To Use

Use this skill when asked to:

- create a new skill
- turn a process into a skill
- port a Claude-compatible skill into OpenCode
- improve a skill description or trigger behavior
- reorganize a large skill into references or scripts
- define a local skill library for a project

## Skill Output

Create skills with this shape:

```text
.opencode/skills/<skill-name>/
  SKILL.md
  references/
  scripts/
  assets/
```

Only create `references/`, `scripts/`, or `assets/` when they provide clear value.

## Workflow

### 1. Capture intent

Clarify these points before writing or editing a skill:

1. What exact task should the skill help with?
2. What user phrases should trigger it?
3. What output or behavior should the model produce?
4. What belongs in the main instructions versus a reference file or script?

If the conversation already demonstrates the workflow, extract the steps from the transcript instead of asking for everything again.

### 2. Choose the right package shape

Use the smallest structure that works:

- `SKILL.md` only: for lightweight procedural guidance.
- `SKILL.md` + `references/`: for larger domain knowledge or checklists.
- `SKILL.md` + `scripts/`: when the same deterministic helper would otherwise be rewritten repeatedly.
- `SKILL.md` + `references/` + `scripts/`: for complex operational skills.

Do not add directories just because they are possible.

### 3. Write the frontmatter carefully

The description is the main trigger surface. It should say both what the skill does and when to use it.

Use this pattern:

```markdown
---
name: skill-name
description: Create or improve X. Use when the user asks for Y, mentions Z, or needs A in context B.
---
```

Rules:

- `name` must match the folder name.
- Use lowercase kebab-case.
- Put trigger words near the front of `description`.
- Be specific about adjacent cases so the skill triggers reliably.

### 4. Keep SKILL.md lean

`SKILL.md` should contain:

- the job of the skill
- when to use it
- the main workflow
- decision rules
- output expectations
- links to bundled references or scripts

Move large catalogs, examples, schemas, or deep background into `references/`.

### 5. Prefer reusable guidance over overfitting

Write instructions that generalize beyond the current example.

Avoid:

- fragile instructions that only fit one prompt
- giant lists of rules with no explanation
- hardcoding local assumptions without saying so

Prefer:

- short decision frameworks
- concise examples
- explanation of why a step matters

### 6. Test the trigger and the body

After drafting a skill, test two things:

1. Would the description make the skill trigger on realistic prompts?
2. If loaded, would the body actually help another agent execute the task?

Draft at least 3 realistic prompts:

- a direct trigger
- an indirect but valid trigger
- a near miss that should not trigger

### 7. Iterate

If the skill is too broad, split it.

If the skill is too narrow, generalize its decision rules.

If the skill is too long, move detail into `references/`.

If the skill repeatedly implies the same helper logic, add a script.

## Writing Rules

- Prefer imperative guidance.
- Prefer concrete trigger phrases.
- Prefer short sections over long prose.
- Prefer one skill per coherent job.
- Prefer project-local skills over global changes unless the user asks otherwise.

## Review Checklist

Before finishing, verify:

- the folder name matches `name`
- `description` clearly states what and when
- `SKILL.md` is focused and readable
- every referenced file exists
- the skill contains only guidance relevant to its described job

## Example Creation Flow

1. Identify repeated workflow.
2. Name the skill after the job, not the current task.
3. Draft a trigger-focused description.
4. Write the minimal workflow.
5. Move bulky content into `references/`.
6. Validate with realistic prompts.
7. Refine the description last.

## Bundled References

Read these when needed:

- `references/openai-opencode-notes.md`: OpenCode-specific authoring rules and packaging notes.
- `references/skill-review-checklist.md`: a deeper review checklist for final QA.

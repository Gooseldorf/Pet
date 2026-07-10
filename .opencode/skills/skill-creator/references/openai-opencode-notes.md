## OpenCode Notes

Use these constraints when creating or porting skills into this project.

### Location

Project-local skills live at:

```text
.opencode/skills/<skill-name>/SKILL.md
```

No `opencode.json` change is required when using the default project path.

### Frontmatter

Required fields:

- `name`
- `description`

Best practices:

- `name` should match the folder name exactly.
- keep the name lowercase and hyphenated
- use a description that states both the action and trigger context

### Progressive disclosure

Keep core workflow in `SKILL.md`.

Move large content into:

- `references/` for docs, examples, schemas, or long checklists
- `scripts/` for deterministic helpers
- `assets/` for templates or output resources

### Trigger quality

Descriptions should include:

- verbs the user will actually say
- object or domain terms they are likely to mention
- enough specificity to beat nearby skills

Weak example:

```text
Helps with skills.
```

Strong example:

```text
Create new OpenCode skills, port Claude-compatible skills, and improve existing `SKILL.md` files. Use when the user wants to build a reusable skill, refine skill triggering, or organize a skill into references and scripts.
```

### Porting from Claude-compatible skills

When adapting an upstream skill:

- keep the useful workflow
- remove tool instructions that depend on unavailable commands
- replace environment-specific commands with project-local guidance
- remove references to packaging flows that do not apply here
- keep attribution in commit notes or documentation when needed

### When to add scripts

Add a helper script only if it is:

- deterministic
- reused across invocations
- meaningfully better than prose-only instructions

Do not add scripts just to make the skill look richer.

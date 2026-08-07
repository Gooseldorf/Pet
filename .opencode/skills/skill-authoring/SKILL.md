---
name: skill-authoring
description: Create, port, review, or change a local OpenCode SKILL.md package. Use for skill triggers, frontmatter, package shape, references, and skill-library consolidation.
---

# Skill Authoring

1. Define one distinct recurring job and its direct, indirect, and near-miss prompts.
2. Keep the description short, mutually distinct, and sufficient for discovery; the folder and `name` must match.
3. Keep the body to task procedure. Link to repository policy and durable docs rather than copying them.
4. Add a reference, script, or asset only when it materially reduces the body without duplicating it.
5. Check referenced paths and run `powershell -ExecutionPolicy Bypass -File tools/validate.ps1 -Mode Context`.

Report trigger boundary, package changes, and validation result.

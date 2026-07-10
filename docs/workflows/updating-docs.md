# Updating Docs

## Goal

Keep the project knowledge base accurate enough to be useful for both humans and coding agents.

## Preconditions

- A technical change has been made, or a meaningful project fact has been clarified.
- The relevant source files already exist in the repository, or are being added in the same change.

## Rules

- Update the smallest authoritative page that matches the topic.
- Prefer editing an existing source-of-truth page over creating a new overlapping document.
- Use concrete repository paths.
- Record major changes in `docs/history/milestones.md`.
- If a new source-of-truth page is added, also link it from:
  - `docs/project-map.md`
  - `docs/ai/retrieval-map.md`

## Common Update Cases

| Change type | Update |
| --- | --- |
| repository structure changed | `docs/project-map.md` |
| Unity layout changed | `docs/unity/project-structure.md` |
| build or deployment changed | `docs/systems/ci-cd.md` |
| major technical milestone completed | `docs/history/milestones.md` |
| new agent navigation target added | `docs/ai/assistant-entrypoint.md` and `docs/ai/retrieval-map.md` |

## Suggested Steps

1. Identify the source-of-truth page for the changed topic.
2. Read the affected repository files.
3. Update the page with concrete paths and current behavior.
4. Add or update related links.
5. If the change is significant, add a milestone entry.

## Verification

Check that:

- every important statement is backed by a real repo path
- the updated page is linked from at least one discoverable entrypoint
- no older page now contradicts the updated one

## Related Docs

- `../index.md`
- `../project-map.md`
- `../ai/assistant-entrypoint.md`
- `../ai/retrieval-map.md`
- `../history/milestones.md`

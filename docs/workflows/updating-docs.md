# Updating Documentation

Persist information only when it records durable intent, a repeated procedure, or an external obligation that the repository cannot reliably show. Current code, serialized wiring, versions, packages, settings, and workflow values belong in their repository authorities, not a prose mirror.

1. Read the affected authority and identify the existing topic owner in [the documentation index](../index.md).
2. Update that owner or delete superseded context. Create a page only when no durable owner fits.
3. Add a one-line entry to [the documentation index](../index.md) only when a durable page is added, renamed, or removed. Update [the project map](../project-map.md) only when an ownership boundary changes.
4. Check links and contradictions, then run `powershell -ExecutionPolicy Bypass -File tools/validate.ps1 -Mode Context`.

Do not create milestones, generic templates, repository inventories, or documentation for facts that a targeted repository read establishes.

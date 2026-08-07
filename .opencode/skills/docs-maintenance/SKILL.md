---
name: docs-maintenance
description: Create, update, restructure, or preserve durable project documentation. Use when the user asks to document a system, record an approved decision, or change the documentation structure.
---

# Docs Maintenance

Read `docs/workflows/updating-docs.md`, then inspect the repository authority for each claim.

1. Identify whether the request records durable intent, repeated procedure, or an external obligation.
2. Update the smallest existing topic owner. Create a document only when no current owner fits.
3. Do not mirror volatile source, asset, package, setting, or workflow facts.
4. Update `docs/index.md` only when a durable page changes discoverability.
5. Run `powershell -ExecutionPolicy Bypass -File tools/validate.ps1 -Mode Context`.

Report the owner changed, repository evidence inspected, and validation result.

---
name: docs-auditor
description: Audit project documentation for gaps, staleness, overlap, or retrieval cost. Use for documentation audits, context-efficiency reviews, and requests to find outdated or redundant docs.
---

# Docs Auditor

1. Set the audit boundary before loading broad documentation. Use `docs/index.md` only for a broad audit.
2. Verify material claims against their repository authority.
3. Find stale summaries, duplicate policy, conflicting instructions, and links that do not route to a clear owner.
4. Prefer deletion over moving content that is derivable from the repository.
5. Run `powershell -ExecutionPolicy Bypass -File tools/validate.ps1 -Mode Context` after changes.

Return findings first, ordered by severity with paths and evidence. State explicitly when no findings are discovered and identify remaining verification gaps.

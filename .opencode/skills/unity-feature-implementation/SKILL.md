---
name: unity-feature-implementation
description: Implement or modify routine Unity gameplay or scene-linked behavior. Use for Unity features, MonoBehaviour changes, or authored runtime code that is not better handled by the UI or spider skills.
---

# Unity Feature Implementation

1. Locate the behavior owner, callers, adjacent tests, and required scene, prefab, or config integration.
2. Decide whether the change belongs in the current `MonoBehaviour` owner or a focused collaborator.
3. Check whether serialized Editor work is required; do not replace it with a runtime workaround.
4. Implement the narrowest change against current repository evidence.
5. Load multiplayer review when the change can affect authority or peer-visible gameplay.
6. Run the narrowest available deterministic check and inspect the diff.

Report changed behavior, exact Editor follow-up, validation result, and any relevant single-player assumption.

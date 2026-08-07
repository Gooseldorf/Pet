---
name: unity-ui-flow
description: Implement, review, or place Unity UI flow. Use for screens, popups, HUDs, overlays, navigation, back flow, UI config, prefab wiring, or scene-specific UI controllers.
---

# Unity UI Flow

First decide whether the request is a review or an implementation, then inspect the affected UI, DI, config, prefab, and scene owners.

| Need | Primitive |
| --- | --- |
| Primary scene state or history-bearing flow | Screen |
| Interrupting dialog above a state | Popup |
| Persistent gameplay presentation | HUD |
| Blocking or transitional presentation | Overlay |

For a review, assess shared-flow versus scene-slice ownership, navigation/back/queue semantics, and required Editor wiring. Return `Fit`, `Primitive`, `Ownership`, `Main risk`, `Recommended shape`, and `Editor work`.

For implementation, keep shared navigation, popup, and back behavior in the shared UI flow; keep scene-specific behavior in its scene slice. Preserve config-driven prefab and DI ownership. State exact Editor work, load multiplayer review only if the UI can affect authority or peer-visible gameplay, run the narrowest available deterministic check, and inspect the diff.

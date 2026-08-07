---
name: unity-spider-player-implementation
description: Implement or extend the player-controlled spider. Use for spider traversal, surface orientation, adhesion, jump, web behavior, camera binding, or spider locomotion changes.
---

# Unity Spider Player Implementation

Read `docs/unity/spider-player.md` before inspecting the current spider, camera, input, spawn, prefab, and config owners.

1. Preserve the explicit spawn and camera-binding boundary.
2. If the request needs a new locomotion architecture, obtain design approval before implementation; no staged component plan is approved.
3. Keep the change in the smallest owner supported by current evidence.
4. State exact prefab, config, scene, or Inspector work that remains.
5. Treat the work as single-player unless multiplayer is explicitly introduced; then load multiplayer review.
6. Run the narrowest available deterministic check and inspect the diff.

Report changed ownership, Editor follow-up, single-player status, and validation result.

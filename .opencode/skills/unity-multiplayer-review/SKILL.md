---
name: unity-multiplayer-review
description: Review Unity gameplay or systems changes for multiplayer authority, synchronization, RPC flow, and desync risk. Use when a feature can affect host/client behavior, replicated state, ownership, network events, or any gameplay logic that may diverge between peers.
---

# Unity Multiplayer Review

Inspect the networking implementation before assuming a stack or authority model exists.

1. Identify each changed state or event and its authority owner.
2. Trace host, server, client, and owner execution where those roles exist.
3. Verify how peers observe the result and check for duplicate execution, timing divergence, RPC gaps, and peer-specific wiring.
4. If no multiplayer implementation is in scope, state the single-player assumption rather than inventing infrastructure.

Return `Authority`, `Sync path`, `Risks`, and `Verdict`.

---
name: unity-multiplayer-review
description: Review Unity gameplay or systems changes for multiplayer authority, synchronization, RPC flow, and desync risk. Use when a feature can affect host/client behavior, replicated state, ownership, network events, or any gameplay logic that may diverge between peers.
---

# Unity Multiplayer Review

Review Unity changes for multiplayer correctness before treating the implementation as done.

## Goals

- catch authority mistakes early
- identify host and client divergence risks
- surface RPC and synchronization gaps
- prevent local-only behavior from silently breaking multiplayer

## When To Use

Use this skill when asked to:

- review multiplayer implications
- add or change networked gameplay logic
- change state that may need synchronization
- wire gameplay events that could run on more than one peer
- assess desync risk in a Unity feature

## Review Workflow

1. Identify the gameplay state or event that changes.
2. Identify who is authoritative for that state.
3. Check whether each peer observes the same result through the intended sync path.
4. Check for missing RPCs, duplicated execution, or local-only UI assumptions.
5. Call out unresolved desync or ownership risks explicitly.

## Review Rules

- Every state change should have a clear authority owner.
- Do not assume local execution is safe just because the feature looks cosmetic.
- Check whether the same code path runs on host, client, server, or owner, and whether it should.
- Check whether button presses, timers, counters, or resets need network coordination.
- Flag logic that can run twice on host-plus-client setups or diverge due to timing.
- Flag scene or object wiring assumptions that may differ across peers.
- If the implementation is single-player only, say so explicitly instead of implying multiplayer safety.

## Output Format

Return:

1. `Authority`: who owns the state or action
2. `Sync path`: how other peers observe it
3. `Risks`: RPC, ownership, or desync concerns
4. `Verdict`: safe as-is, needs network changes, or single-player only

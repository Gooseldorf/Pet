## Architectural Priorities

- Prefer the best architectural solution over the smallest diff.
- Surface ambiguity instead of making silent assumptions.
- Prefer the simplest design that solves the real problem.
- Keep changes surgical; avoid unrelated cleanup or speculative abstraction.
- Prefer composition over inheritance for authored gameplay code unless inheritance expresses a real stable template or a clear `is-a` relationship.
- Keep authored behaviors focused, but do not split code mechanically when one small `MonoBehaviour` or class is clearer than multiple shallow pieces.
- Diagnose bugs by checking multiple plausible causes before committing to a fix.
- Verify meaningful changes when feasible instead of deferring all validation.

## Project Knowledge Base

- Project map: `docs/project-map.md`
- Agent entrypoint: `docs/ai/assistant-entrypoint.md`
- Retrieval map: `docs/ai/retrieval-map.md`
- CI/CD source of truth: `docs/systems/ci-cd.md`
- Unity structure source of truth: `docs/unity/project-structure.md`
- Record notable completed work in `docs/history/milestones.md`
- Keep this file short; keep detailed project knowledge in `docs/`

## C# And Unity Conventions

- Private fields use `camelCase` without `_` prefix.
- `const` fields use `UPPER_SNAKE_CASE` names such as `BOOTSTRAP_SCENE_NAME`.
- In authored C# code, do not use fully qualified type names such as `Pet.Gameplay.UIHudView` when a `using` directive can resolve the type; prefer `using` imports instead.
- Do not put multiple classes or structs in one `.cs` file; prefer one top-level type per file.
- Authored namespaces should stay short: prefer `Pet` or `Pet.X`; do not mirror deep folder paths into namespaces unless a third level is a stable module boundary.
- Assembly boundaries should carry architectural isolation; default authored assemblies are `Pet.Runtime` and `Pet.Editor` unless there is a concrete reason to split runtime further or introduce formal test assemblies.
- Any authored config type should be a `ScriptableObject`.
- `CreateAssetMenu` paths for authored types should start with `Configs/` and mirror the project folder structure from there.
- Prefer typed serialized references over `GameObject` + `GetComponent`.
- Avoid `GameObject.Find`, `Transform.Find`, and other runtime hierarchy search to wire references; use them only when serialized fields, config references, or scene/prefab authoring cannot provide the reference.
- Avoid defensive null-guard spam for required serialized/runtime fields; missing wiring should usually fail loudly.
- Prefer explicit initialization entry points driven by composition or spawning code over `Start`, `OnEnable`, or similar Unity lifecycle callbacks when initialization order matters.
- `OnEnable`/`OnDisable` are acceptable for simple local event subscription lifecycles, but they should not replace explicit bootstrap or spawn-driven initialization when ordering matters.
- When runtime objects are instantiated from prefabs, prefer a small explicit spawner/bootstrap flow over scene-owned references or premature factory abstractions.
- Do not add custom exception guards or defensive null checks for required DI dependencies, required serialized references, or required authored config. Let incorrect wiring fail loudly instead of masking ownership mistakes.
- Prefer `UniTask` over `Task` for async gameplay code.

## Multiplayer Review

- Always check multiplayer implications: authority, host/client flow, RPCs, sync, and desync risk.

## Unity Asset Editing Boundaries

- Do not fake required prefab/scene work with runtime hacks; if integration needs prefab or scene changes, ask the user to make them.
- Agent-side edits should stay in code unless the user explicitly asks to edit `ScriptableObject`, `.asset`, prefab, scene, or other serialized Unity assets. If such asset work is needed, say exactly what to wire or change in the Unity Editor.
- Do not create or edit Unity `.meta` files manually. Let the Unity Editor generate and maintain them.

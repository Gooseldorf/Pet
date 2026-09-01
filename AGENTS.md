## Authority And Retrieval

- User requirements define the requested outcome. This file defines repository-wide constraints. Current source, serialized assets, manifests, settings, and workflow YAML define observable facts. Topic documents define durable intent; skills define procedure and cannot override those authorities.
- Use a matching host-advertised skill when one exists. Inspect the affected repository owner before acting; do not rely on prose inventories for current wiring, versions, packages, or workflow configuration.
- Read [runtime architecture](docs/unity/runtime-architecture.md) for startup, DI, additive scenes, spawning, or ordered initialization; [spider player](docs/unity/spider-player.md) for spider work; [CI/CD](docs/systems/ci-cd.md) for CI or deployment; [platform strategy](docs/systems/platform-strategy.md) for platform decisions; [documentation maintenance](docs/workflows/updating-docs.md) for documentation work; [the project map](docs/project-map.md) for unfamiliar ownership; and [third-party assets](THIRD-PARTY-ASSETS.md) for asset licensing.
- Use [the documentation index](docs/index.md) only for human browsing or documentation audits, not as normal task startup.

## C# And Unity Conventions

- Prefer composition over inheritance for authored gameplay code unless inheritance expresses a stable template or clear `is-a` relationship. Do not split one focused owner into shallow pieces mechanically.
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

- Load multiplayer review only when work affects authority, replicated state, RPCs, peer-visible gameplay, or a feature being evaluated for multiplayer. State a single-player assumption explicitly when it matters.

## Unity Asset Editing Boundaries

- Do not fake required prefab/scene work with runtime hacks; if integration needs prefab or scene changes, ask the user to make them.
- Agent-side edits should stay in code unless the user explicitly asks to edit `ScriptableObject`, `.asset`, prefab, scene, or other serialized Unity assets. If such asset work is needed, say exactly what to wire or change in the Unity Editor.
- Do not create or edit Unity `.meta` files manually. Let the Unity Editor generate and maintain them.

## Validation And Completion

- Run the narrowest deterministic check available. For documentation, skills, or agent instructions, run `powershell -ExecutionPolicy Bypass -File tools/validate.ps1 -Mode Context`.
- Supported automated Unity validation is not yet available. Do not present generated project files, archived logs, or an unrun Unity command as validation.
- Before completion, inspect the diff for scope, accidental serialized asset changes, stale documentation impact, and conditional multiplayer implications. Report changed behavior, the exact validation command and result, and any Unity Editor follow-up.

## Session Continuity

- Before intentionally resetting an incomplete task, use `/checkpoint` to save the local handoff.
- When the user explicitly asks to continue interrupted work, use `/resume-task` or inspect the local handoff before editing. The current repository state remains authoritative.
- The handoff is local task state. Keep it concise and do not use it for unrelated requests.

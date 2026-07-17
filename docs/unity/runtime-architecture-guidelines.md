# Runtime Architecture Guidelines

## Purpose

This page defines the current preferred runtime code shape for authored Unity code in this repository.

## Scope

This is a project guideline for code structure and ownership.

It is not a claim that the repository already contains a large established gameplay architecture.

## Source Of Truth

- `AGENTS.md`
- `Assets/_Root/Scripts/`
- `docs/unity/project-structure.md`

## Current Project Reality

The current authored runtime surface is still small.

The project now has a minimal startup architecture built around:

- `Assets/_Root/Scripts/DI/GlobalScope.cs`
- `Assets/_Root/Scripts/Bootstrap/Bootstrap.cs`
- `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs`
- `Assets/_Root/Scripts/Configs/ProjectConfig.cs`
- `Assets/_Root/Scripts/Configs/UI/UIConfig.cs`
- `Assets/_Root/Scripts/Configs/UI/LoadingOverlayConfig.cs`
- `Assets/_Root/Scripts/Input/InputActionsProvider.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- `Assets/_Root/Scripts/UI/UiRoot.cs`
- `Assets/_Root/Scripts/UI/LoadingOverlay.cs`
- `Assets/_Root/Scripts/UI/LoadingOverlayController.cs`

`Assets/_Root/Scripts/Test/MainMenuTester.cs` still exists, but it is a test-style runtime script and not the architecture entry point.

That means these guidelines are intended to shape future growth from a small but real bootstrap/composition-root foundation.

## Current Startup Shape

- `Bootstrap.unity` is the startup scene in `ProjectSettings/EditorBuildSettings.asset`
- `GlobalScope` is the VContainer composition root for startup
- `ProjectConfig` is the current local authored config root registered by `GlobalScope`
- `Bootstrap` is registered as an `IAsyncStartable` entry point
- `SceneLoader` is currently the single scene-loading abstraction and loads `MainMenu` additively

This is currently the preferred shape for application startup in this repository.

## Preferred Boundaries

### `MonoBehaviour`

Prefer `MonoBehaviour` classes for:

- scene lifecycle hooks
- serialized reference wiring
- direct interaction with Unity components
- orchestration of a small scene-specific behavior

Keep the behavior in one `MonoBehaviour` when splitting it would only add indirection.

### Plain C# Classes

Prefer plain C# classes when a behavior:

- represents reusable gameplay rules
- becomes harder to reason about because of Unity callback noise
- should be testable without scene or component setup
- does not need to inherit from Unity framework types

Do not extract helpers mechanically.

Only do it when the split reduces cognitive load or isolates a real ownership boundary.

`Bootstrap`, `SceneLoader`, and `LoadingOverlayController` are acceptable examples here because they isolate startup and scene-transition responsibilities away from scene-owned UI objects.

## UI And Scene Glue

- UI classes should primarily coordinate presentation and input handling.
- Avoid letting UI-owning `MonoBehaviour` classes become the long-term home of gameplay rules.
- Prefer serialized references for TMP, buttons, and authored components.
- Avoid runtime hierarchy search for wiring except when authored setup genuinely cannot provide the reference.

## Reference Wiring

- Prefer typed serialized references over `GameObject` references plus `GetComponent`.
- Required references should usually fail loudly when not assigned correctly.
- Do not compensate for missing authored wiring with broad defensive null handling.

## Local Config Guidance

- Prefer the local `ScriptableObject` config layer for shared authored values that should not live on individual scene components.
- The current config root is `Assets/_Root/Scripts/Configs/ProjectConfig.cs`.
- Config branches can reference other config assets, but each authored config type should itself be a `ScriptableObject`.
- Keep config types split into separate `.cs` files instead of grouping multiple classes or structs into one file.
- Keep `CreateAssetMenu` paths aligned with the project folder structure, starting with `Configs/`.
- Register the local config root in the startup composition root and inject it explicitly where needed.
- Do not introduce global static config access when the existing bootstrap plus DI wiring can provide the dependency.

## Abstraction Rules

- Prefer the smallest design that keeps ownership clear.
- Avoid introducing managers, services, repositories, or generic utility layers without a concrete need.
- Prefer deeper modules that hide messy details over shallow wrappers that only forward calls.
- When a design decision is non-trivial, choose the shape that reduces what future readers must keep in their head.

## Namespaces And Assemblies

- Prefer short authored namespaces rooted at `Pet`.
- Default namespace shapes are `Pet` and `Pet.X` such as `Pet.UI`, `Pet.Input`, `Pet.Configs`, `Pet.MainMenu`, `Pet.Gameplay`, and `Pet.Editor`.
- Do not mirror deep folder paths into namespaces by default.
- Only introduce a third namespace level when it is a stable module boundary rather than a transient folder detail.
- Do not leave authored code in the global namespace.
- Prefer assembly definitions to enforce architectural boundaries.
- Default authored assemblies are `Pet.Runtime` and `Pet.Editor`.
- Do not split runtime code into more assemblies unless there is a concrete reason such as compile-time pressure, dependency isolation, a durable reusable module boundary, or a formal automated test suite.

Current practical guidance:

- keep app startup entry points in `Assets/_Root/Scripts/Bootstrap/`
- keep VContainer lifetime scopes in `Assets/_Root/Scripts/DI/`
- keep scene loading ownership in `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs`
- keep input ownership in `Assets/_Root/Scripts/Input/`
- do not route unrelated feature logic through the bootstrap classes
- do not treat test-style scripts under `Assets/_Root/Scripts/Test/` as production architecture anchors

## Multiplayer Expectations

When gameplay code can affect multiplayer behavior, always review:

- authority ownership
- host versus client execution flow
- RPC and synchronization path
- desync risk

Single-player assumptions should be stated explicitly when they are relied on.

## Unity Editor Boundaries

- Do not replace required prefab or scene integration with runtime hacks.
- If a code change requires Inspector, prefab, or scene setup, document exactly what must be wired in the Unity Editor.

## Related Files

- `AGENTS.md`
- `Assets/_Root/Scripts/Bootstrap/Bootstrap.cs`
- `Assets/_Root/Scripts/Bootstrap/MainMenuEntryPoint.cs`
- `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- `Assets/_Root/Scripts/DI/GlobalScope.cs`
- `Assets/_Root/Scripts/DI/MainMenuScope.cs`
- `Assets/_Root/Scripts/DI/GameplayScope.cs`
- `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs`
- `Assets/_Root/Scripts/Configs/ProjectConfig.cs`
- `Assets/_Root/Scripts/Configs/UI/UIConfig.cs`
- `Assets/_Root/Scripts/Configs/UI/LoadingOverlayConfig.cs`
- `Assets/_Root/Scripts/Input/InputActionsProvider.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- `Assets/_Root/Scripts/UI/UiRoot.cs`
- `Assets/_Root/Scripts/UI/LoadingOverlay.cs`
- `Assets/_Root/Scripts/UI/LoadingOverlayController.cs`
- `Assets/_Root/Scripts/Pet.Runtime.asmdef`
- `Assets/_Root/Scripts/Editor/Pet.Editor.asmdef`
- `Assets/_Root/Scripts/Test/MainMenuTester.cs`
- `docs/unity/project-structure.md`

## Related Docs

- `project-structure.md`
- `../ai/coding-skills.md`

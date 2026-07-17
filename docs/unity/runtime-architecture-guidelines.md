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

The current authored runtime surface is still small, but it now has real scene and UI boundaries.

The main runtime foundation currently centers on:

- `Assets/_Root/Scripts/DI/GlobalScope.cs`
- `Assets/_Root/Scripts/Bootstrap/Bootstrap.cs`
- `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs`
- `Assets/_Root/Scripts/Configs/ProjectConfig.cs`
- `Assets/_Root/Scripts/UI/UIConfig.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayConfig.cs`
- `Assets/_Root/Scripts/Input/InputActionsProvider.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- `Assets/_Root/Scripts/UI/UIRoot.cs`
- `Assets/_Root/Scripts/UI/Base/UIViewBase.cs`
- `Assets/_Root/Scripts/UI/Base/UIScreenViewBase.cs`
- `Assets/_Root/Scripts/UI/Base/UIPopupViewBase.cs`
- `Assets/_Root/Scripts/UI/UIInstanceFactory.cs`
- `Assets/_Root/Scripts/UI/UIScreenNavigator.cs`
- `Assets/_Root/Scripts/UI/UIPopupCoordinator.cs`
- `Assets/_Root/Scripts/UI/UIBackRouter.cs`
- `Assets/_Root/Scripts/UI/UIBackInputListener.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlay.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayController.cs`
- `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuController.cs`
- `Assets/_Root/Scripts/UI/Gameplay/UIGameplayController.cs`

`Assets/_Root/Scripts/Test/MainMenuTester.cs` still exists, but it is a test-style runtime script and not an architecture entry point.

## Current Startup Shape

- `Bootstrap.unity` is the startup scene in `ProjectSettings/EditorBuildSettings.asset`
- `GlobalScope` is the VContainer composition root for startup
- `ProjectConfig` is the local authored config root registered by `GlobalScope`
- `UIRoot` is a persistent shared UI anchor also registered by `GlobalScope`
- `Bootstrap` is registered as an `IAsyncStartable` entry point
- `SceneLoader` is the scene-loading abstraction and currently loads `MainMenu` on startup while also supporting switches to `Gameplay`

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

`Bootstrap`, `SceneLoader`, `UILoadingOverlayController`, `UIScreenNavigator`, and `UIPopupCoordinator` are acceptable examples because they isolate scene flow and shared UI orchestration away from scene-owned views.

## UI And Scene Glue

- UI classes should primarily coordinate presentation and input handling.
- Avoid letting UI-owning `MonoBehaviour` classes become the long-term home of gameplay rules.
- Prefer serialized references for TMP, buttons, and authored components.
- Avoid runtime hierarchy search for wiring except when authored setup genuinely cannot provide the reference.
- Prefer a small shared UI-flow layer for screen navigation, popup queuing, layer routing, and back handling instead of a single god-manager.
- Keep concrete scene flow in focused controllers such as `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuController.cs` and `Assets/_Root/Scripts/UI/Gameplay/UIGameplayController.cs`.
- Keep shared view/config abstractions in `Assets/_Root/Scripts/UI/Base/` rather than duplicating show-hide behavior per screen or popup.

## Reference Wiring

- Prefer typed serialized references over `GameObject` references plus `GetComponent`.
- Required references should usually fail loudly when not assigned correctly.
- Do not compensate for missing authored wiring with broad defensive null handling.

## Local Config Guidance

- Prefer the local `ScriptableObject` config layer for shared authored values that should not live on individual scene components.
- The current config root type is `Assets/_Root/Scripts/Configs/ProjectConfig.cs`.
- Shared UI config types currently live near the UI module under `Assets/_Root/Scripts/UI/`.
- Config asset instances currently live under `Assets/_Root/Configs/`.
- Config branches can reference other config assets, but each authored config type should itself be a `ScriptableObject`.
- Keep config types split into separate `.cs` files instead of grouping multiple classes or structs into one file.
- Keep `CreateAssetMenu` paths aligned with the project folder structure, starting with `Configs/`.
- Register the local config root and scene-specific config assets in the relevant VContainer scope and inject them explicitly where needed.
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
- keep shared UI flow ownership in `Assets/_Root/Scripts/UI/`
- keep reusable UI bases in `Assets/_Root/Scripts/UI/Base/`
- keep scene-specific UI flow in focused subfolders such as `Assets/_Root/Scripts/UI/MainMenu/` and `Assets/_Root/Scripts/UI/Gameplay/`
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
- `Assets/_Root/Scripts/UI/UIConfig.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayConfig.cs`
- `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuConfig.cs`
- `Assets/_Root/Scripts/UI/Gameplay/Hud/UIHudConfig.cs`
- `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPauseMenuConfig.cs`
- `Assets/_Root/Scripts/Input/InputActionsProvider.cs`
- `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- `Assets/_Root/Scripts/UI/UIRoot.cs`
- `Assets/_Root/Scripts/UI/Base/UIViewBase.cs`
- `Assets/_Root/Scripts/UI/Base/UIScreenViewBase.cs`
- `Assets/_Root/Scripts/UI/Base/UIPopupViewBase.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlay.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayController.cs`
- `Assets/_Root/Scripts/Pet.Runtime.asmdef`
- `Assets/_Root/Scripts/Editor/Pet.Editor.asmdef`
- `Assets/_Root/Scripts/Test/MainMenuTester.cs`
- `docs/unity/project-structure.md`

## Related Docs

- `project-structure.md`
- `../ai/coding-skills.md`

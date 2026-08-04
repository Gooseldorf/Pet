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
- `Assets/_Root/Scripts/SceneLoading/ISceneEntryPoint.cs`
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
- additive content scenes now start through explicit scene-scoped entry points rather than scene-level `IAsyncStartable` registration
- `Assets/_Root/Scripts/SceneLoading/ISceneEntryPoint.cs` is the explicit startup contract for content scenes
- `Assets/_Root/Scripts/Bootstrap/MainMenuEntryPoint.cs` and `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs` are invoked explicitly by `SceneLoader` after `SceneManager.SetActiveScene(...)`

This is currently the preferred shape for application startup in this repository.

## Preferred Boundaries

### `MonoBehaviour`

Prefer `MonoBehaviour` classes for:

- scene lifecycle hooks
- serialized reference wiring
- direct interaction with Unity components
- orchestration of a small scene-specific behavior

Keep the behavior in one `MonoBehaviour` when splitting it would only add indirection.

Prefer focused responsibilities, not the smallest possible classes.

### Plain C# Classes

Prefer plain C# classes when a behavior:

- represents reusable gameplay rules
- becomes harder to reason about because of Unity callback noise
- should be testable without scene or component setup
- does not need to inherit from Unity framework types

Do not extract helpers mechanically.

Only do it when the split reduces cognitive load or isolates a real ownership boundary.

`Bootstrap`, `SceneLoader`, `UILoadingOverlayController`, `UIScreenNavigator`, and `UIPopupCoordinator` are acceptable examples because they isolate scene flow and shared UI orchestration away from scene-owned views.

## Composition Over Inheritance

- Prefer composition over inheritance for authored gameplay code.
- Reach for inheritance only when it represents a real stable template, a clear `is-a` relationship, or a small number of genuinely variable steps in an otherwise shared algorithm.
- Do not introduce base `Entity`, `Character`, or `Manager` hierarchies just to share a few fields or helper methods.
- Shared gameplay capabilities such as damage, cooldowns, movement gating, sensing, or presentation reactions should usually live in focused components or plain C# collaborators that can be recombined.
- Treat this as a strong preference, not a rule to fragment code mechanically.

## Component Orchestration

- A scene-owned gameplay root may act as a small orchestrator that wires focused components together.
- Prefer that orchestrator to hold explicit serialized references, configure stable local relationships, and coordinate cross-component reactions.
- Keep long-term gameplay rules inside the owning component or plain C# collaborator rather than letting the orchestrator become a god object.
- If one small `MonoBehaviour` is already the clearest ownership boundary, keep it together instead of splitting only to satisfy a composition slogan.

## Event-Driven Coordination

- Prefer events for local cross-component reactions when they reduce direct coupling and keep ownership clear.
- Typical examples include presentation, audio, cooldown resets, and other secondary reactions to a primary gameplay action.
- For simple scene-owned or component-owned subscription lifecycles, subscribing in `OnEnable` and unsubscribing in `OnDisable` is acceptable.
- Do not treat `OnEnable` and `OnDisable` subscriptions as a substitute for explicit bootstrap, spawn, or scene-entry initialization when startup order matters.
- Avoid duplicate fallback subscription paths such as wiring the same relationship in both `Start` and `OnEnable`.

## Condition-Based Rules

- When a reusable gameplay action depends on multiple runtime conditions, prefer a composable gating approach over baking unrelated dependencies directly into that component.
- This can be implemented through delegates, small condition objects, or another focused mechanism appropriate to the feature.
- Do not introduce a generic condition framework by default; use the lightest shape that keeps dependencies explicit and reusable.

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
- Avoid `GetComponent` in hot paths such as `Update`, `FixedUpdate`, or repeated per-item loops when authored wiring or cached references can provide the dependency.

## Initialization Order

- When initialization order matters, prefer explicit initialization methods driven by bootstrap, composition-root, or spawning code over `Start`, `OnEnable`, or other implicit Unity lifecycle callbacks.
- Prefer a single clear startup flow such as `construct -> spawn -> inject -> initialize -> tick` instead of spreading ownership across multiple Unity callbacks.
- Do not add secondary fallback initialization paths such as subscribing in both `OnEnable` and `Start` to compensate for unclear ordering.
- For additive content scenes, prefer explicit scene startup invoked by `SceneLoader` after `SceneManager.SetActiveScene(...)` over scene-level `IStartable` or `IAsyncStartable` auto-start.
- For `Rigidbody`-based gameplay controllers, it is acceptable to read the latest buffered input state in `FixedUpdate` while keeping one-shot input events queued separately between input callbacks and physics ticks.
- For simple local wiring that does not depend on external startup order, `Awake` may configure internal component relationships and `OnEnable`/`OnDisable` may manage local event subscriptions.

## Runtime Spawn Ownership

- When a gameplay runtime object should be created from authored data, prefer storing its prefab on the owning `ScriptableObject` config.
- Prefer a small focused spawner/bootstrap class for single well-defined spawn flows.
- Do not introduce a separate factory abstraction until there is a real need such as multiple variants, pooling, respawn orchestration, or multiplayer ownership flow.

## Failure Style

- Required DI dependencies, required config references, and required serialized component references should usually fail loudly through normal execution instead of defensive null checks.
- Do not add custom exception guards whose main purpose is to validate required authored wiring or required initialization sequencing.
- Fix ownership and initialization flow at the source instead of layering protective runtime checks onto expected-required dependencies.

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
- Keep files focused and reasonably compact, but do not split files mechanically to satisfy an arbitrary line-count limit.
- Prefer minimal necessary code changes when fixing bugs or extending existing behavior, as long as the resulting ownership stays clean.

## Namespaces And Assemblies

- Prefer short authored namespaces rooted at `Pet`.
- Default namespace shapes are `Pet` and `Pet.X` such as `Pet.UI`, `Pet.Input`, `Pet.Configs`, `Pet.MainMenu`, `Pet.Gameplay`, and `Pet.Editor`.
- Do not mirror deep folder paths into namespaces by default.
- Only introduce a third namespace level when it is a stable module boundary rather than a transient folder detail.
- Do not leave authored code in the global namespace.
- Prefer assembly definitions to enforce architectural boundaries.
- Default authored assemblies are `Pet.Runtime` and `Pet.Editor`.
- Do not split runtime code into more assemblies unless there is a concrete reason such as compile-time pressure, dependency isolation, a durable reusable module boundary, or a formal automated test suite.

Gameplay naming heuristics:

- For authored gameplay `MonoBehaviour` types, `SomethingComponent` and `SomethingController` are acceptable naming patterns when they clarify whether the type is a reusable capability or an input/flow coordinator.
- Do not force those suffixes onto every authored runtime type; bootstrap, DI, scene-loading, config, and UI-flow code may use clearer domain names such as `Bootstrap`, `SceneLoader`, `GlobalScope`, `UIRoot`, or `UIInstanceFactory`.

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

## Verification And Debugging

- When diagnosing bugs, consider multiple plausible causes before locking onto one fix path.
- After a meaningful code change, run the most relevant feasible verification step instead of assuming the change is safe.
- Prefer targeted verification that matches the touched ownership boundary.

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
- `Assets/_Root/Scripts/SceneLoading/ISceneEntryPoint.cs`
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

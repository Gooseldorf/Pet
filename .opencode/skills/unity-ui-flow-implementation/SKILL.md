---
name: unity-ui-flow-implementation
description: Implement or modify Unity UI flow in this project. Use when the user asks to add or change a screen, popup, HUD, overlay, button flow, UI config, UI prefab wiring, or scene-specific UI controller under `Assets/_Root/Scripts/UI/`.
---

# Unity UI Flow Implementation

Implement UI changes against this project's existing layered UI flow instead of inventing scene-local patterns.

## Goals

- preserve the shared UI flow centered on screens, popups, HUD, and overlays
- keep scene-specific behavior in focused UI controllers
- use config-driven prefab wiring and VContainer registration
- avoid runtime hacks that bypass authored Unity wiring

## When To Use

Use this skill when asked to:

- add or modify a `Screen`, `Popup`, `HUD`, or `Overlay`
- change button flow, view callbacks, or screen-opening behavior
- add a new UI config, prefab-backed view, or UI controller
- change `UIInstanceFactory`, `UIScreenNavigator`, `UIPopupCoordinator`, or back-flow behavior
- wire a UI feature into `MainMenu`, `Gameplay`, or `LoadingScreen`

Do not use this as the main skill for architecture-only evaluation. Use `unity-ui-flow-review` for that.

## Current UI Shape

The project's authored UI runtime is organized as:

- shared flow in `Assets/_Root/Scripts/UI/`
- reusable bases in `Assets/_Root/Scripts/UI/Base/`
- scene slices in `Assets/_Root/Scripts/UI/MainMenu/`, `Assets/_Root/Scripts/UI/Gameplay/`, and `Assets/_Root/Scripts/UI/LoadingScreen/`
- config asset instances in `Assets/_Root/Configs/UI/`
- authored prefabs in `Assets/_Root/Prefabs/UI/`

Key flow types:

- `UIRoot.cs`: layer anchors for `Screen`, `Popup`, `Hud`, `Overlay`
- `UIInstanceFactory.cs`: prefab instantiation plus DI injection
- `UIScreenNavigator.cs`: screen lifetime and history
- `UIPopupCoordinator.cs`: popup lifetime and queue behavior
- `UIBackRouter.cs` and `UIBackInputListener.cs`: centralized back handling
- `UIViewBase.cs`: shared `CanvasGroup` visibility behavior

## UI Primitive Choice

Choose the lightest primitive that matches the behavior:

- `Screen`: a primary scene state or full-screen flow that may participate in history
- `Popup`: an interrupting dialog layered above another state
- `Hud`: persistent gameplay UI that can coexist with gameplay input
- `Overlay`: blocking or transitional UI such as loading

Do not implement a popup as a screen or a screen as a HUD just because it is faster to wire.

## Workflow

1. Identify whether the change belongs in shared flow or in a scene slice.
2. Choose `Screen`, `Popup`, `Hud`, or `Overlay` explicitly.
3. Follow the existing project pattern for that primitive:
    - `View + Controller + Config` for screens and popups
    - `View + Controller + Config` for HUD when prefab-backed runtime creation is needed
    - overlay behavior should stay aligned with `UILoadingOverlay`
4. Put the code in the owning folder under `Assets/_Root/Scripts/UI/`.
5. Register scene-specific configs and controllers in the relevant scope:
   - `Assets/_Root/Scripts/DI/MainMenuScope.cs`
   - `Assets/_Root/Scripts/DI/GameplayScope.cs`
   - shared flow in `Assets/_Root/Scripts/DI/GlobalScope.cs`
6. Prefer config-driven prefab references over direct scene lookup.
7. If Unity Editor wiring is required, state exactly which prefab, asset, or Inspector field must be assigned.

## Decision Rules

- Keep shared flow changes inside the existing UI flow layer instead of creating parallel managers.
- Preserve `UIViewBase` visibility semantics and `CanvasGroup`-based show/hide behavior unless the change truly requires a different base behavior.
- Prefer `UIScreenConfigBase` and `UIPopupConfigBase` for new screen and popup config types.
- Prefer typed serialized fields on views for buttons and UI elements.
- Do not use `GameObject.Find`, `Transform.Find`, or broad runtime hierarchy search for UI wiring.
- Do not add defensive null-guard noise for required authored references.
- Prefer `SetCallbacks(...)` or existing controller-to-view orchestration patterns over introducing presenter layers.
- Treat `View + Controller + Config` as a project UI pattern, not a universal requirement to invent extra layers where one focused owner is clearer.
- Local `OnEnable` and `OnDisable` subscriptions are acceptable for simple UI-owned wiring, but they should not replace explicit startup flow when scene ordering matters.
- Use `UniTask` for async UI flow to match the rest of the project.
- Respect existing flow semantics:
  - screen history goes through `UIHistoryModeEnum`
  - popup behavior goes through `UiPopupQueueModeEnum`
  - lifetime caching goes through `UICacheModeEnum`
- If a UI action affects gameplay state rather than only local presentation, call out multiplayer implications explicitly.

## Project-Specific Cautions

- Some serialized assets and scenes still show old type identifiers from an earlier architecture. Do not assume authored wiring is fully normalized without checking current references.
- `MainMenu` and `PauseMenu` already demonstrate the intended screen and popup pattern.
- `Gameplay` HUD exists in code, but asset and prefab coverage appears less complete than `MainMenu` and `PauseMenu`. Verify authored wiring before depending on it.

## Output Expectations

When finishing work:

1. state which UI primitive and owning module were changed
2. name any files added or updated
3. list required Unity Editor wiring, if any
4. mention multiplayer implications when the UI can trigger gameplay state changes
5. mention the verification step that was run, or why it was not feasible

## Related Files

- `Assets/_Root/Scripts/UI/UIRoot.cs`
- `Assets/_Root/Scripts/UI/UIInstanceFactory.cs`
- `Assets/_Root/Scripts/UI/UIScreenNavigator.cs`
- `Assets/_Root/Scripts/UI/UIPopupCoordinator.cs`
- `Assets/_Root/Scripts/UI/UIBackRouter.cs`
- `Assets/_Root/Scripts/UI/UIBackInputListener.cs`
- `Assets/_Root/Scripts/UI/Base/UIViewBase.cs`
- `Assets/_Root/Scripts/UI/Base/UIScreenViewBase.cs`
- `Assets/_Root/Scripts/UI/Base/UIPopupViewBase.cs`
- `Assets/_Root/Scripts/UI/Base/UIScreenConfigBase.cs`
- `Assets/_Root/Scripts/UI/Base/UIPopupConfigBase.cs`
- `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuController.cs`
- `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuScreenView.cs`
- `Assets/_Root/Scripts/UI/Gameplay/Hud/UIHudController.cs`
- `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPauseMenuController.cs`
- `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPausePopupView.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlay.cs`
- `Assets/_Root/Scripts/DI/GlobalScope.cs`
- `Assets/_Root/Scripts/DI/MainMenuScope.cs`
- `Assets/_Root/Scripts/DI/GameplayScope.cs`
- `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs`

## Related Docs

- `docs/ai/coding-skills.md`
- `docs/project-map.md`
- `docs/unity/project-structure.md`
- `docs/unity/runtime-architecture-guidelines.md`

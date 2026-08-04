---
name: unity-ui-flow-review
description: Review or plan Unity UI flow in this project. Use when the user asks where a UI feature should live, whether it should be a screen, popup, HUD, or overlay, how to fit a change into `Assets/_Root/Scripts/UI/`, or whether a UI design matches the current project architecture.
---

# Unity UI Flow Review

Review UI changes against the project's current layered UI architecture before recommending code shape.

## Goals

- choose the correct UI primitive and ownership boundary
- protect the shared UI flow from scene-local shortcuts and duplicate managers
- catch back-flow, config, DI, and wiring mistakes before implementation
- keep recommendations aligned with the current `MainMenu`, `Gameplay`, and `LoadingScreen` patterns

## When To Use

Use this skill when asked to:

- review a UI architecture idea
- decide whether something is a screen, popup, HUD, or overlay
- decide whether logic belongs in shared flow or a scene slice
- check if a UI change fits the current VContainer and config wiring
- assess the impact of changing back handling, navigation, popup queueing, or scene transitions

Do not use this as the main skill for routine implementation. Use `unity-ui-flow-implementation` for code-writing work.

## Current UI Shape

Review proposals against these project facts:

- shared UI flow lives in `Assets/_Root/Scripts/UI/`
- reusable bases live in `Assets/_Root/Scripts/UI/Base/`
- scene-specific slices live in `Assets/_Root/Scripts/UI/MainMenu/`, `Assets/_Root/Scripts/UI/Gameplay/`, and `Assets/_Root/Scripts/UI/LoadingScreen/`
- global UI services are registered in `Assets/_Root/Scripts/DI/GlobalScope.cs`
- scene-specific configs and controllers are registered in `MainMenuScope.cs` and `GameplayScope.cs`
- prefab-backed runtime view creation goes through `UIInstanceFactory.cs`
- back flow goes through `UIBackRouter.cs`, not through ad hoc button-only logic

## Review Workflow

1. Identify the user-facing UI behavior being added or changed.
2. Classify it as `Screen`, `Popup`, `Hud`, `Overlay`, or not actually UI flow.
3. Check whether the owning module should be shared flow, `MainMenu`, `Gameplay`, or `LoadingScreen`.
4. Check whether the proposal matches the project's established `View + Controller + Config` pattern where applicable.
5. Check DI scope placement, config asset implications, and prefab wiring expectations.
6. Check back/history/queue semantics when the change affects navigation.
7. Call out any required Unity Editor work explicitly.

## Review Rules

- Prefer the existing layered UI flow over new generic UI managers.
- A full-scene state should usually be a `Screen`.
- An interrupting modal above another state should usually be a `Popup`.
- Persistent gameplay display should usually be a `Hud`.
- Transitional blocking UI should usually be an `Overlay`.
- Shared navigation, queueing, or back rules belong in the shared UI flow layer, not in scene-local views.
- Scene-specific button behavior and opening logic belong in the scene slice controller.
- View classes should stay focused on presentation and forwarding interaction.
- Treat the established `View + Controller + Config` pattern as guidance for fit, not a reason to add forwarding layers that do not clarify ownership.
- Distinguish simple local subscription lifecycles from UI flows that require explicit startup ownership or scene-entry ordering.
- If a proposal only adds forwarding wrappers, presenters, or services around one concrete UI flow, treat that as suspect.
- If the change can trigger gameplay state, authority, or network-visible effects, call out multiplayer review needs.

## Output Format

Return:

1. `Fit`: good fit, needs reshaping, or wrong layer
2. `Primitive`: screen, popup, HUD, overlay, or not a UI-flow concern
3. `Ownership`: shared flow or specific scene slice
4. `Main risk`: the biggest architectural or wiring risk
5. `Recommended shape`: the lightest structure that fits this project
6. `Editor work`: any prefab, config asset, or Inspector wiring needed

## Related Files

- `Assets/_Root/Scripts/UI/UIRoot.cs`
- `Assets/_Root/Scripts/UI/UIInstanceFactory.cs`
- `Assets/_Root/Scripts/UI/UIScreenNavigator.cs`
- `Assets/_Root/Scripts/UI/UIPopupCoordinator.cs`
- `Assets/_Root/Scripts/UI/UIBackRouter.cs`
- `Assets/_Root/Scripts/UI/Base/UIViewBase.cs`
- `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuController.cs`
- `Assets/_Root/Scripts/UI/Gameplay/UIGameplayController.cs`
- `Assets/_Root/Scripts/UI/Gameplay/Hud/UIHudController.cs`
- `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPauseMenuController.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlay.cs`
- `Assets/_Root/Scripts/DI/GlobalScope.cs`
- `Assets/_Root/Scripts/DI/MainMenuScope.cs`
- `Assets/_Root/Scripts/DI/GameplayScope.cs`

## Related Docs

- `docs/ai/coding-skills.md`
- `docs/project-map.md`
- `docs/unity/project-structure.md`
- `docs/unity/runtime-architecture-guidelines.md`

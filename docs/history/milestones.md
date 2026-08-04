# Milestones

## Purpose

This page records notable technical milestones that should not live only in chat history.

## Milestones

### Runtime composition and coding guidance refined from external agent notes

Status: completed

Summary:

- The project guidance now explicitly folds in composition-first gameplay rules, local event-wiring guidance, bug-diagnosis expectations, and verification expectations from external agent notes while preserving existing project-specific architecture constraints.

Impact:

- authored gameplay work now has a clearer documented stance on composition over inheritance without turning that preference into mechanical over-splitting
- local `Awake` plus `OnEnable`/`OnDisable` patterns are now documented as acceptable for simple scene-owned wiring without weakening the existing explicit-initialization rule for ordered startup
- bug-fix and implementation guidance now explicitly calls for checking multiple plausible causes and verifying meaningful changes when feasible

Key artifacts:

- `AGENTS.md`
- `docs/unity/runtime-architecture-guidelines.md`
- `docs/ai/assistant-entrypoint.md`
- `docs/project-map.md`
- `docs/unity/unity-composition-guide.md`
- `docs/ai/external-agent-guidance.md`

Related docs:

- `../unity/runtime-architecture-guidelines.md`
- `../ai/assistant-entrypoint.md`
- `../project-map.md`

### Spider locomotion stack intentionally removed pending rewrite

Status: completed

Summary:

- The authored spider movement, surface-detection, orientation, adhesion, and debug stack has been removed so the controller can be rebuilt from a minimal spawn-plus-camera baseline.

Impact:

- the preserved spider runtime boundary is now explicit: spawn still flows through `GameplayEntryPoint` and `SpiderPlayerSpawner`, while camera binding still flows through `CameraSpawner` and `CameraRig`
- `SpiderConfig` now only carries the spider prefab reference instead of stale movement tuning
- future spider-controller work can restart from a clean baseline without carrying forward the removed locomotion contracts or probe model

Key artifacts:

- startup orchestration: `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- spider runtime root: `Assets/_Root/Scripts/Gameplay/Spider/SpiderConfig.cs`, `Assets/_Root/Scripts/Gameplay/Spider/SpiderPlayerController.cs`, `Assets/_Root/Scripts/Gameplay/Spider/SpiderPlayerSpawner.cs`, `Assets/_Root/Scripts/Gameplay/Spider/PlayerSpawnPoint.cs`
- camera runtime slice: `Assets/_Root/Scripts/Gameplay/Camera/CameraRig.cs`, `Assets/_Root/Scripts/Gameplay/Camera/CameraSpawner.cs`
- removed runtime files: `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceComponent.cs`, `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceState.cs`, `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceHit.cs`, `Assets/_Root/Scripts/Gameplay/Spider/SpiderLookRotationComponent.cs`, `Assets/_Root/Scripts/Gameplay/Spider/SpiderMovementComponent.cs`, `Assets/_Root/Scripts/Gameplay/Spider/SpiderMovementResult.cs`, `Assets/_Root/Scripts/Test/GameplayTester.cs`

Related docs:

- `../project-map.md`
- `../unity/project-structure.md`
- `../unity/spider-player-controller-plan.md`

### Spider traversal camera up is now sourced from the player root

Status: completed

Summary:

- The gameplay camera bind flow now assigns `CinemachineBrain.WorldUpOverride` to the spawned spider root so wall and ceiling look controls use traversal-relative up, while the camera orbit binding mode remains prefab-authored.

Impact:

- looking on ceilings and walls now follows the spider's current traversal frame instead of staying pinned to world up
- gameplay code now owns only runtime follow/look binding and brain world-up wiring, while camera orbit behavior remains an authored camera-prefab decision
- camera tuning experiments such as `Lazy Follow` can now live on the prefab without being overwritten at startup

Key artifacts:

- startup orchestration: `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- camera runtime slice: `Assets/_Root/Scripts/Gameplay/Camera/CameraRig.cs`
- authored camera rig prefab: `Assets/_Root/Prefabs/pf_FreeLookCamera.prefab`

Related docs:

- `../project-map.md`
- `../unity/project-structure.md`
- `../unity/spider-player-controller-plan.md`

### Spider wall-transition stabilization and probe debugging established

Status: completed

Summary:

- The spider controller now uses a hybrid forward-plus-down surface probe flow with wall-takeover confirmation, smoothed local-up alignment, and editor gizmos for support debugging.

Impact:

- floor-to-wall transitions no longer switch support ownership on the first forward wall contact
- spider orientation now preserves body-tangent continuity while still blending toward camera-relative yaw on the new traversal plane
- runtime debugging of probe origin, forward/down sphere checks, sampled surface normal, and current reference up is now available directly in the Scene view

Key artifacts:

- support detection: `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceComponent.cs`
- local-up smoothing and gizmos: `Assets/_Root/Scripts/Gameplay/Spider/SpiderPlayerController.cs`
- transition-aware look rotation: `Assets/_Root/Scripts/Gameplay/Spider/SpiderLookRotationComponent.cs`
- transition tuning: `Assets/_Root/Scripts/Gameplay/Spider/SpiderConfig.cs`

Related docs:

- `../project-map.md`
- `../unity/project-structure.md`
- `../unity/spider-player-controller-plan.md`

### Spider orientation and hover adhesion established

Status: completed

Summary:

- The spider controller now has an explicit look-rotation rule component plus a configurable hover-style adhesion target above traversable surfaces.

Impact:

- spider forward now tracks the scene `Main Camera` movement-reference frame instead of relying only on the spider root's previous facing direction
- body up alignment and camera-facing body rotation now live in a dedicated `SpiderLookRotationComponent` boundary instead of the older generic orientation component name
- surface adhesion now supports a leg-like hover gap through config while still allowing outside forces to press the rigidbody closer to the surface

Key artifacts:

- orientation runtime: `Assets/_Root/Scripts/Gameplay/Spider/SpiderLookRotationComponent.cs`
- controller orchestration: `Assets/_Root/Scripts/Gameplay/Spider/SpiderPlayerController.cs`
- locomotion and adhesion tuning: `Assets/_Root/Scripts/Gameplay/Spider/SpiderMovementComponent.cs`, `Assets/_Root/Scripts/Gameplay/Spider/SpiderConfig.cs`

Related docs:

- `../project-map.md`
- `../unity/project-structure.md`
- `../unity/spider-player-controller-plan.md`

### Spider player controller roadmap and local implementation skill established

Status: completed

Summary:

- The repository now has a dedicated source-of-truth roadmap for the player-controlled spider plus a project-local skill for implementing that controller in staged increments.

Impact:

- future spider-player work can follow a stable implementation order instead of redesigning the controller each turn
- the project now has an explicit boundary between locomotion core, jump, web traversal, camera integration, and later IK work
- local agent guidance can now route spider-player tasks through a project-specific workflow instead of relying only on generic Unity implementation instructions

Key artifacts:

- `docs/unity/spider-player-controller-plan.md`
- `.opencode/skills/unity-spider-player-implementation/SKILL.md`
- `docs/ai/coding-skills.md`

Related docs:

- `../unity/spider-player-controller-plan.md`
- `../ai/coding-skills.md`
- `../project-map.md`

### Bootstrap scene and VContainer startup established

Status: completed

Summary:

- The project now starts through a dedicated bootstrap scene and a VContainer composition root.

Key artifacts:

- scene: `Assets/_Root/Scenes/Bootstrap.unity`
- scope: `Assets/_Root/Scripts/DI/GlobalScope.cs`
- entry point: `Assets/_Root/Scripts/Bootstrap/Bootstrap.cs`
- scene loading: `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs`
- build settings: `ProjectSettings/EditorBuildSettings.asset`

Impact:

- application startup is no longer coupled to opening `MainMenu.unity` directly
- the repository now has an explicit composition root for global runtime wiring
- additive scene loading is now part of the documented runtime startup path

Related docs:

- `../project-map.md`
- `../unity/project-structure.md`

### Project-local UI coding skills added

Status: completed

Summary:

- The repository now has dedicated OpenCode skills for project-specific UI flow implementation and UI architecture review.

Impact:

- agents can now trigger UI guidance that matches this repository's layered `Screen`/`Popup`/`Hud`/`Overlay` model instead of relying only on generic Unity feature instructions
- UI changes can more consistently land in the correct scope, module, and prefab-config wiring path
- review prompts about UI placement and back/navigation behavior now have a focused project-local skill target

Key artifacts:

- `.opencode/skills/unity-ui-flow-implementation/SKILL.md`
- `.opencode/skills/unity-ui-flow-review/SKILL.md`
- `docs/ai/coding-skills.md`

Related docs:

- `../ai/coding-skills.md`
- `../project-map.md`
- `../unity/runtime-architecture-guidelines.md`

### CI/CD module established

Status: completed

Summary:

- A GitHub Actions workflow now builds a Unity WebGL player and deploys it to GitHub Pages.

Key artifacts:

- workflow: `.github/workflows/deploy-pages.yml`

Impact:

- the repository now has an automated WebGL delivery path
- CI/CD is now a documented project system and should be updated when the workflow changes

Related docs:

- `../systems/ci-cd.md`
- `../project-map.md`

### Project knowledge base established

Status: completed

Summary:

- A Markdown-based technical knowledge base was added under `docs/`.

Impact:

- project memory no longer depends only on chat history
- humans and coding agents now have shared documentation entrypoints
- documentation maintenance is now part of normal project work

Key artifacts:

- `docs/index.md`
- `docs/project-map.md`
- `docs/ai/assistant-entrypoint.md`
- `docs/systems/ci-cd.md`
- `docs/unity/project-structure.md`

Related docs:

- `../index.md`
- `../workflows/updating-docs.md`

### Platform strategy documented separately from the current WebGL pipeline

Status: completed

Summary:

- The knowledge base now distinguishes the temporary WebGL delivery workflow from the project's long-term mobile-plus-PC platform direction.

Impact:

- coding agents should no longer treat WebGL as the default long-term architectural target
- current deployment facts remain documented without overriding long-term platform intent

Key artifacts:

- `docs/systems/platform-strategy.md`
- `docs/ai/assistant-entrypoint.md`
- `docs/project-map.md`
- `docs/systems/ci-cd.md`

Related docs:

- `../systems/platform-strategy.md`
- `../systems/ci-cd.md`
- `../project-map.md`

### Local ScriptableObject config layer established

Status: completed

Summary:

- The runtime architecture now includes a local `ScriptableObject` config root for shared authored values.

Impact:

- shared authored values can start moving out of scene components into a bootstrap-wired config layer
- the current startup graph now includes a local config dependency registered in `GlobalScope`
- config branches are now modeled as separate `ScriptableObject` asset types instead of nested serializable classes

Key artifacts:

- `Assets/_Root/Scripts/Configs/ProjectConfig.cs`
- `Assets/_Root/Scripts/UI/UIConfig.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayConfig.cs`
- `Assets/_Root/Scripts/DI/GlobalScope.cs`
- `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlay.cs`

Related docs:

- `../project-map.md`
- `../unity/project-structure.md`
- `../unity/runtime-architecture-guidelines.md`

### Reactive input module established

Status: completed

Summary:

- The runtime architecture now includes a dedicated reactive input slice built on top of Unity Input System and R3.

Key artifacts:

- ownership: `Assets/_Root/Scripts/Input/InputActionsProvider.cs`
- action adapters: `Assets/_Root/Scripts/Input/InputActionObservableExtensions.cs`
- player API: `Assets/_Root/Scripts/Input/Player/IPlayerInputStreams.cs`
- player streams: `Assets/_Root/Scripts/Input/Player/PlayerInputStreams.cs`
- bootstrap wiring: `Assets/_Root/Scripts/DI/GlobalScope.cs`

Impact:

- Unity `InputAction` details are now contained inside a dedicated module boundary
- player input can now be consumed as reactive streams instead of ad-hoc callbacks
- startup wiring now enables input maps centrally through the composition root

Related docs:

- `../project-map.md`
- `../unity/runtime-architecture-guidelines.md`

### Authored script folders reorganized into top-level slices

Status: completed

Summary:

- Authored runtime scripts under `Assets/_Root/Scripts/` were reorganized so bootstrap, DI, scene-loading, config, input, UI, editor, and test-style code now live in separate top-level folders.

Impact:

- docs and agent retrieval can now reason about script ownership from folder layout more directly
- config asset types are no longer documented as part of the bootstrap architecture folder
- UI runtime components now have an explicit home separate from startup code
- composition roots, scene loading, and runtime input now have clearer top-level homes

Key artifacts:

- `Assets/_Root/Scripts/Bootstrap/`
- `Assets/_Root/Scripts/DI/`
- `Assets/_Root/Scripts/SceneLoading/`
- `Assets/_Root/Scripts/Configs/`
- `Assets/_Root/Scripts/Input/`
- `Assets/_Root/Scripts/UI/`
- `Assets/_Root/Scripts/Editor/`
- `Assets/_Root/Scripts/Test/`

Related docs:

- `../project-map.md`
- `../ai/assistant-entrypoint.md`
- `../unity/project-structure.md`
- `../unity/runtime-architecture-guidelines.md`

### Namespace and assembly conventions established

Status: completed

Summary:

- The project now has an explicit rule for short `Pet`-rooted namespaces and a baseline authored assembly layout.

Impact:

- future code should stay readable without deep namespace nesting
- architectural isolation should now be expressed primarily through `asmdef` boundaries instead of long namespace paths
- editor-only code now has a dedicated authored assembly home

Key artifacts:

- `AGENTS.md`
- `docs/unity/runtime-architecture-guidelines.md`
- `Assets/_Root/Scripts/Pet.Runtime.asmdef`
- `Assets/_Root/Scripts/Editor/Pet.Editor.asmdef`

Related docs:

- `../ai/assistant-entrypoint.md`
- `../unity/runtime-architecture-guidelines.md`

### Runtime UI flow foundation established

Status: completed

Summary:

- The runtime UI layer now has shared screen navigation, popup queuing, back-routing, and prefab-config-driven view instantiation.

Impact:

- UI flow can now grow around explicit screen, popup, HUD, and overlay layers instead of a single global UI manager
- `MainMenu` and `Gameplay` now plug into the same UI flow primitives while keeping scene-specific behavior in focused controllers
- scene transitions between `MainMenu` and `Gameplay` now have a dedicated content-scene switch path in `SceneLoader`

Key artifacts:

- UI root and bases: `Assets/_Root/Scripts/UI/UIRoot.cs`, `Assets/_Root/Scripts/UI/Base/UIViewBase.cs`, `Assets/_Root/Scripts/UI/Base/UIScreenViewBase.cs`, `Assets/_Root/Scripts/UI/Base/UIPopupViewBase.cs`
- UI configs and runtime flow: `Assets/_Root/Scripts/UI/Base/UIScreenConfigBase.cs`, `Assets/_Root/Scripts/UI/Base/UIPopupConfigBase.cs`, `Assets/_Root/Scripts/UI/UIConfig.cs`, `Assets/_Root/Scripts/UI/UIInstanceFactory.cs`, `Assets/_Root/Scripts/UI/UIScreenNavigator.cs`, `Assets/_Root/Scripts/UI/UIPopupCoordinator.cs`, `Assets/_Root/Scripts/UI/UIBackRouter.cs`, `Assets/_Root/Scripts/UI/UIBackInputListener.cs`, `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlay.cs`, `Assets/_Root/Scripts/UI/LoadingScreen/UILoadingOverlayController.cs`
- feature flow: `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuController.cs`, `Assets/_Root/Scripts/UI/MainMenu/UIMainMenuScreenView.cs`, `Assets/_Root/Scripts/UI/Gameplay/Hud/UIHudController.cs`, `Assets/_Root/Scripts/UI/Gameplay/UIGameplayController.cs`, `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPauseMenuController.cs`, `Assets/_Root/Scripts/UI/Gameplay/PauseMenu/UIPausePopupView.cs`
- scene switching: `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs`

Related docs:

- `../project-map.md`
- `../unity/runtime-architecture-guidelines.md`

### Authored content layout expanded beyond scenes, prefabs, and scripts

Status: completed

Summary:

- `Assets/_Root/` now has explicit top-level homes for authored config assets, models, materials, and animation content in addition to scenes, prefabs, scripts, and settings.

Impact:

- docs and agent retrieval can now distinguish config asset instances from config type definitions
- authored non-code assets now have stable top-level locations instead of being implicit or undocumented
- Unity layout docs should treat `Assets/_Root/Configs/` as the asset home while config type definitions remain in `Assets/_Root/Scripts/`

Key artifacts:

- `Assets/_Root/Configs/`
- `Assets/_Root/Models/`
- `Assets/_Root/Materials/`
- `Assets/_Root/Animations/`
- `Assets/_Root/Prefabs/UI/`

Related docs:

- `../project-map.md`
- `../unity/project-structure.md`

### Spider controller stages 1 and 2 established

Status: completed

Summary:

- The project now has the first authored player-spider runtime stack with explicit prefab-driven spawn flow, input-owned controller root, and stage-2 five-probe surface detection.

Impact:

- spider-controller work can now build forward from a stable root without revisiting ownership of spawn, config, and input initialization
- surface orientation, adhesion, and movement stages now have a concrete `CurrentSurfaceState` boundary to build on
- testing surface support became easier through the lightweight `GameplayTester` debug log entry point

Key artifacts:

- startup and DI: `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`, `Assets/_Root/Scripts/DI/GameplayScope.cs`
- spider runtime root: `Assets/_Root/Scripts/Gameplay/Spider/SpiderConfig.cs`, `Assets/_Root/Scripts/Gameplay/Spider/SpiderPlayerController.cs`, `Assets/_Root/Scripts/Gameplay/Spider/SpiderPlayerSpawner.cs`, `Assets/_Root/Scripts/Gameplay/Spider/PlayerSpawnPoint.cs`
- surface detection: `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceComponent.cs`, `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceState.cs`, `Assets/_Root/Scripts/Gameplay/Spider/SpiderSurfaceHit.cs`
- debug support: `Assets/_Root/Scripts/Test/GameplayTester.cs`

Related docs:

- `../project-map.md`
- `../unity/runtime-architecture-guidelines.md`
- `../unity/spider-player-controller-plan.md`

### Explicit content-scene startup established

Status: completed

Summary:

- `MainMenu` and `Gameplay` scene startup now runs through explicit scene-scoped entry points invoked by `SceneLoader` after `SetActiveScene`, instead of scene-level `IAsyncStartable` auto-start.

Impact:

- additive scene transitions now have a deterministic startup handoff owned by `SceneLoader`
- runtime-spawned gameplay objects such as the spider player can initialize after the target content scene becomes active
- scene-specific input-map setup, UI startup, and gameplay startup no longer depend on VContainer PlayerLoop timing for content scenes

Key artifacts:

- startup contract: `Assets/_Root/Scripts/SceneLoading/ISceneEntryPoint.cs`
- startup orchestration: `Assets/_Root/Scripts/SceneLoading/SceneLoader.cs`
- scene entry points: `Assets/_Root/Scripts/Bootstrap/MainMenuEntryPoint.cs`, `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- scene scope registrations: `Assets/_Root/Scripts/DI/MainMenuScope.cs`, `Assets/_Root/Scripts/DI/GameplayScope.cs`

Related docs:

- `../project-map.md`
- `../unity/project-structure.md`
- `../unity/runtime-architecture-guidelines.md`

### Gameplay camera spawn flow established

Status: completed

Summary:

- `Gameplay` scene startup now includes explicit runtime spawn of a Cinemachine 3 camera rig from authored config, followed by target binding to the spawned spider player.

Impact:

- gameplay camera ownership now follows the same explicit prefab-backed startup style as the spider player instead of depending on scene-owned target references
- camera target binding is now an authored spider-prefab concern through dedicated follow and look target references
- camera input responsibility is now clearly split: project gameplay code owns player input, while the camera prefab is expected to own its Cinemachine input-controller wiring

Key artifacts:

- startup orchestration: `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- scene scope registration: `Assets/_Root/Scripts/DI/GameplayScope.cs`
- camera runtime slice: `Assets/_Root/Scripts/Gameplay/Camera/CameraConfig.cs`, `Assets/_Root/Scripts/Gameplay/Camera/CameraRig.cs`, `Assets/_Root/Scripts/Gameplay/Camera/CameraSpawner.cs`
- spider camera targets: `Assets/_Root/Scripts/Gameplay/Spider/SpiderPlayerController.cs`
- runtime assembly reference: `Assets/_Root/Scripts/Pet.Runtime.asmdef`

Related docs:

- `../project-map.md`
- `../unity/runtime-architecture-guidelines.md`
- `../unity/spider-player-controller-plan.md`

### Spider movement now uses scene Main Camera forward

Status: completed

Summary:

- Spider locomotion is now camera-relative by default: gameplay startup resolves the scene `Main Camera`, passes its transform into the spawned spider controller, and movement projects that camera frame onto the active traversal plane.

Impact:

- floor, wall, and ceiling movement now follows the player's current gameplay view instead of the spider root's own forward axis
- camera ownership stays split cleanly: the spawned `CameraRig` still owns Cinemachine follow/look binding, while the scene `Main Camera` is the explicit locomotion input frame
- the movement core now has a documented fallback for near-degenerate cases where camera forward approaches the surface normal

Key artifacts:

- startup orchestration: `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`
- scene scope registration: `Assets/_Root/Scripts/DI/GameplayScope.cs`
- locomotion runtime: `Assets/_Root/Scripts/Gameplay/Spider/SpiderMovementComponent.cs`
- spider controller runtime: `Assets/_Root/Scripts/Gameplay/Spider/SpiderPlayerController.cs`

Related docs:

- `../project-map.md`
- `../unity/spider-player-controller-plan.md`

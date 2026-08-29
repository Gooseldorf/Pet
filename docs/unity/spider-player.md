# Spider Player

The first spider locomotion slice controls the body only. It uses a dynamic Rigidbody to traverse static, physically reachable `Traversable` surfaces, including walls, ceilings, curved geometry, and compatible seams. The body falls when no surface remains and automatically reattaches when an eligible surface enters its search range. Moving and rotating supports, jump, air control, sprint, web behaviour, and leg stepping are deliberately outside this slice.

`SpiderPlayerController` owns the one explicit physics tick. Its surface detector queries only physical sides of eligible static colliders, selects a proximity-weighted, mutually compatible group of contacts, and exposes the resulting read-only support state. A contact must be compatible with every member of the selected group so nearby opposing surfaces are never averaged together. Its locomotion motor moves tangent to that surface, holds the authored collider offset, and aligns the body up with the support normal while following the camera heading. Do not split those tightly ordered stages into independent MonoBehaviour callbacks without a new approved reason.

Movement input is interpreted in the body frame: the body forward follows the camera heading projected onto the attached surface, while lateral and reverse input remain strafe and reverse movement. Tune traversal through `SpiderConfig`; enable `SpiderSurfaceVisualizer` on the spider prefab when adjusting a traversal environment or authored body offset. The visualizer renders contact diagnostics at runtime and does not extend the public surface-state API.

The public surface state is the integration boundary for future systems such as legs. It reports whether the body is attached, the blended support point and normal, and the contributing colliders. Consumers observe it and must not own body movement, surface sampling, or controller tick order.

The preserved boundary is explicit runtime spawn: gameplay startup creates the spider and its camera rig, then the rig binds to authored follow and look targets on the spawned spider. Keep camera ownership in the camera slice and do not replace that binding with scene searches or scene-owned target references.

The current spider scope is single-player. Do not infer networking support; evaluate authority and synchronization only when multiplayer is introduced or requested.

Repository authorities: `Assets/_Root/Scripts/Gameplay/Spider/`, `Assets/_Root/Scripts/Gameplay/Camera/`, `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`, `Assets/_Root/Scripts/DI/GameplayScope.cs`, and the relevant prefab and config assets.

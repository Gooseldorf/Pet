# Spider Player

Spider locomotion is intentionally undefined while the controller is rewritten. Before implementing traversal, orientation, jump, web, or controller architecture, inspect the current spider, camera, input, spawn, prefab, and config owners and obtain an approved design for any new architecture.

The preserved boundary is explicit runtime spawn: gameplay startup creates the spider and its camera rig, then the rig binds to authored follow and look targets on the spawned spider. Keep camera ownership in the camera slice and do not replace that binding with scene searches or scene-owned target references.

The current spider scope is single-player. Do not infer networking support; evaluate authority and synchronization only when multiplayer is introduced or requested.

Repository authorities: `Assets/_Root/Scripts/Gameplay/Spider/`, `Assets/_Root/Scripts/Gameplay/Camera/`, `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`, `Assets/_Root/Scripts/DI/GameplayScope.cs`, and the relevant prefab and config assets.

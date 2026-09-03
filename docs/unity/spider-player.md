# Spider Player

`SpiderPlayerController` owns the physics tick for WASD movement. It converts input to a horizontal direction using the movement camera's forward and right axes, then sets the Rigidbody's horizontal velocity from `SpiderConfig`. It has no surface detection, adhesion, jumping, or leg-stepping behavior.

The preserved boundary is explicit runtime spawn: gameplay startup creates the spider and its camera rig, then the rig binds to authored follow and look targets on the spawned spider. Keep camera ownership in the camera slice and do not replace that binding with scene searches or scene-owned target references.

The current spider scope is single-player. Do not infer networking support; evaluate authority and synchronization only when multiplayer is introduced or requested.

Repository authorities: `Assets/_Root/Scripts/Gameplay/Spider/`, `Assets/_Root/Scripts/Gameplay/Camera/`, `Assets/_Root/Scripts/Bootstrap/GameplayEntryPoint.cs`, `Assets/_Root/Scripts/DI/GameplayScope.cs`, and the relevant prefab and config assets.

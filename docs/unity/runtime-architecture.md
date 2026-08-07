# Runtime Architecture

Read this page for startup, DI, additive scene loading, runtime spawning, or ordered initialization. Inspect the affected code and serialized assets for current details.

`Bootstrap` owns application startup through the global VContainer scope. Content scenes load additively through `SceneLoader`, become active before their scene scope resolves `ISceneEntryPoint`, and initialize through that explicit entry point. This prevents content-scene startup from depending on scene-level automatic entry-point timing.

`GlobalScope` owns persistent application dependencies, including the root configuration and shared UI root. Scene scopes own their scene-specific registrations. Authored configuration is `ScriptableObject`-based; config types and config assets are kept separate, then injected from the owning scope.

Runtime objects instantiated from prefabs are owned by explicit small spawn flows. Preserve that ownership rather than moving runtime references into scenes or adding a factory without a concrete need. Use explicit initialization when order matters; local enable/disable subscriptions remain suitable for simple local lifecycles.

Repository authorities: `Assets/_Root/Scripts/Bootstrap/`, `DI/`, `SceneLoading/`, `Configs/`, and the affected UI or gameplay slice. Serialized integration is owned by the corresponding scene, prefab, or config asset.

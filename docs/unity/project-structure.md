# Unity Project Structure

## Purpose

This page documents the current Unity-authored structure of the repository.

## Scope

This is a project-layout document, not a full gameplay architecture document.

## Source Of Truth

- `Assets/_Root/`
- `Assets/Settings/`
- `Packages/manifest.json`
- `ProjectSettings/`

## Current Implementation

### Main Unity Directories

| Path | Purpose |
| --- | --- |
| `Assets/_Root/` | Main authored project content |
| `Assets/Settings/` | Shared project settings assets such as build profiles |
| `Packages/` | Unity package dependencies |
| `ProjectSettings/` | Unity editor and project configuration |

### Authored Content Under `Assets/_Root/`

| Path | Purpose |
| --- | --- |
| `Assets/_Root/Scenes/` | Scenes |
| `Assets/_Root/Prefabs/` | Prefabs |
| `Assets/_Root/Scripts/` | C# scripts |
| `Assets/_Root/Settings/` | Authored Unity settings assets |

## Current Known Assets

### Scenes

- `Assets/_Root/Scenes/MainMenu.unity`

### Prefabs

- `Assets/_Root/Prefabs/pf_MainMenuButton.prefab`

### Scripts

- `Assets/_Root/Scripts/Test/MainMenuTester.cs`

### Build Profiles

- `Assets/Settings/Build Profiles/Web - Desktop - Release.asset`

## Current Script Notes

`Assets/_Root/Scripts/Test/MainMenuTester.cs` is the current authored script.

Observed characteristics:

- namespace: `Test`
- serialized references for `TextMeshProUGUI` and `Button`
- simple counter behavior driven by UI button events

## Package Notes

`Packages/manifest.json` currently includes, among others:

- `com.unity.inputsystem`
- `com.unity.multiplayer.center`
- `com.unity.render-pipelines.universal`
- `com.unity.test-framework`
- `com.unity.ugui`

This indicates the project already has package-level support for UI, URP, input, testing, and multiplayer-related tooling, even though authored gameplay systems are still minimal.

## Conventions

- authored content is grouped under `Assets/_Root/`
- prefab names currently use the `pf_` prefix
- required object references should generally be wired through serialized fields
- project-specific Unity coding constraints are defined in `AGENTS.md`

## Related Files

- `Assets/_Root/Scripts/Test/MainMenuTester.cs`
- `Assets/_Root/Scenes/MainMenu.unity`
- `Assets/_Root/Prefabs/pf_MainMenuButton.prefab`
- `Assets/Settings/Build Profiles/Web - Desktop - Release.asset`
- `Packages/manifest.json`
- `ProjectSettings/ProjectVersion.txt`

## Related Docs

- `../project-map.md`
- `../systems/ci-cd.md`

## Open Questions

- There is not yet an established long-term runtime code organization beyond `Assets/_Root/Scripts/`.
- There are not yet formal documented scene/prefab ownership conventions beyond the currently observed naming pattern.

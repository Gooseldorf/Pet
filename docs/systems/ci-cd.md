# CI/CD

## Purpose

This page describes the current automated build and deployment pipeline for the project.

## Scope

Current scope covers the GitHub Actions workflow that builds a WebGL player and deploys it to GitHub Pages.

This document is the source of truth for the current pipeline, not for the long-term platform strategy.

## Source Of Truth

- `.github/workflows/deploy-pages.yml`
- `ProjectSettings/ProjectVersion.txt`

## Current Implementation

The current workflow is `Build and Deploy Pages` in `.github/workflows/deploy-pages.yml`.

It performs two jobs:

1. `build`
2. `deploy`

### Triggers

- `push` to branch `WebGLBuild`
- `workflow_dispatch`

### Build Job

The build job currently does the following:

1. checks out the repository with LFS enabled
2. restores or populates a cache for `Library`
3. configures GitHub Pages
4. builds a Unity WebGL player with `game-ci/unity-builder`
5. uploads the `build/WebGL` artifact for Pages deployment

### Deploy Job

The deploy job:

1. waits for the build job
2. deploys the uploaded Pages artifact through `actions/deploy-pages`

## Build Configuration

Current key settings:

- target platform: `WebGL`
- output root: `build`
- deployed artifact path: `build/WebGL`
- configured workflow branch trigger: `WebGLBuild`

## Required Secrets

The workflow currently expects the following GitHub secrets:

- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`
- `UNITY_SERIAL`

## Operational Notes

- The workflow uses `ubuntu-latest`.
- `Library` caching key currently depends on `Assets/**`, `Packages/**`, and `ProjectSettings/**`.
- `concurrency` is configured per ref and cancels in-progress runs for the same ref.
- GitHub Pages is currently used for the built WebGL player, not for a docs site.
- The workflow does not currently reference a committed Unity build profile asset.

## Failure Modes

Typical failure sources to check first:

- invalid or missing Unity license secrets
- GitHub Pages permissions or environment issues
- Unity build failures inside `game-ci/unity-builder`
- stale cache requiring a cache key change or manual invalidation
- LFS-dependent assets not being available if checkout configuration changes

## Related Files

- `.github/workflows/deploy-pages.yml`
- `ProjectSettings/ProjectVersion.txt`

## Related Docs

- `../project-map.md`
- `platform-strategy.md`
- `../workflows/updating-docs.md`
- `../history/milestones.md`

## Open Questions

- The repository does not yet document the public Pages URL as a stable project document.
- There is no documented release workflow beyond this WebGL Pages deployment path yet.

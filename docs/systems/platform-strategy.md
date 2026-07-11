# Platform Strategy

## Purpose

This page records the current platform direction so humans and coding agents do not infer long-term product strategy from the current deployment workflow alone.

## Scope

This page covers target platform direction and how to interpret the current WebGL delivery path.

It is not the source of truth for the current CI workflow details.

## Source Of Truth

- `docs/project-map.md`
- `docs/systems/ci-cd.md`
- `Packages/manifest.json`
- user direction captured during project work

## Current Platform Direction

- Long-term target platforms are mobile and PC.
- The current WebGL build and GitHub Pages deployment path is a temporary delivery path for the project's current stage.
- WebGL should not be treated as the default architectural driver when evaluating long-term runtime systems.

## Current Interpretation Rules

- Use WebGL constraints when the task is specifically about the current deployment workflow, browser runtime limitations, or the currently shipped build.
- Do not let WebGL-specific limitations dominate general architecture decisions when the task is about long-term gameplay, runtime systems, performance direction, or platform strategy.
- When there is a tradeoff between a WebGL-friendly design and a mobile-plus-PC-oriented design, prefer the mobile and PC direction unless the task explicitly targets the current WebGL delivery path.

## Current Delivery Reality

- The repository currently has a WebGL deployment workflow through GitHub Pages.
- The workflow is defined in `.github/workflows/deploy-pages.yml`.
- The current delivery workflow remains relevant operationally until a different release path replaces it.

## Architectural Implications

- Future runtime architecture can assume likely growth toward mobile and PC constraints.
- Systems that would later rely on ECS, jobs, or multithreading should not be rejected only because WebGL is the current delivery path.
- Platform-sensitive decisions should distinguish between temporary iteration constraints and long-term product direction.

## Related Files

- `.github/workflows/deploy-pages.yml`
- `Packages/manifest.json`
- `docs/project-map.md`
- `docs/systems/ci-cd.md`

## Related Docs

- `ci-cd.md`
- `../project-map.md`
- `../ai/assistant-entrypoint.md`

# Platform Strategy

- Long-term target platforms are mobile and PC.
- The current WebGL and GitHub Pages path is temporary delivery infrastructure, not the default architectural driver.
- Apply WebGL constraints when the task concerns browser behavior or that delivery path. Otherwise prefer the mobile-plus-PC direction when a tradeoff is explicit.

Read [the Pages workflow](../../.github/workflows/deploy-pages.yml) for current delivery facts. This page does not prescribe ECS, jobs, threading, or other implementation strategy.

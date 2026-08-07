# CI/CD

The Pages workflow is a delivery concern, not a general validation contract. Read [the workflow](../../.github/workflows/deploy-pages.yml) for its exact triggers, actions, inputs, paths, permissions, and credentials.

The current workflow builds and deploys a WebGL player to GitHub Pages. WebGL delivery does not define the long-term platform direction; see [platform strategy](platform-strategy.md).

There is no supported automated Unity compile or test command yet. `tools/validate.ps1 -Mode Context` validates only agent-context artifacts. Do not treat generated project files, archived logs, or a deployment run as a local validation contract.

For a workflow failure, inspect the failed workflow's action output first. Check Unity license access, LFS checkout, Unity build failure details, Pages permissions, and cache behavior as applicable. Exact remediation depends on the current YAML and workflow run.

# Unity Refactor Smells And Moves

Use these patterns when diagnosing awkward Unity code.

## Common Smells

- `MonoBehaviour` mixes UI wiring, gameplay rules, formatting, and persistence details.
- A caller must know too much about call order or hidden setup steps.
- One feature change forces edits across several unrelated files.
- A helper exists only to forward calls or rename a method.
- Special-case conditionals keep leaking upward into button handlers or scene glue.
- Multiple classes manipulate the same conceptual state with no clear owner.

## Preferred Moves

- Move hidden bookkeeping into the class that owns the behavior.
- Replace step-by-step calling patterns with one stronger semantic method.
- Merge shallow wrapper classes back into the deeper owner.
- Extract a plain C# helper only when it removes non-Unity rules from scene glue.
- Isolate exceptional behavior behind a dedicated method instead of scattering `if` branches.
- Tighten naming until class and method names reflect the real abstraction, not the mechanism.

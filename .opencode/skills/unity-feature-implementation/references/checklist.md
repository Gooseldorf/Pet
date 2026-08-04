# Unity Feature Checklist

Use this checklist before finishing a Unity feature change.

- Is the behavior owned by the right `MonoBehaviour` or helper?
- Does the design prefer composition without mechanically splitting a small clear owner?
- Are required references wired through serialized fields where possible?
- Did the change avoid runtime hierarchy search and unnecessary `GetComponent` chains?
- Did the code stay small enough to read in one pass?
- If UI is involved, does the UI class only coordinate presentation and interaction?
- If logic moved into a helper, did that split clearly reduce Unity glue in the main class?
- If this was a bug fix, were multiple plausible causes checked before the fix was chosen?
- If scene or prefab wiring is needed, did you say exactly what to assign in the Inspector?
- If multiplayer could be affected, did you review authority, RPC flow, and desync risk?
- Was the most relevant feasible verification step run after the change?

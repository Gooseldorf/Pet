# Unity Feature Checklist

Use this checklist before finishing a Unity feature change.

- Is the behavior owned by the right `MonoBehaviour` or helper?
- Are required references wired through serialized fields where possible?
- Did the change avoid runtime hierarchy search and unnecessary `GetComponent` chains?
- Did the code stay small enough to read in one pass?
- If UI is involved, does the UI class only coordinate presentation and interaction?
- If logic moved into a helper, did that split clearly reduce Unity glue in the main class?
- If scene or prefab wiring is needed, did you say exactly what to assign in the Inspector?
- If multiplayer could be affected, did you review authority, RPC flow, and desync risk?

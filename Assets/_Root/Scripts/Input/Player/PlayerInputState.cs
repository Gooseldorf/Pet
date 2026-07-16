using UnityEngine;

namespace Architecture.Input.Player
{
    public readonly struct PlayerInputState
    {
        public PlayerInputState(Vector2 move, Vector2 look, bool sprintHeld, bool crouchHeld)
        {
            Move = move;
            Look = look;
            SprintHeld = sprintHeld;
            CrouchHeld = crouchHeld;
        }

        public Vector2 Move { get; }
        public Vector2 Look { get; }
        public bool SprintHeld { get; }
        public bool CrouchHeld { get; }
    }
}

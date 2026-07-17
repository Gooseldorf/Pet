using R3;
using UnityEngine;

namespace Pet.Input
{
    public interface IPlayerInputStreams
    {
        PlayerInputState CurrentState { get; }

        Observable<Vector2> Move { get; }
        Observable<Vector2> Look { get; }
        Observable<bool> SprintHeld { get; }
        Observable<bool> CrouchHeld { get; }

        Observable<Unit> JumpPressed { get; }
        Observable<Unit> AttackPressed { get; }
        Observable<Unit> InteractStarted { get; }
        Observable<Unit> InteractCanceled { get; }
        Observable<Unit> PreviousPressed { get; }
        Observable<Unit> NextPressed { get; }
    }
}

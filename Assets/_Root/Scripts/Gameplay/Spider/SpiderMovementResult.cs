using UnityEngine;

namespace Pet.Gameplay
{
    public readonly struct SpiderMovementResult
    {
        public SpiderMovementResult(Vector3 linearVelocity)
        {
            LinearVelocity = linearVelocity;
        }

        public Vector3 LinearVelocity { get; }
    }
}

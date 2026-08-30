using UnityEngine;

namespace Pet.Gameplay
{
    public readonly struct SpiderLegSurfaceContact
    {
        public SpiderLegSurfaceContact(Collider collider, Vector3 point, Vector3 normal, float score)
        {
            Collider = collider;
            Point = point;
            Normal = normal;
            Score = score;
        }

        public Collider Collider { get; }
        public Vector3 Point { get; }
        public Vector3 Normal { get; }
        public float Score { get; }
    }
}

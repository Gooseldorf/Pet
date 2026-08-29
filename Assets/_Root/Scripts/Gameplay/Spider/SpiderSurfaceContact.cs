using UnityEngine;

namespace Pet.Gameplay
{
    internal readonly struct SpiderSurfaceContact
    {
        public SpiderSurfaceContact(Collider collider, Vector3 point, Vector3 normal, float distance)
        {
            Collider = collider;
            Point = point;
            Normal = normal;
            Distance = distance;
        }

        public Collider Collider { get; }
        public Vector3 Point { get; }
        public Vector3 Normal { get; }
        public float Distance { get; }
    }
}

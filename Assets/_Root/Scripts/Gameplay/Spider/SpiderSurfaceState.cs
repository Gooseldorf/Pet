using System.Collections.Generic;
using UnityEngine;

namespace Pet.Gameplay
{
    public sealed class SpiderSurfaceState
    {
        private readonly List<Collider> colliders = new();

        public bool IsAttached { get; private set; }
        public Vector3 Point { get; private set; }
        public Vector3 Normal { get; private set; } = Vector3.up;
        public IReadOnlyList<Collider> Colliders => colliders;

        internal void SetAttached(Vector3 point, Vector3 normal, SpiderSurfaceContact[] contacts, int contactCount)
        {
            IsAttached = true;
            Point = point;
            Normal = normal;
            colliders.Clear();

            for (int contactIndex = 0; contactIndex < contactCount; contactIndex++)
            {
                Collider collider = contacts[contactIndex].Collider;

                if (!colliders.Contains(collider))
                {
                    colliders.Add(collider);
                }
            }
        }

        internal void SetAirborne()
        {
            IsAttached = false;
            colliders.Clear();
        }
    }
}

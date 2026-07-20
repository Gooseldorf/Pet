using UnityEngine;

namespace Pet.Gameplay
{
    public readonly struct SpiderSurfaceState
    {
        public SpiderSurfaceState(
            bool hasSurface,
            bool isStableSurface,
            Vector3 surfaceNormal,
            Vector3 surfacePoint,
            float surfaceDistance,
            SpiderSurfaceHit primaryHit,
            int hitCount)
        {
            HasSurface = hasSurface;
            IsStableSurface = isStableSurface;
            SurfaceNormal = surfaceNormal;
            SurfacePoint = surfacePoint;
            SurfaceDistance = surfaceDistance;
            PrimaryHit = primaryHit;
            HitCount = hitCount;
        }

        public bool HasSurface { get; }
        public bool IsStableSurface { get; }
        public Vector3 SurfaceNormal { get; }
        public Vector3 SurfacePoint { get; }
        public float SurfaceDistance { get; }
        public SpiderSurfaceHit PrimaryHit { get; }
        public int HitCount { get; }
    }
}

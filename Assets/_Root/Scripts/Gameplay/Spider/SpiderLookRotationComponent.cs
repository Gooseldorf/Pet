using UnityEngine;

namespace Pet.Gameplay
{
    public class SpiderLookRotationComponent
    {
        private const float MIN_DIRECTION_MAGNITUDE = 0.0001f;

        private readonly SpiderConfig config;

        public SpiderLookRotationComponent(SpiderConfig config)
        {
            this.config = config;
        }

        public Quaternion Evaluate(
            Transform orientationRoot,
            SpiderSurfaceState surfaceState,
            Transform movementReference,
            Vector3 referenceUp,
            float fixedDeltaTime)
        {
            Vector3 targetUp = surfaceState.HasSurface && surfaceState.IsStableSurface
                ? surfaceState.SurfaceNormal
                : referenceUp;

            Vector3 targetForward = ResolvePlanarForward(
                movementReference.forward,
                movementReference.right,
                orientationRoot.forward,
                orientationRoot.right,
                targetUp);

            Quaternion targetRotation = Quaternion.LookRotation(targetForward, targetUp);
            float interpolationFactor = 1f - Mathf.Exp(-config.OrientationSharpness * fixedDeltaTime);
            return Quaternion.Slerp(orientationRoot.rotation, targetRotation, interpolationFactor);
        }

        private Vector3 ResolvePlanarForward(
            Vector3 preferredForward,
            Vector3 preferredRight,
            Vector3 fallbackForward,
            Vector3 fallbackRight,
            Vector3 up)
        {
            Vector3 projectedForward = Vector3.ProjectOnPlane(preferredForward, up);

            if (projectedForward.sqrMagnitude > MIN_DIRECTION_MAGNITUDE)
            {
                return projectedForward.normalized;
            }

            Vector3 projectedRight = Vector3.ProjectOnPlane(preferredRight, up);

            if (projectedRight.sqrMagnitude > MIN_DIRECTION_MAGNITUDE)
            {
                projectedRight.Normalize();
                return Vector3.Cross(up, projectedRight).normalized;
            }

            projectedForward = Vector3.ProjectOnPlane(fallbackForward, up);

            if (projectedForward.sqrMagnitude > MIN_DIRECTION_MAGNITUDE)
            {
                return projectedForward.normalized;
            }

            projectedRight = Vector3.ProjectOnPlane(fallbackRight, up);

            if (projectedRight.sqrMagnitude > MIN_DIRECTION_MAGNITUDE)
            {
                projectedRight.Normalize();
                return Vector3.Cross(up, projectedRight).normalized;
            }

            Vector3 worldRight = Vector3.ProjectOnPlane(Vector3.right, up);

            if (worldRight.sqrMagnitude > MIN_DIRECTION_MAGNITUDE)
            {
                worldRight.Normalize();
                return Vector3.Cross(up, worldRight).normalized;
            }

            return Vector3.ProjectOnPlane(Vector3.forward, up).normalized;
        }
    }
}

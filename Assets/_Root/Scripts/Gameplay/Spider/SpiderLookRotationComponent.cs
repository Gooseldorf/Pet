using UnityEngine;

namespace Pet.Gameplay
{
    public class SpiderLookRotationComponent
    {
        private const float MIN_CAMERA_FORWARD_INFLUENCE = 0.35f;
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
            Transform forwardReference = movementReference != null ? movementReference : orientationRoot;
            Vector3 surfaceTargetUp = surfaceState.HasSurface && surfaceState.IsStableSurface
                ? surfaceState.SurfaceNormal
                : referenceUp;
            float upInterpolationFactor = 1f - Mathf.Exp(-config.OrientationSharpness * fixedDeltaTime);
            Vector3 targetUp = Vector3.Slerp(referenceUp, surfaceTargetUp, upInterpolationFactor).normalized;
            Vector3 bodyForward = ResolveCurrentPlanarForward(orientationRoot.forward, orientationRoot.right, targetUp);
            Vector3 cameraForward = ResolvePlanarForward(
                forwardReference.forward,
                forwardReference.right,
                orientationRoot.forward,
                orientationRoot.right,
                targetUp);
            float upDelta = Vector3.Angle(referenceUp, surfaceTargetUp);
            float transitionBlend = 1f - Mathf.Clamp01(upDelta / 90f);
            float cameraForwardInfluence = Mathf.Lerp(MIN_CAMERA_FORWARD_INFLUENCE, 1f, transitionBlend);
            Vector3 targetForward = Vector3.Slerp(bodyForward, cameraForward, cameraForwardInfluence).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(targetForward, targetUp);
            return Quaternion.Slerp(orientationRoot.rotation, targetRotation, upInterpolationFactor);
        }

        private Vector3 ResolveCurrentPlanarForward(Vector3 forward, Vector3 right, Vector3 up)
        {
            Vector3 projectedForward = Vector3.ProjectOnPlane(forward, up);

            if (projectedForward.sqrMagnitude > MIN_DIRECTION_MAGNITUDE)
            {
                return projectedForward.normalized;
            }

            Vector3 projectedRight = Vector3.ProjectOnPlane(right, up);

            if (projectedRight.sqrMagnitude > MIN_DIRECTION_MAGNITUDE)
            {
                projectedRight.Normalize();
                return Vector3.Cross(up, projectedRight).normalized;
            }

            return ResolvePlanarForward(Vector3.forward, Vector3.right, forward, right, up);
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

using UnityEngine;

namespace Pet.Gameplay
{
    public class SpiderOrientationComponent
    {
        private readonly SpiderConfig config;

        public SpiderOrientationComponent(SpiderConfig config)
        {
            this.config = config;
        }

        public Quaternion Evaluate(Transform orientationRoot, SpiderSurfaceState surfaceState, float fixedDeltaTime)
        {
            Quaternion currentRotation = orientationRoot.rotation;

            if (!surfaceState.HasSurface || !surfaceState.IsStableSurface)
            {
                return currentRotation;
            }

            Vector3 targetUp = surfaceState.SurfaceNormal;
            Vector3 projectedForward = Vector3.ProjectOnPlane(orientationRoot.forward, targetUp);

            if (projectedForward.sqrMagnitude < 0.0001f)
            {
                projectedForward = Vector3.ProjectOnPlane(orientationRoot.right, targetUp);
                projectedForward = Vector3.Cross(targetUp, projectedForward).normalized;
            }
            else
            {
                projectedForward.Normalize();
            }

            Quaternion targetRotation = Quaternion.LookRotation(projectedForward, targetUp);
            float interpolationFactor = 1f - Mathf.Exp(-config.OrientationSharpness * fixedDeltaTime);
            return Quaternion.Slerp(currentRotation, targetRotation, interpolationFactor);
        }
    }
}

using Pet.Input;
using UnityEngine;

namespace Pet.Gameplay
{
    public class SpiderMovementComponent
    {
        private const float MIN_DIRECTION_MAGNITUDE = 0.0001f;
        private const float MAX_SURFACE_STEP_FACTOR = 0.99f;

        private readonly SpiderConfig config;

        public SpiderMovementComponent(SpiderConfig config)
        {
            this.config = config;
        }

        public SpiderMovementResult Evaluate(SpiderPlayerController controller, SpiderSurfaceState surfaceState, float fixedDeltaTime)
        {
            Vector3 currentVelocity = controller.BodyRigidbody.linearVelocity;

            if (surfaceState.HasSurface && surfaceState.IsStableSurface)
            {
                return new SpiderMovementResult(EvaluateSurfaceVelocity(controller, surfaceState, currentVelocity, fixedDeltaTime));
            }

            return new SpiderMovementResult(EvaluateAirborneVelocity(controller, currentVelocity, fixedDeltaTime));
        }

        private Vector3 EvaluateSurfaceVelocity(
            SpiderPlayerController controller,
            SpiderSurfaceState surfaceState,
            Vector3 currentVelocity,
            float fixedDeltaTime)
        {
            Vector3 surfaceNormal = controller.CurrentReferenceUp;
            Vector3 currentPlanarVelocity = Vector3.ProjectOnPlane(currentVelocity, surfaceNormal);
            Vector3 targetPlanarVelocity = CalculateSurfacePlanarTargetVelocity(controller.CurrentInputState, controller.MovementReference, surfaceNormal);
            float maxSurfaceSpeed = CalculateMaxSurfaceSpeed(fixedDeltaTime);

            if (targetPlanarVelocity.sqrMagnitude > maxSurfaceSpeed * maxSurfaceSpeed)
            {
                targetPlanarVelocity = targetPlanarVelocity.normalized * maxSurfaceSpeed;
            }

            Vector3 nextPlanarVelocity = Vector3.MoveTowards(
                currentPlanarVelocity,
                targetPlanarVelocity,
                config.MoveAcceleration * fixedDeltaTime);

            float currentNormalSpeed = Vector3.Dot(currentVelocity, surfaceNormal);
            float targetNormalSpeed = CalculateSurfaceNormalTargetSpeed(
                surfaceState.SurfaceDistance,
                currentNormalSpeed,
                fixedDeltaTime);

            float nextNormalSpeed = Mathf.MoveTowards(
                currentNormalSpeed,
                targetNormalSpeed,
                config.AdhesionForce * fixedDeltaTime);

            return nextPlanarVelocity + surfaceNormal * nextNormalSpeed;
        }

        private Vector3 EvaluateAirborneVelocity(SpiderPlayerController controller, Vector3 currentVelocity, float fixedDeltaTime)
        {
            Vector3 localUp = controller.CurrentReferenceUp;
            Vector3 currentLateralVelocity = Vector3.ProjectOnPlane(currentVelocity, localUp);
            Vector3 gravityVelocity = -localUp * config.AirborneGravity * fixedDeltaTime;
            Vector3 nextVelocity = currentVelocity + gravityVelocity;

            if (!config.EnableAirControl)
            {
                return nextVelocity;
            }

            Vector3 targetLateralVelocity = CalculateAirborneLateralTargetVelocity(controller.CurrentInputState, controller.MovementReference, localUp);
            Vector3 nextLateralVelocity = Vector3.MoveTowards(
                currentLateralVelocity,
                targetLateralVelocity,
                config.AirMoveAcceleration * fixedDeltaTime);
            Vector3 nextVerticalVelocity = Vector3.Project(nextVelocity, localUp);

            return nextLateralVelocity + nextVerticalVelocity;
        }

        private Vector3 CalculateSurfacePlanarTargetVelocity(PlayerInputState inputState, Transform movementRoot, Vector3 surfaceNormal)
        {
            Vector3 moveDirection = CalculateMoveDirection(inputState, movementRoot, surfaceNormal);
            return moveDirection * config.MaxMoveSpeed;
        }

        private Vector3 CalculateAirborneLateralTargetVelocity(PlayerInputState inputState, Transform movementRoot, Vector3 localUp)
        {
            Vector3 moveDirection = CalculateMoveDirection(inputState, movementRoot, localUp);
            return moveDirection * config.MaxAirMoveSpeed;
        }

        private float CalculateSurfaceNormalTargetSpeed(float surfaceDistance, float currentNormalSpeed, float fixedDeltaTime)
        {
            if (!config.EnableSurfaceAdhesion)
            {
                return Mathf.Min(currentNormalSpeed, 0f);
            }

            float deadZone = Mathf.Max(config.AdhesionDeadZone, 0.001f);
            float hoverOffset = Mathf.Max(config.SurfaceHoverOffset, 0f);
            float distanceToTarget = surfaceDistance - hoverOffset;

            if (distanceToTarget <= deadZone)
            {
                return Mathf.Min(currentNormalSpeed, 0f);
            }

            float pullDistance = distanceToTarget - deadZone;
            float maxPullSpeed = Mathf.Max(config.SurfaceStickSpeed, 0f);
            float desiredPullSpeed = Mathf.Min(maxPullSpeed, pullDistance / Mathf.Max(fixedDeltaTime, 0.0001f));
            return -desiredPullSpeed;
        }

        private float CalculateMaxSurfaceSpeed(float fixedDeltaTime)
        {
            float maxStepDistance = config.DownProbeRadius * MAX_SURFACE_STEP_FACTOR;
            float maxStepSpeed = maxStepDistance / Mathf.Max(fixedDeltaTime, 0.0001f);
            return Mathf.Min(config.MaxMoveSpeed, maxStepSpeed);
        }

        private Vector3 CalculateMoveDirection(PlayerInputState inputState, Transform movementRoot, Vector3 planeNormal)
        {
            Vector2 moveInput = inputState.Move;

            if (moveInput.sqrMagnitude <= MIN_DIRECTION_MAGNITUDE)
            {
                return Vector3.zero;
            }

            Vector3 forward = Vector3.ProjectOnPlane(movementRoot.forward, planeNormal);

            if (forward.sqrMagnitude <= MIN_DIRECTION_MAGNITUDE)
            {
                Vector3 projectedRight = Vector3.ProjectOnPlane(movementRoot.right, planeNormal);

                if (projectedRight.sqrMagnitude <= MIN_DIRECTION_MAGNITUDE)
                {
                    return Vector3.zero;
                }

                projectedRight.Normalize();
                forward = Vector3.Cross(projectedRight, planeNormal);
            }

            forward.Normalize();

            Vector3 right = Vector3.Cross(planeNormal, forward);

            if (right.sqrMagnitude <= MIN_DIRECTION_MAGNITUDE)
            {
                return Vector3.zero;
            }

            right.Normalize();

            Vector3 moveDirection = (forward * moveInput.y) + (right * moveInput.x);

            if (moveDirection.sqrMagnitude <= MIN_DIRECTION_MAGNITUDE)
            {
                return Vector3.zero;
            }

            return moveDirection.normalized;
        }
    }
}

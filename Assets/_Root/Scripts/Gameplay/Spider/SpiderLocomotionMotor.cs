using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Pet.Gameplay
{
    internal sealed class SpiderLocomotionMotor
    {
        private const float MIN_VECTOR_SQR_MAGNITUDE = 0.000001f;

        private readonly Rigidbody bodyRigidbody;
        private readonly SphereCollider bodyCollider;
        private readonly SpiderConfig config;
        private Vector3 airborneControlVelocity;
        private bool wasMovementBlocked;

        public SpiderLocomotionMotor(Rigidbody bodyRigidbody, SphereCollider bodyCollider, SpiderConfig config)
        {
            this.bodyRigidbody = bodyRigidbody;
            this.bodyCollider = bodyCollider;
            this.config = config;
        }

        public void BeginAttachment(SpiderSurfaceState surface)
        {
            airborneControlVelocity = Vector3.zero;
            Vector3 velocity = bodyRigidbody.linearVelocity;
            float normalVelocity = Vector3.Dot(velocity, surface.Normal);

            if (normalVelocity < 0f)
            {
                bodyRigidbody.linearVelocity = velocity - surface.Normal * normalVelocity;
            }
        }

        public void BeginJump(SpiderSurfaceState surface)
        {
            airborneControlVelocity = Vector3.zero;
            Vector3 tangentialVelocity = Vector3.ProjectOnPlane(bodyRigidbody.linearVelocity, surface.Normal);
            bodyRigidbody.linearVelocity = tangentialVelocity + surface.Normal * config.JumpSpeed;
        }

        public void Tick(SpiderSurfaceDetector surfaceDetector, Vector2 moveInput, Transform cameraTransform, float deltaTime)
        {
            SpiderSurfaceState surface = surfaceDetector.State;

            if (surface.IsAttached)
            {
                TickAttached(surfaceDetector, surface, moveInput, cameraTransform, deltaTime);
                return;
            }

            TickAirborne(moveInput, cameraTransform, deltaTime);
        }

        private void TickAttached(
            SpiderSurfaceDetector surfaceDetector,
            SpiderSurfaceState surface,
            Vector2 moveInput,
            Transform cameraTransform,
            float deltaTime)
        {
            if (!surfaceDetector.HasDetectedSurface)
            {
                if (!wasMovementBlocked)
                {
                    LogTraversal(
                        $"Stopped at edge. position={bodyRigidbody.position}, velocity={bodyRigidbody.linearVelocity}, " +
                        $"normal={surface.Normal}, input={moveInput}");
                    wasMovementBlocked = true;
                }

                StopAtEdge(surfaceDetector.LastAttachedBodyPosition, surfaceDetector.LastAttachedBodyRotation);
                return;
            }

            Vector3 heading = CalculateSurfaceHeading(surface.Normal, cameraTransform);
            Vector3 moveDirection = CalculateMoveDirection(moveInput, heading, surface.Normal);
            Quaternion plannedRotation = CalculateAttachedRotation(
                heading,
                surface.Normal,
                moveInput.sqrMagnitude > config.MovingHeadingInputThreshold * config.MovingHeadingInputThreshold,
                deltaTime);
            UpdateAttachedVelocity(surfaceDetector, surface, moveDirection, plannedRotation, deltaTime);
            ApplyRotation(plannedRotation);
        }

        private void TickAirborne(Vector2 moveInput, Transform cameraTransform, float deltaTime)
        {
            bodyRigidbody.AddForce(Vector3.down * config.AirborneGravity, ForceMode.Acceleration);
            UpdateAirborneControl(moveInput, cameraTransform, deltaTime);
            UpdateAirborneRotation(cameraTransform, deltaTime);
        }

        private Vector3 CalculateSurfaceHeading(Vector3 surfaceNormal, Transform cameraTransform)
        {
            Vector3 heading = Vector3.ProjectOnPlane(cameraTransform.forward, surfaceNormal);

            if (heading.sqrMagnitude <= MIN_VECTOR_SQR_MAGNITUDE)
            {
                heading = Vector3.ProjectOnPlane(bodyRigidbody.transform.forward, surfaceNormal);
            }

            if (heading.sqrMagnitude <= MIN_VECTOR_SQR_MAGNITUDE)
            {
                heading = Vector3.Cross(surfaceNormal, bodyRigidbody.transform.right);
            }

            return heading.normalized;
        }

        private Vector3 CalculateMoveDirection(Vector2 moveInput, Vector3 heading, Vector3 surfaceNormal)
        {
            Vector3 right = Vector3.Cross(surfaceNormal, heading).normalized;
            Vector3 moveDirection = heading * moveInput.y + right * moveInput.x;
            return Vector3.ClampMagnitude(moveDirection, 1f);
        }

        private void UpdateAttachedVelocity(
            SpiderSurfaceDetector surfaceDetector,
            SpiderSurfaceState surface,
            Vector3 moveDirection,
            Quaternion plannedRotation,
            float deltaTime)
        {
            Vector3 velocity = bodyRigidbody.linearVelocity;
            Vector3 tangentialVelocity = Vector3.ProjectOnPlane(velocity, surface.Normal);
            Vector3 targetVelocity = moveDirection * config.MaxMoveSpeed;
            Vector3 adjustedTangentialVelocity = Vector3.MoveTowards(
                tangentialVelocity,
                targetVelocity,
                config.MoveAcceleration * deltaTime);
            float adhesionNormalVelocity = CalculateAdhesionNormalVelocity(surface, velocity, deltaTime);
            Vector3 candidateVelocity = adjustedTangentialVelocity + surface.Normal * adhesionNormalVelocity;
            Vector3 predictedBodyCenter = CalculatePredictedBodyCenter(
                surfaceDetector,
                candidateVelocity,
                plannedRotation,
                deltaTime);
            SpiderSurfaceContact predictedSupport = default;
            bool hasPredictedSupport = adjustedTangentialVelocity.sqrMagnitude > MIN_VECTOR_SQR_MAGNITUDE &&
                                       surfaceDetector.TryFindPredictedSupport(
                                           surface,
                                           predictedBodyCenter,
                                           out predictedSupport);

            if (adjustedTangentialVelocity.sqrMagnitude > MIN_VECTOR_SQR_MAGNITUDE && !hasPredictedSupport)
            {
                if (!wasMovementBlocked)
                {
                    LogTraversal(
                        $"Blocked movement without reachable support. position={bodyRigidbody.position}, " +
                        $"targetVelocity={adjustedTangentialVelocity}, normal={surface.Normal}");
                    wasMovementBlocked = true;
                }

                adjustedTangentialVelocity = Vector3.zero;
            }
            else if (hasPredictedSupport)
            {
                wasMovementBlocked = false;
                Vector3 pathNormal = Vector3.Slerp(surface.Normal, predictedSupport.Normal, 0.5f).normalized;
                adjustedTangentialVelocity = Quaternion.FromToRotation(surface.Normal, pathNormal) * adjustedTangentialVelocity;
            }
            else
            {
                wasMovementBlocked = false;
            }

            bodyRigidbody.linearVelocity = adjustedTangentialVelocity + surface.Normal * adhesionNormalVelocity;
        }

        private Vector3 CalculatePredictedBodyCenter(
            SpiderSurfaceDetector surfaceDetector,
            Vector3 candidateVelocity,
            Quaternion plannedRotation,
            float deltaTime)
        {
            Vector3 currentBodyCenter = surfaceDetector.BodyCenter;
            Vector3 currentCenterOffset = currentBodyCenter - bodyRigidbody.position;
            Quaternion rotationDelta = plannedRotation * Quaternion.Inverse(bodyRigidbody.rotation);
            Vector3 rotatedCenterOffset = rotationDelta * currentCenterOffset;

            return currentBodyCenter +
                   candidateVelocity * deltaTime +
                   rotatedCenterOffset -
                   currentCenterOffset;
        }

        private void StopAtEdge(Vector3 lastAttachedBodyPosition, Quaternion lastAttachedBodyRotation)
        {
            bodyRigidbody.linearVelocity = Vector3.zero;
            bodyRigidbody.angularVelocity = Vector3.zero;
            bodyRigidbody.MovePosition(lastAttachedBodyPosition);
            bodyRigidbody.MoveRotation(lastAttachedBodyRotation);
        }

        private void UpdateAirborneControl(Vector2 moveInput, Transform cameraTransform, float deltaTime)
        {
            Vector3 moveDirection = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
            Vector3 targetControlVelocity = moveDirection * config.MaxMoveSpeed;
            Vector3 previousControlVelocity = airborneControlVelocity;
            airborneControlVelocity = Vector3.MoveTowards(
                airborneControlVelocity,
                targetControlVelocity,
                config.MoveAcceleration * config.AirControlCoefficient * deltaTime);
            bodyRigidbody.linearVelocity += airborneControlVelocity - previousControlVelocity;
        }

        private void UpdateAirborneRotation(Transform cameraTransform, float deltaTime)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);
            float interpolation = 1f - Mathf.Exp(-config.HeadingAlignmentSharpness * deltaTime);
            bodyRigidbody.angularVelocity = Vector3.zero;
            bodyRigidbody.MoveRotation(Quaternion.Slerp(bodyRigidbody.rotation, targetRotation, interpolation));
        }

        private float CalculateAdhesionNormalVelocity(
            SpiderSurfaceState surface,
            Vector3 velocity,
            float deltaTime)
        {
            Vector3 bodyCenter = bodyCollider.transform.TransformPoint(bodyCollider.center);
            float bodyRadius = SpiderColliderMetrics.CalculateWorldRadius(bodyCollider);
            float currentOffset = Vector3.Dot(bodyCenter - surface.Point, surface.Normal) - bodyRadius;
            float offsetError = config.SurfaceHoverOffset - currentOffset;
            float targetNormalVelocity = Mathf.Abs(offsetError) <= config.AdhesionDeadZone
                ? 0f
                : offsetError * config.SurfaceStickSpeed;
            float currentNormalVelocity = Vector3.Dot(velocity, surface.Normal);
            return Mathf.MoveTowards(
                currentNormalVelocity,
                targetNormalVelocity,
                config.AdhesionForce * deltaTime);
        }

        private Quaternion CalculateAttachedRotation(
            Vector3 heading,
            Vector3 surfaceNormal,
            bool shouldAlignHeading,
            float deltaTime)
        {
            Quaternion surfaceRotation = Quaternion.FromToRotation(
                bodyRigidbody.transform.up,
                surfaceNormal) * bodyRigidbody.rotation;
            float surfaceInterpolation = 1f - Mathf.Exp(-config.SurfaceAlignmentSharpness * deltaTime);
            Quaternion surfaceAlignedRotation = Quaternion.Slerp(
                bodyRigidbody.rotation,
                surfaceRotation,
                surfaceInterpolation);

            if (!shouldAlignHeading)
            {
                return surfaceAlignedRotation;
            }

            Quaternion targetRotation = Quaternion.LookRotation(heading, surfaceNormal);
            float headingInterpolation = 1f - Mathf.Exp(-config.MovingHeadingAlignmentSharpness * deltaTime);
            return Quaternion.Slerp(
                surfaceAlignedRotation,
                targetRotation,
                headingInterpolation);
        }

        private void ApplyRotation(Quaternion rotation)
        {
            bodyRigidbody.angularVelocity = Vector3.zero;
            bodyRigidbody.MoveRotation(rotation);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        private void LogTraversal(string message)
        {
            Debug.Log($"[SpiderTraversal] {message}", bodyCollider);
        }
    }
}

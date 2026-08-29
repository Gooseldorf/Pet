using UnityEngine;

namespace Pet.Gameplay
{
    public sealed class SpiderLocomotionMotor
    {
        private const float MIN_VECTOR_SQR_MAGNITUDE = 0.000001f;

        private readonly Rigidbody bodyRigidbody;
        private readonly SphereCollider bodyCollider;
        private readonly SpiderConfig config;

        public SpiderLocomotionMotor(Rigidbody bodyRigidbody, SphereCollider bodyCollider, SpiderConfig config)
        {
            this.bodyRigidbody = bodyRigidbody;
            this.bodyCollider = bodyCollider;
            this.config = config;
        }

        public void BeginAttachment(SpiderSurfaceState surface)
        {
            Vector3 velocity = bodyRigidbody.linearVelocity;
            float normalVelocity = Vector3.Dot(velocity, surface.Normal);

            if (normalVelocity < 0f)
            {
                bodyRigidbody.linearVelocity = velocity - surface.Normal * normalVelocity;
            }
        }

        public void Tick(SpiderSurfaceState surface, Vector2 moveInput, Transform cameraTransform, float deltaTime)
        {
            if (surface.IsAttached)
            {
                TickAttached(surface, moveInput, cameraTransform, deltaTime);
                return;
            }

            TickAirborne();
        }

        private void TickAttached(
            SpiderSurfaceState surface,
            Vector2 moveInput,
            Transform cameraTransform,
            float deltaTime)
        {
            Vector3 heading = CalculateSurfaceHeading(surface.Normal, cameraTransform);
            Vector3 moveDirection = CalculateMoveDirection(moveInput, heading, surface.Normal);

            UpdateTangentialVelocity(moveDirection, surface.Normal, deltaTime);
            UpdateAdhesion(surface, deltaTime);
            UpdateRotation(heading, surface.Normal, deltaTime);
        }

        private void TickAirborne()
        {
            bodyRigidbody.AddForce(Vector3.down * config.AirborneGravity, ForceMode.Acceleration);
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

        private void UpdateTangentialVelocity(Vector3 moveDirection, Vector3 surfaceNormal, float deltaTime)
        {
            Vector3 velocity = bodyRigidbody.linearVelocity;
            Vector3 tangentialVelocity = Vector3.ProjectOnPlane(velocity, surfaceNormal);
            Vector3 targetVelocity = moveDirection * config.MaxMoveSpeed;
            Vector3 adjustedTangentialVelocity = Vector3.MoveTowards(
                tangentialVelocity,
                targetVelocity,
                config.MoveAcceleration * deltaTime);
            float normalVelocity = Vector3.Dot(velocity, surfaceNormal);

            bodyRigidbody.linearVelocity = adjustedTangentialVelocity + surfaceNormal * normalVelocity;
        }

        private void UpdateAdhesion(SpiderSurfaceState surface, float deltaTime)
        {
            Vector3 bodyCenter = bodyCollider.transform.TransformPoint(bodyCollider.center);
            float bodyRadius = CalculateWorldRadius();
            float currentOffset = Vector3.Dot(bodyCenter - surface.Point, surface.Normal) - bodyRadius;
            float offsetError = config.SurfaceHoverOffset - currentOffset;
            float targetNormalVelocity = Mathf.Abs(offsetError) <= config.AdhesionDeadZone
                ? 0f
                : offsetError * config.SurfaceStickSpeed;
            float currentNormalVelocity = Vector3.Dot(bodyRigidbody.linearVelocity, surface.Normal);
            float adjustedNormalVelocity = Mathf.MoveTowards(
                currentNormalVelocity,
                targetNormalVelocity,
                config.AdhesionForce * deltaTime);

            bodyRigidbody.linearVelocity += surface.Normal * (adjustedNormalVelocity - currentNormalVelocity);
        }

        private void UpdateRotation(Vector3 heading, Vector3 surfaceNormal, float deltaTime)
        {
            Quaternion targetRotation = Quaternion.LookRotation(heading, surfaceNormal);
            Quaternion surfaceRotation = Quaternion.FromToRotation(
                bodyRigidbody.transform.up,
                surfaceNormal) * bodyRigidbody.rotation;
            float surfaceInterpolation = 1f - Mathf.Exp(-config.SurfaceAlignmentSharpness * deltaTime);
            Quaternion surfaceAlignedRotation = Quaternion.Slerp(
                bodyRigidbody.rotation,
                surfaceRotation,
                surfaceInterpolation);
            float headingInterpolation = 1f - Mathf.Exp(-config.HeadingAlignmentSharpness * deltaTime);
            Quaternion alignedRotation = Quaternion.Slerp(
                surfaceAlignedRotation,
                targetRotation,
                headingInterpolation);
            bodyRigidbody.angularVelocity = Vector3.zero;
            bodyRigidbody.MoveRotation(alignedRotation);
        }

        private float CalculateWorldRadius()
        {
            Vector3 scale = bodyCollider.transform.lossyScale;
            float largestScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            return bodyCollider.radius * largestScale;
        }
    }
}

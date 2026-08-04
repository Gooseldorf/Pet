using System;
using Pet.Input;
using R3;
using UnityEngine;
using VContainer;

namespace Pet.Gameplay
{
    public class SpiderPlayerController : MonoBehaviour
    {
        private const float DEFAULT_GIZMO_PROBE_DISTANCE = 1f;
        private const float DEFAULT_GIZMO_PROBE_RADIUS = 0.25f;
        private const float SURFACE_TRANSITION_ANGLE_EPSILON = 5f;

        [Header("Required References")]
        [SerializeField] private Rigidbody bodyRigidbody;
        [SerializeField] private Collider bodyCollider;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Transform probeOrigin;
        [SerializeField] private Transform cameraFollowTarget;
        [SerializeField] private Transform cameraLookTarget;

        private SpiderConfig config;
        private IPlayerInputStreams inputStreams;
        private IDisposable jumpPressedSubscription;
        private PlayerInputState currentInputState;
        private SpiderSurfaceComponent surfaceComponent;
        private SpiderLookRotationComponent lookRotationComponent;
        private SpiderMovementComponent movementComponent;
        private SpiderSurfaceState currentSurfaceState;
        private SpiderMovementResult currentMovementResult;
        private Quaternion targetBodyRotation;
        private Vector3 currentReferenceUp;
        private Transform movementReference;
        private bool jumpQueued;
        private bool isInitialized;
        private bool isSurfaceTransitionActive;

        public SpiderConfig Config => config;
        public Rigidbody BodyRigidbody => bodyRigidbody;
        public Collider BodyCollider => bodyCollider;
        public Transform VisualRoot => visualRoot;
        public Transform ProbeOrigin => probeOrigin;
        public Transform CameraFollowTarget => cameraFollowTarget;
        public Transform CameraLookTarget => cameraLookTarget;
        public PlayerInputState CurrentInputState => currentInputState;
        public SpiderSurfaceState CurrentSurfaceState => currentSurfaceState;
        public Vector3 CurrentReferenceUp => currentReferenceUp;
        public Transform MovementReference => movementReference;
        public bool IsSurfaceTransitionActive => isSurfaceTransitionActive;

        [Inject]
        public void Construct(SpiderConfig config, IPlayerInputStreams inputStreams)
        {
            this.inputStreams = inputStreams;
            this.config = config;
        }

        public void Initialize()
        {
            surfaceComponent = new SpiderSurfaceComponent(config);
            lookRotationComponent = new SpiderLookRotationComponent(config);
            movementComponent = new SpiderMovementComponent(config);
            jumpPressedSubscription = inputStreams.JumpPressed.Subscribe(_ =>
            {
                if (!isActiveAndEnabled)
                {
                    return;
                }

                jumpQueued = true;
            });

            currentInputState = inputStreams.CurrentState;
            targetBodyRotation = transform.rotation;
            currentMovementResult = new SpiderMovementResult(bodyRigidbody.linearVelocity);
            currentReferenceUp = transform.up;
            isInitialized = true;
        }

        public void SetMovementReference(Transform movementReference)
        {
            this.movementReference = movementReference;
        }

        private void OnDestroy()
        {
            jumpPressedSubscription?.Dispose();
            jumpPressedSubscription = null;
            jumpQueued = false;
            isInitialized = false;
        }

        private void FixedUpdate()
        {
            if (!isInitialized)
            {
                return;
            }

            CaptureInputState();
            RunControllerStep(Time.fixedDeltaTime);
            ApplyRootState();
        }

        private void CaptureInputState()
        {
            currentInputState = inputStreams.CurrentState;
        }

        private void RunControllerStep(float fixedDeltaTime)
        {
            currentSurfaceState = surfaceComponent.Sample(this);

            if (currentSurfaceState.HasSurface && currentSurfaceState.IsStableSurface)
            {
                UpdateReferenceUp(currentSurfaceState.SurfaceNormal, fixedDeltaTime);
            }
            else
            {
                isSurfaceTransitionActive = false;
            }

            targetBodyRotation = lookRotationComponent.Evaluate(
                transform,
                currentSurfaceState,
                movementReference,
                currentReferenceUp,
                fixedDeltaTime);
            currentMovementResult = movementComponent.Evaluate(this, currentSurfaceState, fixedDeltaTime);
        }

        private void UpdateReferenceUp(Vector3 targetSurfaceNormal, float fixedDeltaTime)
        {
            if (targetSurfaceNormal.sqrMagnitude <= Mathf.Epsilon)
            {
                isSurfaceTransitionActive = false;
                return;
            }

            Vector3 normalizedTargetUp = targetSurfaceNormal.normalized;
            float targetAngle = Vector3.Angle(currentReferenceUp, normalizedTargetUp);
            float interpolationFactor = 1f - Mathf.Exp(-config.SurfaceAlignmentSharpness * fixedDeltaTime);
            currentReferenceUp = Vector3.Slerp(currentReferenceUp, normalizedTargetUp, interpolationFactor).normalized;
            isSurfaceTransitionActive = targetAngle > SURFACE_TRANSITION_ANGLE_EPSILON;
        }

        private void ApplyRootState()
        {
            bodyRigidbody.linearVelocity = currentMovementResult.LinearVelocity;
            bodyRigidbody.angularVelocity = Vector3.zero;
            bodyRigidbody.MoveRotation(targetBodyRotation);
        }

        public bool ConsumeJumpRequest()
        {
            bool wasQueued = jumpQueued;
            jumpQueued = false;
            return wasQueued;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 probeStart = probeOrigin != null ? probeOrigin.position : transform.position;
            Vector3 referenceUp = currentReferenceUp.sqrMagnitude > Mathf.Epsilon ? currentReferenceUp.normalized : transform.up;
            Vector3 downDirection = -referenceUp;
            Vector3 forwardDirection = transform.forward;
            float downDistance = config != null ? config.DownProbeDistance : DEFAULT_GIZMO_PROBE_DISTANCE;
            float downRadius = config != null ? config.DownProbeRadius : DEFAULT_GIZMO_PROBE_RADIUS;
            float forwardDistance = config != null ? config.ForwardProbeDistance : DEFAULT_GIZMO_PROBE_DISTANCE;
            float forwardRadius = config != null ? config.ForwardProbeRadius : DEFAULT_GIZMO_PROBE_RADIUS;
            float overlapRadius = Mathf.Max(downRadius, forwardRadius);

            Gizmos.color = Color.green;
            DrawSphereCastGizmo(probeStart, downDirection, downDistance, downRadius);

            Gizmos.color = Color.yellow;
            DrawSphereCastGizmo(probeStart, forwardDirection, forwardDistance, forwardRadius);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(probeStart, overlapRadius);

            if (!currentSurfaceState.HasSurface || !currentSurfaceState.IsStableSurface)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(currentSurfaceState.SurfacePoint, 0.04f);
            Gizmos.DrawLine(
                currentSurfaceState.SurfacePoint,
                currentSurfaceState.SurfacePoint + currentSurfaceState.SurfaceNormal.normalized * 0.5f);

            Gizmos.color = isSurfaceTransitionActive ? new Color(1f, 0.4f, 0f) : Color.cyan;
            Gizmos.DrawLine(probeStart, probeStart + currentReferenceUp.normalized * 0.6f);
        }

        private void DrawSphereCastGizmo(Vector3 origin, Vector3 direction, float distance, float radius)
        {
            if (direction.sqrMagnitude <= Mathf.Epsilon || distance <= 0f || radius <= 0f)
            {
                return;
            }

            Vector3 normalizedDirection = direction.normalized;
            Vector3 end = origin + normalizedDirection * distance;

            Gizmos.DrawWireSphere(origin, radius);
            Gizmos.DrawLine(origin, end);
            Gizmos.DrawWireSphere(end, radius);
        }
    }
}

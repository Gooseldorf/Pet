using System;
using Pet.Input;
using R3;
using UnityEngine;
using VContainer;

namespace Pet.Gameplay
{
    public class SpiderPlayerController : MonoBehaviour
    {
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
                currentReferenceUp = currentSurfaceState.SurfaceNormal;
            }

            targetBodyRotation = lookRotationComponent.Evaluate(
                transform,
                currentSurfaceState,
                movementReference,
                currentReferenceUp,
                fixedDeltaTime);
            currentMovementResult = movementComponent.Evaluate(this, currentSurfaceState, fixedDeltaTime);
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
    }
}

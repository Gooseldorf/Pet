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

        private SpiderConfig config;
        private IPlayerInputStreams inputStreams;
        private IDisposable jumpPressedSubscription;
        private PlayerInputState currentInputState;
        private SpiderSurfaceComponent surfaceComponent;
        private SpiderOrientationComponent orientationComponent;
        private SpiderMovementComponent movementComponent;
        private SpiderSurfaceState currentSurfaceState;
        private SpiderMovementResult currentMovementResult;
        private Quaternion targetBodyRotation;
        private Vector3 currentReferenceUp;
        private bool jumpQueued;
        private bool isInitialized;

        public SpiderConfig Config => config;
        public Rigidbody BodyRigidbody => bodyRigidbody;
        public Collider BodyCollider => bodyCollider;
        public Transform VisualRoot => visualRoot;
        public Transform ProbeOrigin => probeOrigin;
        public PlayerInputState CurrentInputState => currentInputState;
        public SpiderSurfaceState CurrentSurfaceState => currentSurfaceState;
        public Vector3 CurrentReferenceUp => currentReferenceUp;

        [Inject]
        public void Construct(SpiderConfig config, IPlayerInputStreams inputStreams)
        {
            this.inputStreams = inputStreams;
            this.config = config;
        }

        private void Awake()
        {
            bodyRigidbody.useGravity = false;
        }

        public void Initialize()
        {
            surfaceComponent = new SpiderSurfaceComponent(config);
            orientationComponent = new SpiderOrientationComponent(config);
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

            targetBodyRotation = orientationComponent.Evaluate(transform, currentSurfaceState, fixedDeltaTime);
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

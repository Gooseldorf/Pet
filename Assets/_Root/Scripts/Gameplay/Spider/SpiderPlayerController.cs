using System;
using Pet.Input;
using R3;
using UnityEngine;

namespace Pet.Gameplay
{
    public class SpiderPlayerController : MonoBehaviour
    {
        [Header("Required References")]
        [SerializeField] private Rigidbody bodyRigidbody;
        [SerializeField] private SphereCollider bodyCollider;

        [Header("Camera Targets")]
        [SerializeField] private Transform cameraFollowTarget;
        [SerializeField] private Transform cameraLookTarget;

        private SpiderSurfaceDetector surfaceDetector;
        private SpiderLocomotionMotor locomotionMotor;
        private IPlayerInputStreams inputStreams;
        private Transform movementCameraTransform;
        private IDisposable jumpInputSubscription;
        private bool jumpRequested;

        public Transform CameraFollowTarget => cameraFollowTarget;
        public Transform CameraLookTarget => cameraLookTarget;
        public SpiderSurfaceState SurfaceState => surfaceDetector.State;
        internal SpiderSurfaceDetector SurfaceDetector => surfaceDetector;

        public void Initialize(SpiderConfig config, IPlayerInputStreams inputStreams, Camera movementCamera)
        {
            this.inputStreams = inputStreams;
            movementCameraTransform = movementCamera.transform;
            surfaceDetector = new SpiderSurfaceDetector(bodyCollider, config);
            locomotionMotor = new SpiderLocomotionMotor(bodyRigidbody, bodyCollider, config);
            jumpInputSubscription = inputStreams.JumpPressed.Subscribe(_ => jumpRequested = true);
        }

        private void FixedUpdate()
        {
            bool wasAttached = surfaceDetector.State.IsAttached;
            surfaceDetector.Sample();
            bool shouldJump = jumpRequested;
            jumpRequested = false;

            if (shouldJump && surfaceDetector.State.IsAttached && surfaceDetector.HasDetectedSurface)
            {
                locomotionMotor.BeginJump(surfaceDetector.State);
                surfaceDetector.BeginJump();
            }

            if (!wasAttached && surfaceDetector.State.IsAttached)
            {
                locomotionMotor.BeginAttachment(surfaceDetector.State);
            }

            locomotionMotor.Tick(
                surfaceDetector,
                inputStreams.CurrentState.Move,
                movementCameraTransform,
                Time.fixedDeltaTime);
        }

        private void OnDestroy()
        {
            jumpInputSubscription?.Dispose();
        }
    }
}

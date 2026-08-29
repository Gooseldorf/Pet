using Pet.Input;
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
        }

        private void FixedUpdate()
        {
            bool wasAttached = surfaceDetector.State.IsAttached;
            surfaceDetector.Sample();

            if (!wasAttached && surfaceDetector.State.IsAttached)
            {
                locomotionMotor.BeginAttachment(surfaceDetector.State);
            }

            locomotionMotor.Tick(
                surfaceDetector.State,
                inputStreams.CurrentState.Move,
                movementCameraTransform,
                Time.fixedDeltaTime);
        }
    }
}

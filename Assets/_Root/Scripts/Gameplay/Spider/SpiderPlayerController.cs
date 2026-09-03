using Pet.Input;
using UnityEngine;

namespace Pet.Gameplay
{
    public sealed class SpiderPlayerController : MonoBehaviour
    {
        [Header("Required References")]
        [SerializeField] private Rigidbody bodyRigidbody;

        [Header("Camera Targets")]
        [SerializeField] private Transform cameraFollowTarget;
        [SerializeField] private Transform cameraLookTarget;

        private SpiderConfig config;
        private IPlayerInputStreams inputStreams;
        private Camera camera;

        public Transform CameraFollowTarget => cameraFollowTarget;
        public Transform CameraLookTarget => cameraLookTarget;

        public void Initialize(SpiderConfig config, IPlayerInputStreams inputStreams, Camera camera)
        {
            this.config = config;
            this.inputStreams = inputStreams;
            this.camera = camera;
        }

        private void FixedUpdate()
        {
            Vector2 moveInput = inputStreams.CurrentState.Move;
            Transform cameraTransform = camera.transform;
            Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            Vector3 moveDirection = Vector3.ClampMagnitude(forward * moveInput.y + right * moveInput.x, 1f);
            Vector3 velocity = bodyRigidbody.linearVelocity;
            bodyRigidbody.linearVelocity = new Vector3(moveDirection.x * config.MaxMoveSpeed, velocity.y, moveDirection.z * config.MaxMoveSpeed);
        }
    }
}

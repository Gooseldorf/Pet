using UnityEngine;

namespace Pet.Gameplay
{
    public class SpiderPlayerController : MonoBehaviour
    {
        [Header("Camera Targets")]
        [SerializeField] private Transform cameraFollowTarget;
        [SerializeField] private Transform cameraLookTarget;

        public Transform CameraFollowTarget => cameraFollowTarget;
        public Transform CameraLookTarget => cameraLookTarget;
    }
}

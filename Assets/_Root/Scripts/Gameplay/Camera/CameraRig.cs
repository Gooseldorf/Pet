using Unity.Cinemachine;
using UnityEngine;

namespace Pet.Gameplay
{
    public class CameraRig : MonoBehaviour
    {
        [Header("Required References")]
        [SerializeField] private CinemachineCamera cinemachineCamera;

        public void Bind(SpiderPlayerController player)
        {
            cinemachineCamera.Follow = player.CameraFollowTarget;
            cinemachineCamera.LookAt = player.CameraLookTarget;
        }
    }
}

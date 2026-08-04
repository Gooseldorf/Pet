using Unity.Cinemachine;
using UnityEngine;

namespace Pet.Gameplay
{
    public class CameraRig : MonoBehaviour
    {
        [Header("Required References")]
        [SerializeField] private CinemachineCamera cinemachineCamera;

        public void Bind(SpiderPlayerController player, CinemachineBrain brain)
        {
            cinemachineCamera.Follow = player.CameraFollowTarget;
            cinemachineCamera.LookAt = player.CameraLookTarget;
            brain.WorldUpOverride = player.transform;
        }
    }
}

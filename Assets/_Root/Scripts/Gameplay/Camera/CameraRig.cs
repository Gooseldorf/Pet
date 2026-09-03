using Unity.Cinemachine;
using UnityEngine;

namespace Pet.Gameplay
{
    public class CameraRig : MonoBehaviour
    {
        [Header("Required References")]
        [SerializeField] private CinemachineCamera cinemachineCamera;

        // Привязывает Cinemachine к авторским целям созданного паука, сохраняя камеру источником направления движения.
        public void Bind(SpiderPlayerController player)
        {
            cinemachineCamera.Follow = player.CameraFollowTarget;
            cinemachineCamera.LookAt = player.CameraLookTarget;
        }
    }
}

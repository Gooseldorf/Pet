using UnityEngine;

namespace Pet.Gameplay
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Configs/Gameplay/Camera/CameraConfig")]
    public class CameraConfig : ScriptableObject
    {
        [field: SerializeField] public CameraRig Prefab { get; private set; }
    }
}

using UnityEngine;

namespace Pet.Gameplay
{
    [CreateAssetMenu(fileName = "SpiderConfig", menuName = "Configs/Gameplay/Spider/SpiderConfig")]
    public class SpiderConfig : ScriptableObject
    {
        [Header("Prefab")]
        [field: SerializeField] public SpiderPlayerController Prefab { get; private set; }
    }
}

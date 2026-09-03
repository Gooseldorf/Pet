using UnityEngine;

namespace Pet.Gameplay
{
    [CreateAssetMenu(fileName = "SpiderConfig", menuName = "Configs/Gameplay/Spider/SpiderConfig")]
    public sealed class SpiderConfig : ScriptableObject
    {
        [Header("Prefab")]
        [field: SerializeField] public SpiderPlayerController Prefab { get; private set; }

        [Header("Movement")]
        [SerializeField, Min(0f)] private float maxMoveSpeed = 5f;

        public float MaxMoveSpeed => maxMoveSpeed;
    }
}

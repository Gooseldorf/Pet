using UnityEngine;

namespace Pet.Configs
{
    [CreateAssetMenu(fileName = "LoadingOverlayConfig", menuName = "Configs/UI/Loading Overlay Config")]
    public class LoadingOverlayConfig : ScriptableObject
    {
        [field: SerializeField] public float FadeDuration { get; private set; } = 0.2f;
    }
}

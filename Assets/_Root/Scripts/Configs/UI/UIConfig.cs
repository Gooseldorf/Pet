using UnityEngine;

namespace Pet.Configs
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Configs/UI/UI Config")]
    public class UIConfig : ScriptableObject
    {
        [field: SerializeField] public LoadingOverlayConfig LoadingOverlay { get; private set; }
    }
}

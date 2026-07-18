using Pet.Gameplay;
using UnityEngine;

namespace Pet.UI
{
    [CreateAssetMenu(fileName = "HUDConfig", menuName = "Configs/UI/HUD Config")]
    public class UIHudConfig : ScriptableObject
    {
        [field: SerializeField] public UIHudView Prefab { get; private set; }
        [field: SerializeField] public UICacheModeEnum CacheModeEnum { get; private set; } = UICacheModeEnum.Persistent;
    }
}

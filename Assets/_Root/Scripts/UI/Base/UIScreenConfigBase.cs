using UnityEngine;

namespace Pet.UI
{
    public abstract class UIScreenConfigBase : ScriptableObject
    {
        [field: SerializeField] public UIScreenViewBase Prefab { get; private set; }
        [field: SerializeField] public UIHistoryModeEnum HistoryModeEnum { get; private set; } = UIHistoryModeEnum.Push;
        [field: SerializeField] public UICacheModeEnum CacheModeEnum { get; private set; } = UICacheModeEnum.Persistent;
    }
}

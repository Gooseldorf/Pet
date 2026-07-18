using UnityEngine;

namespace Pet.UI
{
    public abstract class UIPopupConfigBase : ScriptableObject
    {
        [field: SerializeField] public UIPopupViewBase Prefab { get; private set; }
        [field: SerializeField] public UiPopupQueueModeEnum QueueModeEnum { get; private set; } = UiPopupQueueModeEnum.Enqueue;
        [field: SerializeField] public UICacheModeEnum CacheModeEnum { get; private set; } = UICacheModeEnum.Persistent;
    }
}

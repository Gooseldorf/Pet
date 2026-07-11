using UnityEngine;

namespace Architecture.Configs
{
    [CreateAssetMenu(fileName = "ProjectConfig", menuName = "Configs/Project Config")]
    public class ProjectConfig : ScriptableObject
    {
        [field: SerializeField] public UIConfig UI { get; private set; }
    }
}

using UnityEngine;

namespace Architecture.UI
{
    public class UiRoot : MonoBehaviour
    {
        [SerializeField] private LoadingOverlay loadingOverlay;

        public LoadingOverlay LoadingOverlay => loadingOverlay;
    }
}

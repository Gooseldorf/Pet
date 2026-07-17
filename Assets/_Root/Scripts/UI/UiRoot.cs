using UnityEngine;

namespace Pet.UI
{
    public class UiRoot : MonoBehaviour
    {
        [SerializeField] private LoadingOverlay loadingOverlay;

        public LoadingOverlay LoadingOverlay => loadingOverlay;
    }
}

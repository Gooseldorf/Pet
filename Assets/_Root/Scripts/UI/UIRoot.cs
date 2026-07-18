using UnityEngine;

namespace Pet.UI
{
    public class UIRoot : MonoBehaviour
    {
        [SerializeField] private UILoadingOverlay loadingOverlay;
        [SerializeField] private Transform screenLayer;
        [SerializeField] private Transform popupLayer;
        [SerializeField] private Transform hudLayer;
        [SerializeField] private Transform overlayLayer;

        public UILoadingOverlay LoadingOverlay => loadingOverlay;

        public Transform GetLayer(UILayerEnum layerEnum)
        {
            return layerEnum switch
            {
                UILayerEnum.Screen => screenLayer,
                UILayerEnum.Popup => popupLayer,
                UILayerEnum.Hud => hudLayer,
                UILayerEnum.Overlay => overlayLayer,
                _ => throw new System.ArgumentOutOfRangeException(nameof(layerEnum), layerEnum, null)
            };
        }
    }
}

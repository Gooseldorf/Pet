using System.Collections.Generic;
using Pet.Gameplay;
using UnityEngine;
using VContainer;

namespace Pet.UI
{
    public class UIInstanceFactory
    {
        private readonly IObjectResolver objectResolver;
        private readonly UIRoot uiRoot;

        private readonly Dictionary<UIScreenConfigBase, UIScreenViewBase> cachedScreens = new();
        private readonly Dictionary<UIPopupConfigBase, UIPopupViewBase> cachedPopups = new();
        private readonly Dictionary<UIHudConfig, UIHudView> cachedHuds = new();

        public UIInstanceFactory(IObjectResolver objectResolver, UIRoot uiRoot)
        {
            this.objectResolver = objectResolver;
            this.uiRoot = uiRoot;
        }

        public UIScreenHandle GetScreen(UIScreenConfigBase config)
        {
            UIScreenViewBase view = GetOrCreateScreenView(config);
            return new UIScreenHandle(config, view);
        }

        public UIPopupHandle GetPopup(UIPopupConfigBase config)
        {
            UIPopupViewBase view = GetOrCreatePopupView(config);
            return new UIPopupHandle(config, view);
        }

        public UIHudView GetHud(UIHudConfig config)
        {
            return GetOrCreateHudView(config);
        }

        public void Release(UIScreenHandle handle)
        {
            ReleaseScreenView(handle.Config, handle.View);
        }

        public void Release(UIPopupHandle handle)
        {
            ReleasePopupView(handle.Config, handle.View);
        }

        public void Release(UIHudConfig config)
        {
            if (!cachedHuds.TryGetValue(config, out UIHudView view))
            {
                return;
            }

            Release(config, view);
        }

        public void Release(UIHudConfig config, UIHudView view)
        {
            if (config.CacheModeEnum == UICacheModeEnum.Persistent)
            {
                cachedHuds[config] = view;
            }

            if (config.CacheModeEnum == UICacheModeEnum.DestroyOnHide)
            {
                cachedHuds.Remove(config);
                Object.Destroy(view.gameObject);
                return;
            }

            view.HideInstant();
        }

        private UIScreenViewBase GetOrCreateScreenView(UIScreenConfigBase config)
        {
            if (config.CacheModeEnum == UICacheModeEnum.Persistent && cachedScreens.TryGetValue(config, out UIScreenViewBase cachedView))
            {
                return cachedView;
            }

            UIScreenViewBase view = CreateView(config.Prefab, UILayerEnum.Screen);
            if (config.CacheModeEnum == UICacheModeEnum.Persistent)
            {
                cachedScreens[config] = view;
            }

            return view;
        }

        private UIPopupViewBase GetOrCreatePopupView(UIPopupConfigBase config)
        {
            if (config.CacheModeEnum == UICacheModeEnum.Persistent && cachedPopups.TryGetValue(config, out UIPopupViewBase cachedView))
            {
                return cachedView;
            }

            UIPopupViewBase view = CreateView(config.Prefab, UILayerEnum.Popup);
            if (config.CacheModeEnum == UICacheModeEnum.Persistent)
            {
                cachedPopups[config] = view;
            }

            return view;
        }

        private UIHudView GetOrCreateHudView(UIHudConfig config)
        {
            if (config.CacheModeEnum == UICacheModeEnum.Persistent && cachedHuds.TryGetValue(config, out UIHudView cachedView))
            {
                return cachedView;
            }

            UIHudView view = CreateView(config.Prefab, UILayerEnum.Hud);
            if (config.CacheModeEnum == UICacheModeEnum.Persistent)
            {
                cachedHuds[config] = view;
            }

            return view;
        }

        private void ReleaseScreenView(UIScreenConfigBase config, UIScreenViewBase view)
        {
            if (config.CacheModeEnum == UICacheModeEnum.DestroyOnHide)
            {
                Object.Destroy(view.gameObject);
                return;
            }

            view.HideInstant();
        }

        private void ReleasePopupView(UIPopupConfigBase config, UIPopupViewBase view)
        {
            if (config.CacheModeEnum == UICacheModeEnum.DestroyOnHide)
            {
                Object.Destroy(view.gameObject);
                return;
            }

            view.HideInstant();
        }

        private TView CreateView<TView>(TView prefab, UILayerEnum layerEnum)
            where TView : UIViewBase
        {
            TView instance = Object.Instantiate(prefab, uiRoot.GetLayer(layerEnum), false);
            objectResolver.Inject(instance);
            instance.HideInstant();
            return instance;
        }
    }
}

namespace Pet.UI
{
    public sealed class UIPopupHandle
    {
        public UIPopupHandle(UIPopupConfigBase config, UIPopupViewBase view)
        {
            Config = config;
            View = view;
        }

        public UIPopupConfigBase Config { get; }
        public UIPopupViewBase View { get; }
    }
}

namespace Pet.UI
{
    public sealed class UIScreenHandle
    {
        public UIScreenHandle(UIScreenConfigBase config, UIScreenViewBase view)
        {
            Config = config;
            View = view;
        }

        public UIScreenConfigBase Config { get; }
        public UIScreenViewBase View { get; }
    }
}

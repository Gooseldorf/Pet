using Pet.Input;
using VContainer.Unity;

namespace Pet.MainMenu
{
    public class MainMenuEntryPoint : IStartable
    {
        private readonly InputActionsProvider inputActionsProvider;

        public MainMenuEntryPoint(InputActionsProvider inputActionsProvider)
        {
            this.inputActionsProvider = inputActionsProvider;
        }

        public void Start()
        {
            inputActionsProvider.SetEnabledMaps(InputMapKind.UI);
        }
    }
}

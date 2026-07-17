using Pet.Input;
using VContainer.Unity;

namespace Pet.Gameplay
{
    public class GameplayEntryPoint : IStartable
    {
        private readonly InputActionsProvider inputActionsProvider;

        public GameplayEntryPoint(InputActionsProvider inputActionsProvider)
        {
            this.inputActionsProvider = inputActionsProvider;
        }

        public void Start()
        {
            inputActionsProvider.SetEnabledMaps(InputMapKind.Player);
        }
    }
}

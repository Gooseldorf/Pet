using System;
using UnityEngine.InputSystem;

namespace Pet.Input
{
    public class InputActionsProvider : IDisposable
    {
        private readonly InputSystem_Actions actions;

        public InputActionsProvider()
        {
            actions = new InputSystem_Actions();
        }

        public InputAction Move => actions.Player.Move;
        public InputAction Look => actions.Player.Look;
        public InputAction Attack => actions.Player.Attack;
        public InputAction Interact => actions.Player.Interact;
        public InputAction Crouch => actions.Player.Crouch;
        public InputAction Jump => actions.Player.Jump;
        public InputAction Previous => actions.Player.Previous;
        public InputAction Next => actions.Player.Next;
        public InputAction Sprint => actions.Player.Sprint;

        public InputAction Navigate => actions.UI.Navigate;
        public InputAction Submit => actions.UI.Submit;
        public InputAction Cancel => actions.UI.Cancel;
        public InputAction Point => actions.UI.Point;
        public InputAction Click => actions.UI.Click;
        public InputAction ScrollWheel => actions.UI.ScrollWheel;

        public void SetEnabledMaps(InputMapKind mapKind)
        {
            actions.Player.Disable();
            actions.UI.Disable();

            switch (mapKind)
            {
                case InputMapKind.Player:
                    actions.Player.Enable();
                    break;
                case InputMapKind.UI:
                    actions.UI.Enable();
                    break;
                case InputMapKind.PlayerAndUI:
                    actions.Player.Enable();
                    actions.UI.Enable();
                    break;
            }
        }

        public void Dispose()
        {
            actions.Player.Disable();
            actions.UI.Disable();
            actions.Dispose();
        }
    }
}

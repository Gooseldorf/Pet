using System.Threading;
using R3;
using UnityEngine.InputSystem;

namespace Pet.Input
{
    public static class InputActionObservableExtensions
    {
        public static Observable<InputAction.CallbackContext> StartedAsObservable(
            this InputAction action,
            CancellationToken cancellationToken = default)
        {
            return CreateObservable(action, static (inputAction, handler) => inputAction.started += handler, static (inputAction, handler) => inputAction.started -= handler, cancellationToken);
        }

        public static Observable<InputAction.CallbackContext> PerformedAsObservable(
            this InputAction action,
            CancellationToken cancellationToken = default)
        {
            return CreateObservable(action, static (inputAction, handler) => inputAction.performed += handler, static (inputAction, handler) => inputAction.performed -= handler, cancellationToken);
        }

        public static Observable<InputAction.CallbackContext> CanceledAsObservable(
            this InputAction action,
            CancellationToken cancellationToken = default)
        {
            return CreateObservable(action, static (inputAction, handler) => inputAction.canceled += handler, static (inputAction, handler) => inputAction.canceled -= handler, cancellationToken);
        }

        private static Observable<InputAction.CallbackContext> CreateObservable(
            InputAction action,
            System.Action<InputAction, System.Action<InputAction.CallbackContext>> subscribe,
            System.Action<InputAction, System.Action<InputAction.CallbackContext>> unsubscribe,
            CancellationToken cancellationToken)
        {
            var subject = new Subject<InputAction.CallbackContext>();

            if (cancellationToken.IsCancellationRequested)
            {
                subject.OnCompleted();
                return subject;
            }

            void Handler(InputAction.CallbackContext context) => subject.OnNext(context);

            subscribe(action, Handler);

            cancellationToken.Register(() =>
            {
                unsubscribe(action, Handler);
                subject.OnCompleted();
            });

            return subject;
        }
    }
}

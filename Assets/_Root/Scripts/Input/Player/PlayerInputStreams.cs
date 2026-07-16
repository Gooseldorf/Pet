using System;
using System.Collections.Generic;
using System.Threading;
using R3;
using UnityEngine;

namespace Architecture.Input.Player
{
    public class PlayerInputStreams : IPlayerInputStreams, IDisposable
    {
        private readonly CancellationTokenSource disposeCancellationTokenSource = new();
        private readonly List<IDisposable> subscriptions = new();

        private readonly ReactiveProperty<Vector2> move = new(Vector2.zero);
        private readonly ReactiveProperty<Vector2> look = new(Vector2.zero);
        private readonly ReactiveProperty<bool> sprintHeld = new(false);
        private readonly ReactiveProperty<bool> crouchHeld = new(false);

        private readonly Subject<Unit> jumpPressed = new();
        private readonly Subject<Unit> attackPressed = new();
        private readonly Subject<Unit> interactStarted = new();
        private readonly Subject<Unit> interactCanceled = new();
        private readonly Subject<Unit> previousPressed = new();
        private readonly Subject<Unit> nextPressed = new();

        public PlayerInputStreams(InputActionsProvider inputActionsProvider)
        {
            BindPlayerActions(inputActionsProvider, disposeCancellationTokenSource.Token);
        }

        public PlayerInputState CurrentState => new(move.Value, look.Value, sprintHeld.Value, crouchHeld.Value);

        public Observable<Vector2> Move => move;
        public Observable<Vector2> Look => look;
        public Observable<bool> SprintHeld => sprintHeld;
        public Observable<bool> CrouchHeld => crouchHeld;

        public Observable<Unit> JumpPressed => jumpPressed;
        public Observable<Unit> AttackPressed => attackPressed;
        public Observable<Unit> InteractStarted => interactStarted;
        public Observable<Unit> InteractCanceled => interactCanceled;
        public Observable<Unit> PreviousPressed => previousPressed;
        public Observable<Unit> NextPressed => nextPressed;

        private void BindPlayerActions(InputActionsProvider inputActionsProvider, CancellationToken cancellationToken)
        {
            // Axes stay as state so movement systems can pull the latest value when they tick.
            subscriptions.Add(inputActionsProvider.Move
                .PerformedAsObservable(cancellationToken)
                .Subscribe(context => move.Value = context.ReadValue<Vector2>()));

            subscriptions.Add(inputActionsProvider.Move
                .CanceledAsObservable(cancellationToken)
                .Subscribe(_ => move.Value = Vector2.zero));

            subscriptions.Add(inputActionsProvider.Look
                .PerformedAsObservable(cancellationToken)
                .Subscribe(context => look.Value = context.ReadValue<Vector2>()));

            subscriptions.Add(inputActionsProvider.Look
                .CanceledAsObservable(cancellationToken)
                .Subscribe(_ => look.Value = Vector2.zero));

            // Hold-style buttons publish state transitions so downstream code does not need callback bookkeeping.
            subscriptions.Add(inputActionsProvider.Sprint
                .StartedAsObservable(cancellationToken)
                .Subscribe(_ => sprintHeld.Value = true));

            subscriptions.Add(inputActionsProvider.Sprint
                .CanceledAsObservable(cancellationToken)
                .Subscribe(_ => sprintHeld.Value = false));

            subscriptions.Add(inputActionsProvider.Crouch
                .StartedAsObservable(cancellationToken)
                .Subscribe(_ => crouchHeld.Value = true));

            subscriptions.Add(inputActionsProvider.Crouch
                .CanceledAsObservable(cancellationToken)
                .Subscribe(_ => crouchHeld.Value = false));

            // One-shot actions stay event-like so consumers can compose them without mutating shared state.
            subscriptions.Add(inputActionsProvider.Jump
                .PerformedAsObservable(cancellationToken)
                .Subscribe(_ => jumpPressed.OnNext(Unit.Default)));

            subscriptions.Add(inputActionsProvider.Attack
                .PerformedAsObservable(cancellationToken)
                .Subscribe(_ => attackPressed.OnNext(Unit.Default)));

            subscriptions.Add(inputActionsProvider.Interact
                .StartedAsObservable(cancellationToken)
                .Subscribe(_ => interactStarted.OnNext(Unit.Default)));

            subscriptions.Add(inputActionsProvider.Interact
                .CanceledAsObservable(cancellationToken)
                .Subscribe(_ => interactCanceled.OnNext(Unit.Default)));

            subscriptions.Add(inputActionsProvider.Previous
                .PerformedAsObservable(cancellationToken)
                .Subscribe(_ => previousPressed.OnNext(Unit.Default)));

            subscriptions.Add(inputActionsProvider.Next
                .PerformedAsObservable(cancellationToken)
                .Subscribe(_ => nextPressed.OnNext(Unit.Default)));
        }

        public void Dispose()
        {
            disposeCancellationTokenSource.Cancel();

            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            disposeCancellationTokenSource.Dispose();

            move.Dispose();
            look.Dispose();
            sprintHeld.Dispose();
            crouchHeld.Dispose();

            jumpPressed.Dispose();
            attackPressed.Dispose();
            interactStarted.Dispose();
            interactCanceled.Dispose();
            previousPressed.Dispose();
            nextPressed.Dispose();
        }
    }
}

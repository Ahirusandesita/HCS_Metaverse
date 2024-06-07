using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InteractionScopeChecker : MonoBehaviour
{
    private IInteraction prevInteraction = default;
    private IInteraction currentInteraction = default;
    private readonly Subject<IInteraction> onHitInteractionEnter = new Subject<IInteraction>();
    private readonly Subject<Unit> onHitInteractionExit = new Subject<Unit>();
    private readonly ReactiveProperty<ControllerColliderHit> hitRP = new ReactiveProperty<ControllerColliderHit>();

    public IObservable<IInteraction> OnHitInteractionSubject => onHitInteractionEnter;
    public IObservable<Unit> OnHitInteractionExit => onHitInteractionExit;
    public IReadOnlyReactiveProperty<ControllerColliderHit> HitRP => hitRP;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitRP.Value = hit;

        if (hit.gameObject.TryGetComponent(out currentInteraction))
        {
            if (currentInteraction == prevInteraction)
            {
                return;
            }

            onHitInteractionEnter.OnNext(currentInteraction);
            prevInteraction = currentInteraction;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InteractionScopeChecker : MonoBehaviour
{
    private IInteraction interaction = default;
    private readonly Subject<IInteraction> onHitInteractionEnter = new Subject<IInteraction>();
    private readonly Subject<Unit> onHitInteractionExit = new Subject<Unit>();
    private readonly ReactiveProperty<ControllerColliderHit> hitRP = new ReactiveProperty<ControllerColliderHit>();
    private readonly ReactiveProperty<bool> isHitInteractionRP = new ReactiveProperty<bool>();

    public IObservable<IInteraction> OnInteractionEnter => onHitInteractionEnter;
    public IObservable<Unit> OnInteractionExit => onHitInteractionExit;
    public IReadOnlyReactiveProperty<ControllerColliderHit> HitRP => hitRP;

    private void Awake()
    {
        isHitInteractionRP
            .Subscribe(isHitInteraction =>
            {
                if (isHitInteraction)
                {
                    onHitInteractionEnter.OnNext(interaction);
                }
                else
                {
                    onHitInteractionExit.OnNext(Unit.Default);
                }
            });
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitRP.Value = hit;

        if (hit.gameObject.TryGetComponent(out interaction))
        {
            isHitInteractionRP.Value = true;
        }
        else
        {
            isHitInteractionRP.Value = false;
        }
    }
}

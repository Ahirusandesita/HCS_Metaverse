using System;
using UniRx;
using UnityEngine;

public class InteractionScopeChecker : MonoBehaviour
{
    private readonly Subject<IInteraction> onInteractionEnter = new Subject<IInteraction>();
    private readonly Subject<Unit> onInteractionExit = new Subject<Unit>();

    public IObservable<IInteraction> OnInteractionEnter => onInteractionEnter;
    public IObservable<Unit> OnInteractionExit => onInteractionExit;


    private void Awake()
    {
        onInteractionEnter.AddTo(this);
        onInteractionExit.AddTo(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteraction interaction))
        {
            onInteractionEnter.OnNext(interaction);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteraction _))
        {
            onInteractionExit.OnNext(Unit.Default);
        }
    }
}

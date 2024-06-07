using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private InteractionScopeChecker interactionScopeChecker;
    [SerializeField]
    private SelectedNotificationDI DI;

    private List<ISelectedNotificationInjectable> selectedNotificationInjectables = new List<ISelectedNotificationInjectable>();

    public class NullInteraction : IInteraction
    {
        public ISelectedNotification SelectedNotification => new NullSelectedNotification();

        public void Close()
        {

        }

        public void Open()
        {

        }
    }
    private void Awake()
    {
        interactionScopeChecker.OnInteractionEnter.Subscribe(interaction => InteractionInject(interaction));
        interactionScopeChecker.OnInteractionExit.Subscribe(unit => InteractionInject(new NullInteraction()));
    }

    public void Add(ISelectedNotificationInjectable selectedNotificationInjectable)
    {
        selectedNotificationInjectables.Add(selectedNotificationInjectable);
    }
    public void Remove(ISelectedNotificationInjectable selectedNotificationInjectable)
    {
        selectedNotificationInjectables.Remove(selectedNotificationInjectable);
    }

    private void InteractionInject(IInteraction interaction)
    {
        DI.DependencyInjection(interaction, selectedNotificationInjectables);
    }
}

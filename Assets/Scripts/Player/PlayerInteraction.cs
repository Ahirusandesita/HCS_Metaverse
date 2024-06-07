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
    private IInteraction nowInteraction = new NullInteraction();

    private List<ISelectedNotificationInjectable> selectedNotificationInjectables = new List<ISelectedNotificationInjectable>();

    public class NullInteraction : IInteraction
    {
        public ISelectedNotification SelectedNotification => new NullSelectedNotification();

        public GameObject gameObject => throw new NotImplementedException();

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
        interactionScopeChecker.OnInteractionExit.Subscribe(unit => InteractionCloseInject(new NullInteraction()));
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
        this.nowInteraction = interaction;
        interaction.Open();
        DI.DependencyInjection(interaction, selectedNotificationInjectables);
    }
    private void InteractionCloseInject(IInteraction interaction)
    {
        nowInteraction.Close();
        DI.DependencyInjection(interaction, selectedNotificationInjectables);
        nowInteraction = interaction;
    }
}

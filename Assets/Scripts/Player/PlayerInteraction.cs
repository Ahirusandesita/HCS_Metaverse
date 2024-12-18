using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private InteractionScopeChecker interactionScopeChecker;
    [SerializeField]
    private SelectedNotificationDI DI;
    private List<IInteractionInfoReceiver> interactionInfoReceivers = default;
    private IInteraction nowInteraction = new NullInteraction();

    private List<ISelectedNotificationInjectable> selectedNotificationInjectables = new List<ISelectedNotificationInjectable>();

    public class NullInteraction : IInteraction
    {
        public ISelectedNotification SelectedNotification => new NullSelectedNotification();

        public GameObject gameObject => throw new NotImplementedException();

        public void Close()
        {

        }

        public IInteraction.InteractionInfo Open()
        {
            return new IInteraction.NullInteractionInfo();
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
    public void Add(IInteractionInfoReceiver interactionInfoReceiver)
	{
        interactionInfoReceivers.Add(interactionInfoReceiver);

    }
    public void Remove(ISelectedNotificationInjectable selectedNotificationInjectable)
    {
        selectedNotificationInjectables.Remove(selectedNotificationInjectable);
    }

    private void InteractionInject(IInteraction interaction)
    {
        this.nowInteraction = interaction;
        var info = interaction.Open();
		foreach (var item in interactionInfoReceivers)
		{
            item.SetInfo(info);
		}
        DI.DependencyInjection(interaction, selectedNotificationInjectables);
    }
    private void InteractionCloseInject(IInteraction interaction)
    {
        nowInteraction.Close();
        DI.DependencyInjection(interaction, selectedNotificationInjectables);
        nowInteraction = interaction;
    }
}
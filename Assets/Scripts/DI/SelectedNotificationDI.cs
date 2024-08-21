using HCSMeta.Activity;
using System.Collections.Generic;
using UnityEngine;

namespace HCSMeta.Function.Injection
{
    public class SelectedNotificationDI : MonoBehaviour
    {
        public void DependencyInjection(IInteraction interaction, ISelectedNotificationInjectable injectable)
        {
            Injection(interaction, new ISelectedNotificationInjectable[] { injectable });
        }
        public void DependencyInjection(IInteraction interaction, ISelectedNotificationInjectable[] injectables)
        {
            Injection(interaction, injectables);
        }
        public void DependencyInjection(IInteraction interaction, List<ISelectedNotificationInjectable> injectables)
        {
            Injection(interaction, injectables.ToArray());
        }

        private void Injection(IInteraction interaction, ISelectedNotificationInjectable[] injectables)
        {
            foreach (ISelectedNotificationInjectable injectable in injectables)
            {
                injectable.Inject(interaction.SelectedNotification);
            }
        }
    }
}
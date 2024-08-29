using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PracticableRPCEventInjector : MonoBehaviour
{
    [SerializeField]
    private RPCSpawner RPCSpawner;

    [SerializeField, InterfaceType(typeof(IInjectPracticableRPCEvent))]
    private List<UnityEngine.Object> IInjectPracticableRPCEvents = new List<Object>();
    private List<IInjectPracticableRPCEvent> injectRPCEvents => IInjectPracticableRPCEvents.OfType<IInjectPracticableRPCEvent>().ToList();

    private async void Start()
    {
        IPracticableRPCEvent practicableRPCEvent = await RPCSpawner.PracticableRPCEvent();

        foreach(IInjectPracticableRPCEvent injectPracticableRPCEvent in injectRPCEvents)
        {
            injectPracticableRPCEvent.Inject(practicableRPCEvent);
        }
    }
}

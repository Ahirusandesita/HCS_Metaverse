using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RPCSpawner : MonoBehaviour
{
    [SerializeField]
    private RPCEvent RPCEvent;
    private IPracticableRPCEvent practicableRPCEvent;
       
    private void Start()
    {
        if (GateOfFusion.Instance.IsUsePhoton)
        {
            Spawn();
        }
        else
        {
            practicableRPCEvent = new NullPracticableRPCEvent();
        }
    }

    private async void Spawn()
    {
        NetworkObject instance = await GateOfFusion.Instance.NetworkRunner.SpawnAsync(RPCEvent.gameObject);
        practicableRPCEvent = instance.GetComponent<IPracticableRPCEvent>();
    }

    public IPracticableRPCEvent PracticableRPCEvent
    {
        get
        {
            return practicableRPCEvent;
        }
    }

    public async void SpawnAsync(GameObject synchronizationGameObject)
    {
        if (GateOfFusion.Instance.IsUsePhoton)
        {
            NetworkObject item = await GateOfFusion.Instance.NetworkRunner.SpawnAsync(synchronizationGameObject);
            foreach(IInjectPracticableRPCEvent injectPracticableRPCEvent in item.GetComponents<IInjectPracticableRPCEvent>())
            {
                injectPracticableRPCEvent.Inject(PracticableRPCEvent);
            }
        }
        else
        {
            foreach(IInjectPracticableRPCEvent injectPracticableRPCEvent in Instantiate(synchronizationGameObject).GetComponents<IInjectPracticableRPCEvent>())
            {
                injectPracticableRPCEvent.Inject(practicableRPCEvent);
            }
        }
    }
}

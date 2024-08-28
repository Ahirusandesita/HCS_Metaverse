using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RPCSpawner : MonoBehaviour
{
    [SerializeField]
    private RPCEvent RPCEvent;
    private IPracticableRPCEvent practicableRPCEvent;
    private NetworkObject instance;
    
    private void Start()
    {
        Spawn();
    }

    private async void Spawn()
    {
        instance = await GateOfFusion.Instance.NetworkRunner.SpawnAsync(RPCEvent.gameObject);
        practicableRPCEvent = instance.GetComponent<IPracticableRPCEvent>();
    }

    public IPracticableRPCEvent PracticableRPCEvent
    {
        get
        {
            return practicableRPCEvent is null ? new NullPracticableRPCEvent() : practicableRPCEvent;
        }
    }

    public async void SpawnAsync(GameObject synchronizationGameObject)
    {
        if (instance)
        {
            NetworkObject item = await GateOfFusion.Instance.NetworkRunner.SpawnAsync(synchronizationGameObject);
            item.GetComponent<IInjectPracticableRPCEvent>().Inject(PracticableRPCEvent);
        }
    }
}

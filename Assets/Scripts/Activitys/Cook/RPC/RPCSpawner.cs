using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cysharp.Threading.Tasks;

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
        await UniTask.WaitUntil(() => GateOfFusion.Instance.NetworkRunner.CanSpawn);
        GameObject instance = await GateOfFusion.Instance.SpawnAsync(RPCEvent.gameObject);
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
            foreach (IInjectPracticableRPCEvent injectPracticableRPCEvent in item.GetComponents<IInjectPracticableRPCEvent>())
            {
                injectPracticableRPCEvent.Inject(PracticableRPCEvent);
            }
        }
        else
        {
            foreach (IInjectPracticableRPCEvent injectPracticableRPCEvent in Instantiate(synchronizationGameObject).GetComponents<IInjectPracticableRPCEvent>())
            {
                injectPracticableRPCEvent.Inject(practicableRPCEvent);
            }
        }
    }
}

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

    private bool canSpawn;
    public static RPCSpawner GetRPCSpawner()
    {
        return FindObjectOfType<RPCSpawner>();
    }
    private void Start()
    {
        canSpawn = false;
      

    }

    private async void Spawn()
    {
        await UniTask.WaitUntil(() => canSpawn);
        NetworkObject instance = await GateOfFusion.Instance.NetworkRunner.SpawnAsync(RPCEvent.gameObject);
        practicableRPCEvent = instance.GetComponent<IPracticableRPCEvent>();
    }

    public async UniTask<IPracticableRPCEvent> PracticableRPCEvent()
    {
        await UniTask.WaitUntil(() => practicableRPCEvent == null);
        return practicableRPCEvent;
    }

    public async void SpawnAsync(GameObject synchronizationGameObject)
    {
        if (GateOfFusion.Instance.IsUsePhoton)
        {
            NetworkObject item = await GateOfFusion.Instance.NetworkRunner.SpawnAsync(synchronizationGameObject);
            foreach (IInjectPracticableRPCEvent injectPracticableRPCEvent in item.GetComponents<IInjectPracticableRPCEvent>())
            {
                injectPracticableRPCEvent.Inject(await PracticableRPCEvent());
            }
        }
        else
        {
            foreach (IInjectPracticableRPCEvent injectPracticableRPCEvent in Instantiate(synchronizationGameObject).GetComponents<IInjectPracticableRPCEvent>())
            {
                injectPracticableRPCEvent.Inject(await PracticableRPCEvent());
            }
        }
    }
    public async void InjectAsync(GameObject obj)
    {
        foreach(IInjectPracticableRPCEvent item in obj.transform.root.transform.GetComponentsInChildren<IInjectPracticableRPCEvent>())
        {
            item.Inject(await PracticableRPCEvent());
        }
    }
}

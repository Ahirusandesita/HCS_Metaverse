using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class FoodSpawnManagerRPC : NetworkBehaviour
{
    [SerializeField]
    private FoodSpawnManager foodSpawnManager;

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_FoodSpawn(NetworkObject networkObject,int index)
    {
        foodSpawnManager.SelectedNotificationInjection(networkObject, index);
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_NotificationInjection(NetworkObject networkObject,int id,Vector3 position)
    {
        foodSpawnManager.SelectedNotificationInjection(networkObject, id, position);
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_MasterSelect(int id,Vector3 position)
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            foodSpawnManager.MasterSelect(id, position);
        }
    }
}

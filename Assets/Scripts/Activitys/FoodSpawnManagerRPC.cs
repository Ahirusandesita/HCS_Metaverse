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
        foodSpawnManager.UntiHuzakenna(networkObject, index);
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_Unti(NetworkObject networkObject,int id,Vector3 position)
    {
        foodSpawnManager.UntiHuzakennaSelect(networkObject, id, position);
    }
}

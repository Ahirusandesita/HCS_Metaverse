using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class FoodSpawnManagerRPC : NetworkBehaviour, IPlayerJoined
{
    [SerializeField]
    private FoodSpawnManager foodSpawnManager;

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_FoodSpawn(NetworkObject networkObject, int index)
    {
        foodSpawnManager.SelectedNotificationInjection(networkObject, index);
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_NotificationInjection(NetworkObject networkObject, int id, Vector3 position)
    {
        foodSpawnManager.SelectedNotificationInjection(networkObject, id, position);
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_MasterSelect(int id, Vector3 position)
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            foodSpawnManager.MasterSelect(id, position);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_SpawnNetworkView(int id, Vector3 position)
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            foodSpawnManager.SpawnNetworkView(id, position);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_StartSpawnNetworkView(int id, Vector3 position, int index)
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            foodSpawnManager.StartSpawnNetworkView(id, position, index);
        }
    }



    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_SpawnLocalView(int id, Vector3 position, NetworkObject networkObject)
    {
        foodSpawnManager.SpawnLocalView(id, position, networkObject.GetComponent<NetworkView>());
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_StartSpawnLocalView(int id, NetworkObject networkObject, int index)
    {
        foodSpawnManager.StartSpawnLocalView(id, networkObject.GetComponent<NetworkView>(), index);
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            foodSpawnManager.NewMember(player);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_Joined([RpcTarget] PlayerRef newPlayer, int id, NetworkObject networkObject)
    {
        foodSpawnManager.SpawnLocalView(id, networkObject.GetComponent<NetworkView>().transform.position, networkObject.GetComponent<NetworkView>());
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_JoinedOneGrab([RpcTarget] PlayerRef newPlayer, int id, NetworkObject networkObject)
    {
        foodSpawnManager.LateJoinSpawnLocalView(id, networkObject.GetComponent<NetworkView>().transform.position, networkObject.GetComponent<NetworkView>());
    }
}

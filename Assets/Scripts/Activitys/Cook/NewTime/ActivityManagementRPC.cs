using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class ActivityManagementRPC : NetworkBehaviour,IPlayerJoined
{

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_ReadyTimeInject(NetworkObject networkObject)
    {
        FindObjectOfType<ActivityProgressManagement>().RPC_ReadyInjectable(networkObject.GetComponent<TimeNetwork>());
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_MainTimeInject(NetworkObject networkObject)
    {
        FindObjectOfType<ActivityProgressManagement>().RPC_MainInjectable(networkObject.GetComponent<TimeNetwork>());
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_Inject([RpcTarget]PlayerRef playerRef, NetworkObject readyTime,NetworkObject mainTime)
    {
        FindObjectOfType<ActivityProgressManagement>().RPC_ReadyInjectable(readyTime.GetComponent<TimeNetwork>());
        FindObjectOfType<ActivityProgressManagement>().RPC_MainInjectable(mainTime.GetComponent<TimeNetwork>());
    }
    public async void PlayerJoined(PlayerRef player)
    {
        if(await GateOfFusion.Instance.GetIsLeader())
        {
            FindObjectOfType<ActivityProgressManagement>().RPC_Anpanman(player);
        }
    }
}

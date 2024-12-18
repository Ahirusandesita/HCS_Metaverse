using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cysharp.Threading.Tasks;

public class ActivityManagementRPC : NetworkBehaviour,IPlayerJoined
{
    private bool isStart = false;

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
    public void RPC_NetworkTimeInject([RpcTarget]PlayerRef playerRef, NetworkObject readyTime,NetworkObject mainTime,NetworkObject rpcObject)
    {
        FindObjectOfType<ActivityProgressManagement>().RPC_ReadyInjectable(readyTime.GetComponent<TimeNetwork>());
        FindObjectOfType<ActivityProgressManagement>().RPC_MainInjectable(mainTime.GetComponent<TimeNetwork>());
        FindObjectOfType<ActivityProgressManagement>().RPC_RPCInstance(rpcObject.GetComponent<ActivityManagementRPC>());
    }
    public async void PlayerJoined(PlayerRef player)
    {
        //if (!isStart)
        //{
        //    return;
        //}
        //âºé¿ëïÅ@Ç∆ÇËÇ†Ç¶Ç∏Ç‹Ç¬
        await UniTask.Delay(1000);

        if(GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            Debug.LogError("Joined");
            FindObjectOfType<ActivityProgressManagement>().RPC_Joined(player);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_RPCInstanceInject()
    {
        FindObjectOfType<ActivityProgressManagement>().RPC_RPCInstance(this);
    }
}

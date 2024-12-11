using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class ActivityManagementRPC : NetworkBehaviour
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class LeaderOrMemberRPC : NetworkBehaviour
{

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_ProsessComplete()
    {
        FindObjectOfType<LeaderOrMember>().ProcessComplete();
    }
}

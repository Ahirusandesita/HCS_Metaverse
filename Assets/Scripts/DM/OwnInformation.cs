using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class OwnInformation : NetworkBehaviour
{
    private NetworkObject networkObject;

    public PlayerRef MyPlayerRef => networkObject.StateAuthority;

    private void Awake()
    {
        networkObject = this.GetComponent<NetworkObject>();
    }


    //test
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_Message([RpcTarget]PlayerRef target,string message)
    {
        Debug.LogError("message");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class OwnInformation : NetworkBehaviour
{
    private NetworkObject networkObject;

    public PlayerRef MyPlayerRef => networkObject.StateAuthority;
    public string Name => "Test";
    private void Awake()
    {
        networkObject = this.GetComponent<NetworkObject>();
    }


    //test
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_Message([RpcTarget]PlayerRef target,string message,PlayerRef sender)
    {
        foreach(DM dm in FindObjectsOfType<DM>())
        {
            if (dm.IsTarget(sender))
            {
                dm.Message(message);
            }
        }
    }
}

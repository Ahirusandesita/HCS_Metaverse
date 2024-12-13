using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class LocalThrow : NetworkBehaviour
{
    // 
    private NetworkView _networkView = default;

    private void Start()
    {
        // 
        _networkView = GetComponent<NetworkView>();
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_ThrowAllLocalView(Vector3 throwVector)
    {
        // 
        _networkView.LocalView.GetComponentInChildren<Throwable>().Throw(throwVector);
    }
}

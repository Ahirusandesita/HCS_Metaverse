using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkCommodity : NetworkBehaviour
{
    private NetworkView _networkView = default;

    public NetworkView NetworkView => _networkView;

    [Rpc]
    public void RPC_Despawn()
    {

    }

    [Rpc]
    public void RPC_MixCommodity(NetworkObject hitObject)
    {
        _networkView.LocalView.GetComponent<Commodity>().RPC_MixCommodity(hitObject);
    }
}

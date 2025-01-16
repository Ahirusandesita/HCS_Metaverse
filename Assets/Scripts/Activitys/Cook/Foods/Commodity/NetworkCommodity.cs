using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkCommodity : NetworkBehaviour
{
    private NetworkView _networkView = default;

    public NetworkView NetworkView => _networkView;

    private void Awake()
    {
        _networkView = GetComponent<NetworkView>();
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_Despawn()
    {
        if (!GateOfFusion.Instance.IsActivityConnected || !GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            return;
        }

        FoodSpawnManagerRPC foodSpawnManagerRPC = GameObject.FindObjectOfType<FoodSpawnManagerRPC>();

        foodSpawnManagerRPC.RPC_Despawn(GetComponent<NetworkObject>());
        _networkView.LocalView.GetComponent<Commodity>().RPC_Destroy();
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_MixCommodity(NetworkObject hitObject, int commodityID)
    {
        if (!GateOfFusion.Instance.IsActivityConnected || !GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            return;
        }
        _networkView.LocalView.GetComponent<Commodity>().RPC_MixCommodity(hitObject, commodityID);
    }
}

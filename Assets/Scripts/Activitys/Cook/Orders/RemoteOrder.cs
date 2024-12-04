using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class RemoteOrder : NetworkBehaviour
{
    private Customer customer;
    private OrderManager orderManager;

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Order(int index)
    {
        Debug.LogWarning("‚Õ‚ñ‚Õ‚ñ");
        customer = GameObject.FindObjectOfType<Customer>();
        customer.RemoteOrder(index);
    }

    [Rpc(RpcSources.All,RpcTargets.All,InvokeLocal = false)]
    public void RPC_Submision(int index)
    {
        orderManager = GameObject.FindObjectOfType<OrderManager>();
        orderManager.RemoteSubmision(index);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Initialize()
    {
        orderManager = GameObject.FindObjectOfType<OrderManager>();
        orderManager.Inject(this);
    }
}

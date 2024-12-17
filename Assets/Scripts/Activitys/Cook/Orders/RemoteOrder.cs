using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class RemoteOrder : NetworkBehaviour, IPlayerJoined
{
    private OrderSystem customer;
    private OrderManager orderManager;

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_Order(int index, float orderWaitingTime, int orderWaitingType)
    {
        customer = GameObject.FindObjectOfType<OrderSystem>();
        customer.RemoteOrder(index, orderWaitingTime, orderWaitingType);
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
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

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_Cancel(int index)
    {
        orderManager = GameObject.FindObjectOfType<OrderManager>();
        orderManager.RemoteSubmision(index);
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            customer.NewMember(player);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_Order([RpcTarget]PlayerRef player,int index, float orderWaitingTime, int orderWaitingType)
    {
        customer = GameObject.FindObjectOfType<OrderSystem>();
        customer.RemoteOrder(index, orderWaitingTime, orderWaitingType);
    }
}

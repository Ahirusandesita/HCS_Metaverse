using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class RemoteOrder : NetworkBehaviour
{

    private OrderAsset orderAsset;

    private OrderManager orderManager;
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Order(int index)
    {
        OrderTicket orderTicket = orderManager.Inquiry();
        orderTicket.Orderable.Order(orderAsset.OrderDetailInformations[index].CommodityAsset, orderTicket.CustomerInformation);
    }

    public void InjectOrderManager(OrderManager orderManager)
    {
        this.orderManager = orderManager;
    }
    public void InjectOrderAsset(OrderAsset orderAsset)
    {
        this.orderAsset = orderAsset;
    }
}

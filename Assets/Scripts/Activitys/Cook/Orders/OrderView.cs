using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommodityInformation
{
    public readonly CommodityAsset CommodityAsset;
    public CommodityInformation(CommodityAsset commodityAsset)
    {
        this.CommodityAsset = commodityAsset;
    }
}
public class OrderView : MonoBehaviour
{
    private OrderInitializeEventArgs orderInitializeEventArgs;
    [SerializeField]
    private List<OrderViewDetailImformation> orderViewDetailInformations;

    public void OrderInitializeHandler(OrderInitializeEventArgs orderInitializeEventArgs)
    {
        this.orderInitializeEventArgs = orderInitializeEventArgs;

        for (int i = 0; i < orderViewDetailInformations.Count; i++)
        {
            orderViewDetailInformations[i].Reset();
        }
    }
    public void OrderHandler(OrderEventArgs orderEventArgs)
    {
        if (orderEventArgs.OrderType == OrderType.Order)
        {
            orderViewDetailInformations[orderEventArgs.OrderIndex].View(orderEventArgs.CommodityInformation.CommodityAsset);
        }

        if (orderEventArgs.OrderType == OrderType.Submit)
        {
            orderViewDetailInformations[orderEventArgs.OrderIndex].Reset();
        }
    }
    public void ResetOrderArrayHandler(ResetOrderArrayEventArgs resetOrderArrayEventArgs)
    {
        for (int i = 0; i < resetOrderArrayEventArgs.CommodityInformations.Length; i++)
        {
            if (resetOrderArrayEventArgs.CommodityInformations[i] == null)
            {
                orderViewDetailInformations[i].Reset();
                continue;
            }
            orderViewDetailInformations[i].View(resetOrderArrayEventArgs.CommodityInformations[i].CommodityAsset);
        }
    }
}

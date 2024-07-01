using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    private List<TextMeshProUGUI> textMeshProUGUIs;

    public void OrderInitializeHandler(OrderInitializeEventArgs orderInitializeEventArgs)
    {
        this.orderInitializeEventArgs = orderInitializeEventArgs;

        for (int i = orderInitializeEventArgs.OrderValue; i < textMeshProUGUIs.Count; i++)
        {
            textMeshProUGUIs[i].enabled = false;
        }
    }
    public void OrderHandler(OrderEventArgs orderEventArgs)
    {
        if (orderEventArgs.OrderType == OrderType.Order)
        {
            textMeshProUGUIs[orderEventArgs.OrderIndex].text = orderEventArgs.CommodityInformation.CommodityAsset.name;
        }

        if(orderEventArgs.OrderType == OrderType.Submit)
        {
            textMeshProUGUIs[orderEventArgs.OrderIndex].text = "";
        }
    }
    public void ResetOrderArrayHandler(ResetOrderArrayEventArgs resetOrderArrayEventArgs)
    {
        for(int i = 0; i < resetOrderArrayEventArgs.CommodityInformations.Length; i++)
        {
            if(resetOrderArrayEventArgs.CommodityInformations[i] == null)
            {
                textMeshProUGUIs[i].text = "";
                continue;
            }
            textMeshProUGUIs[i].text = resetOrderArrayEventArgs.CommodityInformations[i].CommodityAsset.name;
        }
    }
}

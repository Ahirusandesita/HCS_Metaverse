using UnityEngine;
using System;
public interface IOrderable
{
    void Order(CommodityAsset commodityAsset);
}
public interface ISubmitable
{
    void Submission(Commodity commodity);
}
public enum OrderType
{
    Order,
    Submit
}
public class OrderEventArgs : System.EventArgs
{
    public readonly CommodityInformation CommodityInformation;
    public readonly OrderType OrderType;
    public readonly int OrderIndex;
    public OrderEventArgs(CommodityInformation commodityInformation, OrderType orderType, int orderIndex)
    {
        this.CommodityInformation = commodityInformation;
        this.OrderType = orderType;
        this.OrderIndex = orderIndex;
    }
}
public class OrderInitializeEventArgs : System.EventArgs
{
    public readonly int OrderValue;
    public OrderInitializeEventArgs(int orderValue)
    {
        this.OrderValue = orderValue;
    }
}
public class ResetOrderArrayEventArgs : System.EventArgs
{
    public readonly CommodityInformation[] CommodityInformations;
    public ResetOrderArrayEventArgs(CommodityInformation[] commodityInformations)
    {
        this.CommodityInformations = commodityInformations;
    }
}

public delegate void OrderHandler(OrderEventArgs orderEventArgs);
public delegate void OrderInitializeHandler(OrderInitializeEventArgs orderInitializeEventArgs);
public delegate void ResetOrderArrayHandler(ResetOrderArrayEventArgs resetOrderArrayEventArgs);



public class OrderManager : MonoBehaviour, IOrderable, ISubmitable
{
    [SerializeField]
    private int orderValue;

    private CommodityAsset[] commodityAssets;
    private CommodityInformation[] commodityInformations;

    public event OrderHandler OnOrder;
    public event OrderInitializeHandler OnOrderInitialize;
    public event ResetOrderArrayHandler OnResetOrder;

    private void Start()
    {
        commodityAssets = new CommodityAsset[orderValue];
        commodityInformations = new CommodityInformation[orderValue];
        OnOrderInitialize?.Invoke(new OrderInitializeEventArgs(orderValue));
    }

    /// <summary>
    /// íçï∂
    /// </summary>
    /// <param name="commodityAsset"></param>
    public void Order(CommodityAsset commodityAsset)
    {
        if (SearchVacantSeatOrder() == commodityAssets.Length)
        {
            return;
        }
        int vacantSeatOrder = SearchVacantSeatOrder();

        commodityAssets[vacantSeatOrder] = commodityAsset;

        OrderEventArgs orderEventArgs = new OrderEventArgs(new CommodityInformation(commodityAsset), OrderType.Order, vacantSeatOrder);
        OnOrder?.Invoke(orderEventArgs);
    }

    public void Submission(Commodity commodity)
    {
        for (int i = 0; i < commodityAssets.Length; i++)
        {
            if (commodity.IsMatchCommodity(commodityAssets[i]))
            {
                //íÒèoäÆóπ
                OrderEventArgs orderEventArgs = new OrderEventArgs(new CommodityInformation(commodityAssets[i]), OrderType.Submit, i);
                commodityAssets[i] = null;
                PackOrders();
                OnOrder?.Invoke(orderEventArgs);
                break;
            }
        }

        for (int i = 0; i < commodityAssets.Length; i++)
        {
            if (commodityAssets[i] == null)
            {
                commodityInformations[i] = null;
                continue;
            }
            commodityInformations[i] = new CommodityInformation(commodityAssets[i]);
        }

        OnResetOrder?.Invoke(new ResetOrderArrayEventArgs(commodityInformations));
    }

    private int SearchVacantSeatOrder()
    {
        for (int i = 0; i < commodityAssets.Length; i++)
        {
            if (commodityAssets[i] is null)
            {
                return i;
            }
        }
        return commodityAssets.Length;
    }

    private void PackOrders()
    {
        for (int i = 0; i < commodityAssets.Length - 1; i++)
        {
            if (commodityAssets[i] == null)
            {
                for (int k = i + 1; k < commodityAssets.Length; k++)
                {
                    if (commodityAssets[k] == null)
                    {
                        continue;
                    }
                    commodityAssets[i] = commodityAssets[k];
                    commodityAssets[k] = null;

                    break;
                }
            }
        }
    }
}

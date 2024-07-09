using UnityEngine;
using System;
public interface IOrderable
{
    void Order(CommodityAsset commodityAsset,CustomerInformation customer);
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

public class CustomerInformation
{
    public readonly int OrderCode;
    public CustomerInformation(int orderCode)
    {
        this.OrderCode = orderCode;
    }
}


    public class OrderTicket
    {
        public readonly IOrderable Orderable;
        public readonly CustomerInformation CustomerInformation;
        public OrderTicket(IOrderable orderable,CustomerInformation customer)
        {
            this.Orderable = orderable;
            this.CustomerInformation = customer;
        }
    }
public class OrderManager : MonoBehaviour, IOrderable, ISubmitable
{
    public class NullOrderable : IOrderable
    {
        public void Order(CommodityAsset commodityAsset, CustomerInformation customer)
        {
            
        }
    }



    [SerializeField]
    private int orderValue;

    private CommodityAsset[] commodityAssets;
    private CustomerInformation[] customers;

    private CommodityInformation[] commodityInformations;

    public event OrderHandler OnOrder;
    public event OrderInitializeHandler OnOrderInitialize;
    public event ResetOrderArrayHandler OnResetOrder;

    private int orderCode = 0;
    private void Awake()
    {
        commodityAssets = new CommodityAsset[orderValue];
        commodityInformations = new CommodityInformation[orderValue];
        customers = new CustomerInformation[orderValue];
    }
    private void Start()
    {
        OnOrderInitialize?.Invoke(new OrderInitializeEventArgs(orderValue));     
    }

    public OrderTicket Inquiry()
    {
        if (SearchVacantSeatOrder() == commodityAssets.Length)
        {
            return new OrderTicket(new NullOrderable(), new CustomerInformation(-1));
        }
        return new OrderTicket(this, new CustomerInformation(orderCode));
    }

    /// <summary>
    /// íçï∂
    /// </summary>
    /// <param name="commodityAsset"></param>
    void IOrderable.Order(CommodityAsset commodityAsset,CustomerInformation customer)
    {
        int vacantSeatOrder = SearchVacantSeatOrder();

        commodityAssets[vacantSeatOrder] = commodityAsset;
        customers[vacantSeatOrder] = customer;

        OrderEventArgs orderEventArgs = new OrderEventArgs(new CommodityInformation(commodityAsset), OrderType.Order, vacantSeatOrder);
        OnOrder?.Invoke(orderEventArgs);
        orderCode++;
    }

    public void Submission(Commodity commodity)
    {
        for (int i = 0; i < commodityAssets.Length; i++)
        {
            if(commodityAssets[i] == null)
            {
                continue;
            }
            if (commodity.IsMatchCommodity(commodityAssets[i]))
            {
                //íÒèoäÆóπ
                Debug.Log("íÒèoäÆóπ");
                OrderEventArgs orderEventArgs = new OrderEventArgs(new CommodityInformation(commodityAssets[i]), OrderType.Submit, i);
                commodityAssets[i] = null;
                customers[i] = null;
                PackOrders();

                Destroy(commodity.gameObject);
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

    public void Cancel(CustomerInformation customer)
    {
        for(int i = 0; i < customers.Length; i++)
        {
            if(customers[i].OrderCode == customer.OrderCode)
            {
                commodityAssets[i] = null;
                customers[i] = null;
                PackOrders();
            }
        }
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

                    customers[i] = customers[k];
                    customers[k] = null;

                    break;
                }
            }
        }
    }
}

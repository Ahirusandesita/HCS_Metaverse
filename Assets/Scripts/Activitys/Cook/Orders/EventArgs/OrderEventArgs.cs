public class OrderEventArgs : System.EventArgs
{
    public readonly CommodityInformation CommodityInformation;
    public readonly OrderType OrderType;
    public readonly int OrderIndex;
    public readonly CustomerInformation CustomerInformation;
    public OrderEventArgs(CommodityInformation commodityInformation, OrderType orderType, int orderIndex, CustomerInformation customerInformation)
    {
        this.CommodityInformation = commodityInformation;
        this.OrderType = orderType;
        this.OrderIndex = orderIndex;
        this.CustomerInformation = customerInformation;
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
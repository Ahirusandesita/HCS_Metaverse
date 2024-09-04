using UnityEngine;
using System;


public class OrderManager : MonoBehaviour, IOrderable, ISubmitable
{
    //Test
    [SerializeField]
    private OrderAsset orderAsset;
    [SerializeField]
    private Customer customer;
    
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
    [SerializeField]
    private RemoteOrder remoteOrder;
    private RemoteOrder instance;
    private void Awake()
    {
        commodityAssets = new CommodityAsset[orderValue];
        commodityInformations = new CommodityInformation[orderValue];
        customers = new CustomerInformation[orderValue];

        if (GameObject.FindObjectOfType<Leader>())
            Initialize();
    }
    private async void Initialize()
    {
        GameObject remoteOrderObject = await GateOfFusion.Instance.SpawnAsync(remoteOrder.gameObject);
        instance = remoteOrderObject.GetComponent<RemoteOrder>();

        customer.InjectRemoteOrder(instance);
        instance.RPC_Initialize();
    }
    public void Inject(RemoteOrder remoteOrder)
    {
        customer.InjectRemoteOrder(remoteOrder);
        instance = remoteOrder;
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
    void IOrderable.Order(CommodityAsset commodityAsset, CustomerInformation customer)
    {
        int vacantSeatOrder = SearchVacantSeatOrder();

        commodityAssets[vacantSeatOrder] = commodityAsset;
        customers[vacantSeatOrder] = customer;

        OrderEventArgs orderEventArgs = new OrderEventArgs(new CommodityInformation(commodityAsset), OrderType.Order, vacantSeatOrder);
        OnOrder?.Invoke(orderEventArgs);
        orderCode++;
    }
    public bool CanSubmit(Commodity commodity)
    {
        for (int i = 0; i < commodityAssets.Length; i++)
        {
            if (commodityAssets[i] == null)
            {
                continue;
            }
            if (commodity.IsMatchCommodity(commodityAssets[i]))
            {
                return true;
            }
        }
        return false;
    }
    public void Submission(Commodity commodity)
    {
        for (int i = 0; i < commodityAssets.Length; i++)
        {
            if (commodityAssets[i] == null)
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

                instance.RPC_Submision(i);
                break;
            }
        }

        GateOfFusion.Instance.NetworkRunner.Despawn(commodity.GetComponent<Fusion.NetworkObject>());


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

    public void RemoteSubmision(int index)
    {
        OrderEventArgs orderEventArgs = new OrderEventArgs(new CommodityInformation(commodityAssets[index]), OrderType.Submit, index);
        commodityAssets[index] = null;
        customers[index] = null;
        PackOrders();

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
        for (int i = 0; i < customers.Length; i++)
        {
            if (customers[i].OrderCode == customer.OrderCode)
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

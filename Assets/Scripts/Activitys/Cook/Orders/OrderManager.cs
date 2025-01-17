using UnityEngine;
using System;
public enum OrderWaitingType
{
    Hide,
    Manifest
}

public class OrderManager : MonoBehaviour, IOrderable, ISubmitable
{
    //Test
    [SerializeField]
    private OrderAsset orderAsset;
    [SerializeField]
    private OrderSystem customer;

    private IScoreCalculator scoreCalculator;
    private int chainValue = 0;
    public class NullOrderable : IOrderable
    {
        public void Order(CommodityAsset commodityAsset, CustomerInformation customer)
        {

        }
        public void Cancel(CustomerInformation customerInformation)
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

    public event Action<CustomerInformation> OnSubmission;

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
        {
            Initialize();
            customer.IsLeader = true;
        }
        else
        {
            customer.IsLeader = false;
        }

        scoreCalculator = InterfaceUtils.FindObjectOfInterfaces<IScoreCalculator>()[0];
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
        customer.InjectOrderAsset(orderAsset);
    }

    public OrderTicket Inquiry(float orderWaitingTime, OrderWaitingType orderWaitingType)
    {
        if (SearchVacantSeatOrder() == commodityAssets.Length)
        {
            return new OrderTicket(new NullOrderable(), new CustomerInformation(-1, orderWaitingTime, orderWaitingType));
        }
        return new OrderTicket(this, new CustomerInformation(orderCode, orderWaitingTime, orderWaitingType));
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

        OrderEventArgs orderEventArgs = new OrderEventArgs(new CommodityInformation(commodityAsset, customer), OrderType.Order, vacantSeatOrder, customer);
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
        CustomerInformation customerInformation = null;
        if (!GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            return;
        }
        for (int i = 0; i < commodityAssets.Length; i++)
        {
            if (commodityAssets[i] == null)
            {
                continue;
            }
            if (commodity.IsMatchCommodity(commodityAssets[i]))
            {
                customerInformation = customers[i];
                commodityAssets[i] = null;
                customers[i] = null;
                PackOrders();

                if (i == 0)
                {
                    chainValue++;
                }
                else
                {
                    chainValue = 0;
                }
                //Score
                scoreCalculator.GetScoreCalculator.ScoreCalucuration(commodity.CommodityAsset.Score, chainValue);
                instance.RPC_Submision(i, chainValue);
                break;
            }
        }

        Debug.LogError("Despawn Commodity");
        //GateOfFusion.Instance.NetworkRunner.Despawn(commodity.GetComponent<Fusion.NetworkObject>());


        for (int i = 0; i < commodityAssets.Length; i++)
        {
            if (commodityAssets[i] == null)
            {
                commodityInformations[i] = null;
                continue;
            }
            commodityInformations[i] = new CommodityInformation(commodityAssets[i], customers[i]);
        }

        OnResetOrder?.Invoke(new ResetOrderArrayEventArgs(commodityInformations));
        OnSubmission?.Invoke(customerInformation);
    }

    public void RemoteSubmision(int index, int chainValue)
    {
        this.chainValue = chainValue;
        scoreCalculator.GetScoreCalculator.ScoreCalucuration(commodityAssets[index].Score, chainValue);
        commodityAssets[index] = null;
        customers[index] = null;
        PackOrders();
        //score
        for (int i = 0; i < commodityAssets.Length; i++)
        {
            if (commodityAssets[i] == null)
            {
                commodityInformations[i] = null;
                continue;
            }
            commodityInformations[i] = new CommodityInformation(commodityAssets[i], customers[i]);
        }

        OnResetOrder?.Invoke(new ResetOrderArrayEventArgs(commodityInformations));
    }

    public void Cancel(CustomerInformation customer)
    {
        for (int i = 0; i < customers.Length; i++)
        {
            if (customers[i] == null)
            {
                continue;
            }
            if (customers[i].OrderCode == customer.OrderCode)
            {
                commodityAssets[i] = null;
                customers[i] = null;
                PackOrders();

                instance.RPC_Cancel(i);
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
            commodityInformations[i] = new CommodityInformation(commodityAssets[i], customers[i]);
        }
        OnResetOrder?.Invoke(new ResetOrderArrayEventArgs(commodityInformations));
    }
    public void Cancel(int index)
    {
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
            commodityInformations[i] = new CommodityInformation(commodityAssets[i], customers[i]);
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

                    customers[i] = customers[k];
                    customers[k] = null;

                    break;
                }
            }
        }
    }
}

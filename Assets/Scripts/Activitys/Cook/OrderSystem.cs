using Cysharp.Threading.Tasks;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class OrderInformation
{
    public readonly OrderTicket OrderTicket;
    public readonly int AssetIndex;
    public OrderInformation(OrderTicket orderTicket, int assetIndex)
    {
        this.OrderTicket = orderTicket;
        this.AssetIndex = assetIndex;
    }
}
public class OrderSystem : MonoBehaviour
{
    private OrderAsset orderAsset;
    [SerializeField]
    private OrderManager orderManager;
    private RemoteOrder remoteOrder;
    private ConnectionChecker _connectonChecker = new ConnectionChecker();
    public bool IsLeader { get; set; }

    [SerializeField]
    private ActivityProgressManagement activityProgressManagement;

    private List<OrderInformation> orderTickets = new List<OrderInformation>();
    private List<OrderInformation> removeOrderTickets = new List<OrderInformation>();

    private bool isActivityConnected = false;
    private async void Start()
    {
        isActivityConnected = false;
        activityProgressManagement.OnStart += () =>
        {
            if (_connectonChecker.IsConnection)
            {
                StartCoroutine(Co());
            }
        };

        await UniTask.WaitUntil(() => GateOfFusion.Instance.IsActivityConnected);
        isActivityConnected = true;
    }
    public OrderTicket Order(int index, float orderWaitingTime, OrderWaitingType orderWaitingType)
    {
        remoteOrder.RPC_Order(index, orderWaitingTime, (int)orderWaitingType);
        OrderTicket orderTicket = orderManager.Inquiry(orderWaitingTime, orderWaitingType);
        orderTicket.Orderable.Order(orderAsset.OrderDetailInformations[index].CommodityAsset, orderTicket.CustomerInformation);
        orderTickets.Add(new OrderInformation(orderTicket, index));
        return orderTicket;
    }
    public void NewMember(PlayerRef player)
    {
        foreach (OrderInformation item in orderTickets)
        {
            remoteOrder.RPC_Order(player, item.AssetIndex, item.OrderTicket.CustomerInformation.RemainingTime, (int)item.OrderTicket.CustomerInformation.OrderWaitingType);
        }
    }

    public void InjectRemoteOrder(RemoteOrder remoteOrder)
    {
        this.remoteOrder = remoteOrder;
    }

    public void InjectOrderAsset(OrderAsset orderAsset)
    {
        this.orderAsset = orderAsset;
        orderManager.OnSubmission += On;
        orderManager.OnSubmission += (info) =>
        {
            orderTickets.Remove(orderTickets.Where(ticket => ticket.OrderTicket.CustomerInformation.OrderCode == info.OrderCode).First());
        };
    }

    private void Update()
    {
        if (!isActivityConnected)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(Co());
        }

        if (!_connectonChecker.IsConnection)
        {
            return;
        }


        for (int i = 0; i < orderTickets.Count; i++)
        {

            OrderInformation orderInformation = orderTickets[i];
            if (orderInformation.OrderTicket.CustomerInformation.RemainingTime <= 0f)
            {
                orderInformation.OrderTicket.Orderable.Cancel(orderInformation.OrderTicket.CustomerInformation);
                StartCoroutine(Co());
                removeOrderTickets.Add(orderInformation);

            }
        }
        if (removeOrderTickets.Count > 0)
        {
            orderTickets = orderTickets.Except(removeOrderTickets).ToList();
            removeOrderTickets.Clear();
        }
    }

    public void RemoteOrder(int index, float orderWaitingTime, int orderWaitingType)
    {
        OrderTicket orderTicket = orderManager.Inquiry(orderWaitingTime, (OrderWaitingType)orderWaitingType);
        orderTicket.Orderable.Order(orderAsset.OrderDetailInformations[index].CommodityAsset, orderTicket.CustomerInformation);
        orderTickets.Add(new OrderInformation(orderTicket, index));
    }

    private void On(CustomerInformation customerInformation)
    {
        if (_connectonChecker.IsConnection)
        {
            StartCoroutine(Co());
        }
    }

    private IEnumerator Co()
    {
        yield return new WaitForSeconds(2f);
        OrderTicket orderTicket = Order(Random.Range(0, orderAsset.OrderDetailInformations.Count), 60f, OrderWaitingType.Manifest);
    }
}
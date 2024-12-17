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
    public bool IsLeader { get; set; }

    [SerializeField]
    private ActivityProgressManagement activityProgressManagement;

    private List<OrderInformation> orderTickets = new List<OrderInformation>();
    private List<OrderInformation> removeOrderTickets = new List<OrderInformation>();

    private void Start()
    {
        activityProgressManagement.OnStart += () =>
        {
            if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
            {
                StartCoroutine(Co());
            }
        };
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(Co());
        }

        if (!GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
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

    private async void On(CustomerInformation customerInformation)
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            StartCoroutine(Co());
        }
    }

    private IEnumerator Co()
    {
        yield return new WaitForSeconds(2f);
        OrderTicket orderTicket = Order(Random.Range(0, orderAsset.OrderDetailInformations.Count), Random.Range(10f, 20f), OrderWaitingType.Manifest);
    }
}
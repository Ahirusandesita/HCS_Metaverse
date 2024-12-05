using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSystem : MonoBehaviour
{
    private OrderAsset orderAsset;
    [SerializeField]
    private OrderManager orderManager;
    private RemoteOrder remoteOrder;
    public bool IsLeader { get; set; }

    [SerializeField]
    private ActivityProgressManagement activityProgressManagement;
    private void Start()
    {
        activityProgressManagement.OnStart += () =>
        {
            StartCoroutine(Co());
        };
    }
    public OrderTicket Order(int index, float orderWaitingTime, OrderWaitingType orderWaitingType)
    {
        remoteOrder.RPC_Order(index, orderWaitingTime, (int)orderWaitingType);
        OrderTicket orderTicket = orderManager.Inquiry(orderWaitingTime, orderWaitingType);
        orderTicket.Orderable.Order(orderAsset.OrderDetailInformations[index].CommodityAsset, orderTicket.CustomerInformation);

        return orderTicket;
    }

    public void InjectRemoteOrder(RemoteOrder remoteOrder)
    {
        this.remoteOrder = remoteOrder;
    }

    public void InjectOrderAsset(OrderAsset orderAsset)
    {
        this.orderAsset = orderAsset;
        orderManager.OnSubmission += On;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(Co());
        }
    }

    public void RemoteOrder(int index, float orderWaitingTime, int orderWaitingType)
    {
        OrderTicket orderTicket = orderManager.Inquiry(orderWaitingTime, (OrderWaitingType)orderWaitingType);
        orderTicket.Orderable.Order(orderAsset.OrderDetailInformations[index].CommodityAsset, orderTicket.CustomerInformation);
    }

    private async void On()
    {
        if (await GateOfFusion.Instance.GetIsLeader())
        {
            StartCoroutine(Co());
        }
    }

    private IEnumerator Co()
    {
        yield return new WaitForSeconds(2f);
        OrderTicket orderTicket = Order(Random.Range(0, orderAsset.OrderDetailInformations.Count), Random.Range(60f, 120f), OrderWaitingType.Manifest);
        StartCoroutine(CancelTime(orderTicket));
    }

    private IEnumerator CancelTime(OrderTicket orderTicket)
    {
        yield return new WaitForSeconds(orderTicket.CustomerInformation.OrderWaitingTime);
        orderTicket.Orderable.Cancel(orderTicket.CustomerInformation);
    }
}
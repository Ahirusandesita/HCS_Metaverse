using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
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
    public void Order(int index)
    {
        remoteOrder.RPC_Order(index);
    }

    public void InjectRemoteOrder(RemoteOrder remoteOrder)
    {
        this.remoteOrder = remoteOrder;
    }

    public void InjectOrderAsset(OrderAsset orderAsset)
    {
        this.orderAsset = orderAsset;
        orderManager.OnResetOrder += On;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Order(0);
        }
    }

    public void RemoteOrder(int index)
    {
        OrderTicket orderTicket = orderManager.Inquiry();
        orderTicket.Orderable.Order(orderAsset.OrderDetailInformations[index].CommodityAsset, orderTicket.CustomerInformation);
    }

    private void On(ResetOrderArrayEventArgs resetOrderArrayEventArgs)
    {
        if (FindObjectOfType<Leader>())
        {
            StartCoroutine(Co());
        }
    }

    private IEnumerator Co()
    {
        yield return new WaitForSeconds(2f);
        Order(Random.Range(0, orderAsset.OrderDetailInformations.Count));
    }
}
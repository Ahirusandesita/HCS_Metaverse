using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmisionTable : MonoBehaviour
{
    private OrderManager orderManager;
    [SerializeField]
    private OrderAsset orderAsset;

    private void Awake()
    {
        orderManager = GameObject.FindObjectOfType<OrderManager>();

        orderManager.OnOrder += OrderHandler;
    }

    private void OrderHandler(OrderEventArgs orderEventArgs)
    {
        if(orderEventArgs.OrderType == OrderType.Submit)
        {
            StartCoroutine(NewOrder());
        }
    }
    private IEnumerator NewOrder()
    {
        yield return new WaitForSeconds(1f);

        int orderIndex = Random.Range(0, orderAsset.OrderDetailInformations.Count - 1);

        OrderManager.OrderTicket orderTicket = orderManager.Inquiry();
        orderTicket.Orderable.Order(orderAsset.OrderDetailInformations[orderIndex].CommodityAsset,orderTicket.Customer);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            foreach(OrderDetailInformation orderDetailInformation in orderAsset.OrderDetailInformations)
            {
                OrderManager.OrderTicket orderTicket = orderManager.Inquiry();
                orderTicket.Orderable.Order(orderDetailInformation.CommodityAsset, orderTicket.Customer);
            }
        }
    }

    public void Sub(Commodity commodity)
    {
        orderManager.Submission(commodity);
    }
}

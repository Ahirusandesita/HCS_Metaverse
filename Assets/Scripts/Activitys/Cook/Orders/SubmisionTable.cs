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
        if (orderEventArgs.OrderType == OrderType.Submit)
        {
            StartCoroutine(NewOrder());
        }
    }
    private IEnumerator NewOrder()
    {
        yield return new WaitForSeconds(1f);

        int orderIndex = Random.Range(0, orderAsset.OrderDetailInformations.Count - 1);

        //OrderTicket orderTicket = orderManager.Inquiry();
       // orderTicket.Orderable.Order(orderAsset.OrderDetailInformations[orderIndex].CommodityAsset, orderTicket.CustomerInformation);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            foreach (OrderDetailInformation orderDetailInformation in orderAsset.OrderDetailInformations)
            {
                //OrderTicket orderTicket = orderManager.Inquiry();
                //orderTicket.Orderable.Order(orderDetailInformation.CommodityAsset, orderTicket.CustomerInformation);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!GateOfFusion.Instance.IsActivityConnected)
        {
            return;
        }

        if (!GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            return;
        }

        if (collision.gameObject.TryGetComponent<Commodity>(out Commodity hitCommodity))
        {
            Submit(hitCommodity);
            hitCommodity.LocalView.NetworkView.GetComponent<NetworkCommodity>().RPC_Despawn();
        }
    }

    public void Submit(Commodity commodity)
    {
        if (orderManager.CanSubmit(commodity))
        {
            orderManager.Submission(commodity);
        }
    }

}

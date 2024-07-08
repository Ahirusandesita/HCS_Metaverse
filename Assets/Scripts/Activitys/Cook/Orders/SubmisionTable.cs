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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            orderManager.Order(orderAsset.OrderDetailInformations[0].CommodityAsset);
        }
    }

    public void Sub(Commodity commodity)
    {
        orderManager.Submission(commodity);
    }
}

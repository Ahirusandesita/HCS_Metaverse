using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderTest : MonoBehaviour
{
    [SerializeField]
    private Commodity commodity;
    [SerializeField]
    private CommodityAsset commodityAsset;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            GameObject.FindObjectOfType<OrderManager>().Order(commodityAsset);
        if (Input.GetKeyDown(KeyCode.L))
            GameObject.FindObjectOfType<OrderManager>().Submission(commodity);
    }
}

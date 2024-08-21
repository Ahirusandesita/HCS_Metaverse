using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HCSMeta.Activity.Cook
{
    public class OrderTest : MonoBehaviour
    {
        [SerializeField]
        private Commodity commodity;
        [SerializeField]
        private CommodityAsset commodityAsset;
        [SerializeField]
        private CommodityAsset burger;
        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.K))
            //    GameObject.FindObjectOfType<OrderManager>().Order(commodityAsset);
            //if (Input.GetKeyDown(KeyCode.B))
            //{
            //    GameObject.FindObjectOfType<OrderManager>().Order(burger);
            //}
            if (Input.GetKeyDown(KeyCode.L))
                GameObject.FindObjectOfType<OrderManager>().Submission(commodity);
        }
    }
}

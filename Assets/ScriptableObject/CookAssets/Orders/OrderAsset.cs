using System.Collections.Generic;
using UnityEngine;
public enum OrderProbabilityType
{
    Low,
    Middle,
    Hight
}

[CreateAssetMenu(fileName = "OrderAsset", menuName = "ScriptableObjects/Foods/OrderAsset")]
public class OrderAsset : ScriptableObject
{
    [SerializeField]
    private List<OrderDetailInformation> orderDetailInformations = new List<OrderDetailInformation>();
}
[System.Serializable]
public class OrderDetailInformation
{
    [SerializeField]
    private CommodityAsset commodityAsset;
    [SerializeField]
    private OrderProbabilityType orderProbability;
}


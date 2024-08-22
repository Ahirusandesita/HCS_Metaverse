using System.Collections.Generic;
using UnityEngine;
using HCSMeta.Activity.Cook.Interface;

namespace HCSMeta.Activity.Cook.Interface
{
    public enum OrderProbabilityType
    {
        Low,
        Middle,
        Hight
    }
}


namespace HCSMeta.Activity.Cook
{
    [CreateAssetMenu(fileName = "OrderAsset", menuName = "ScriptableObjects/Foods/OrderAsset")]
    public class OrderAsset : ScriptableObject
    {
        [SerializeField]
        private List<OrderDetailInformation> orderDetailInformations = new List<OrderDetailInformation>();
        public IReadOnlyList<OrderDetailInformation> OrderDetailInformations => orderDetailInformations;
    }
    [System.Serializable]
    public class OrderDetailInformation
    {
        [SerializeField]
        private CommodityAsset commodityAsset;
        [SerializeField]
        private OrderProbabilityType orderProbability;

        public CommodityAsset CommodityAsset => commodityAsset;
        public OrderProbabilityType OrderProbabilityType => orderProbability;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCSMeta.Activity.Cook.Interface;

namespace HCSMeta.Activity.Cook.Interface
{
    public interface IAllCommodityAsset
    {
        List<Commodity> Commodities { set; }
    }
}

namespace HCSMeta.Activity.Cook
{
    [CreateAssetMenu(fileName = "AllCommodityAsset", menuName = "ScriptableObjects/Foods/AllCommodityAsset")]
    public class AllCommodityAsset : ScriptableObject, IAllCommodityAsset
    {
        [SerializeField]
        private List<Commodity> commodities = new List<Commodity>();
        public IReadOnlyList<Commodity> Commodities => commodities;

        List<Commodity> IAllCommodityAsset.Commodities
        {
            set
            {
                commodities = value;
            }
        }
    }
}

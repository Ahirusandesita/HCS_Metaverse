using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IAllCommodityAsset
{
    List<Commodity> Commodities { set; }
}
[CreateAssetMenu(fileName = "AllCommodityAsset", menuName = "ScriptableObjects/Foods/AllCommodityAsset")]
public class AllCommodityAsset : ScriptableObject,IAllCommodityAsset
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

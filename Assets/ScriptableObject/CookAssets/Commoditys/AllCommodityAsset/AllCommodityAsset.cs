using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllCommodityAsset", menuName = "ScriptableObjects/Foods/AllCommodityAsset")]
public class AllCommodityAsset : ScriptableObject
{
    [SerializeField]
    private List<Commodity> commodities = new List<Commodity>();
    public IReadOnlyList<Commodity> Commodities => commodities;
}

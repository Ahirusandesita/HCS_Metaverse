using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CommodityAppearance
{
    [SerializeField]
    private Sprite commoditySprite;
    public Sprite CommoditySprite => commoditySprite;
}
public class CommodityDetailAppearance
{
    public CommodityDetailAppearance(CommodityAppearance commodityAppearance,IReadOnlyList<ProcessedGoodsAsset> processedGoodsAssets)
    {

    }
}

[CreateAssetMenu(fileName = "CommodityAsset", menuName = "ScriptableObjects/Foods/CommodityAsset")]
public class CommodityAsset : ScriptableObject
{
    [SerializeField]
    private List<ProcessedGoodsAsset> processedGoodsAssets = new List<ProcessedGoodsAsset>();
    [SerializeField]
    private CommodityAppearance commodityAppearance;
    public IReadOnlyList<ProcessedGoodsAsset> ProcessedGoodsAssets => processedGoodsAssets;

    public CommodityDetailAppearance GetCommodityDetailAppearance()
    {
        return new CommodityDetailAppearance(commodityAppearance, ProcessedGoodsAssets);
    }
   
}

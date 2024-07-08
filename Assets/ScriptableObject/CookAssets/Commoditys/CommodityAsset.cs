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
    public CommodityDetailAppearance(CommodityAppearance commodityAppearance,IReadOnlyList<Commodity> commodities)
    {

    }
}
public interface IGrantableCommodityID
{
    void GrantID(int id);
}
public interface ICommodityAssetModerator
{
    void SetCommodities(List<Commodity> commodities);
}

[CreateAssetMenu(fileName = "CommodityAsset", menuName = "ScriptableObjects/Foods/CommodityAsset")]
public class CommodityAsset : ScriptableObject,IGrantableCommodityID,ICommodityAssetModerator
{
    [SerializeField, HideInInspector]
    private int commodityID;

    [SerializeField]
    private List<Commodity> commodities = new List<Commodity>();
    [SerializeField]
    private CommodityAppearance commodityAppearance;
    public IReadOnlyList<Commodity> Commodities => commodities;
    public int CommodityID => commodityID;
    public CommodityDetailAppearance GetCommodityDetailAppearance()
    {
        return new CommodityDetailAppearance(commodityAppearance, commodities);
    }

    public CommodityAppearance CommodityAppearance => commodityAppearance;

    void IGrantableCommodityID.GrantID(int id)
    {
        commodityID = id;
    }

    void ICommodityAssetModerator.SetCommodities(List<Commodity> commodities)
    {
        this.commodities = commodities;
    }
   
}

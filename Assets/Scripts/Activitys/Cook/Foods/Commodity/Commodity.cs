using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Commodity : MonoBehaviour
{
    [SerializeField]
    private CommodityAsset commodityAsset;
    public CommodityAsset CommodityAsset => this.commodityAsset;

    public bool IsMatchCommodity(CommodityAsset commodityAsset)
    {
        if(this.commodityAsset.CommodityID == commodityAsset.CommodityID)
        {
            return true;
        }
        return false;
    }

    public bool CanInstanceCommodity(Commodity[] commodities)
    {
        if(commodityAsset.Commodities.Count != commodities.Length)
        {
            return false;
        }
    
        List<Commodity> targetCommodity = new List<Commodity>();

        foreach(Commodity commodity in commodities)
        {
            targetCommodity.Add(commodity);
        }

        foreach(Commodity commodity in commodityAsset.Commodities)
        {
            foreach(Commodity target in targetCommodity)
            {
                if(commodity.CommodityAsset.CommodityID == target.CommodityAsset.CommodityID)
                {
                    targetCommodity.Remove(target);
                    break;
                }
            }
        }
        if(targetCommodity.Count == 0)
        {
            return true;
        }
        return false;
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commodity : MonoBehaviour
{
    [SerializeField]
    private CommodityAsset commodityAsset;
    public CommodityAsset CommodityAsset => commodityAsset;

    public bool IsMatchCommodity(CommodityAsset commodity)
    {
        if (CommodityAsset.ProcessedGoodsAssets.Count != commodity.ProcessedGoodsAssets.Count)
        {
            return false;
        }

        for (int myProcessedGoods = 0; myProcessedGoods < commodityAsset.ProcessedGoodsAssets.Count; myProcessedGoods++)
        {
            if (!(commodityAsset.ProcessedGoodsAssets[myProcessedGoods].isMatchProcessedGoods(commodity.ProcessedGoodsAssets[myProcessedGoods])))
            {
                return false;
            }
        }

        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commodity : MonoBehaviour
{
    [SerializeField]
    private CommodityAsset commodityAsset;
    public CommodityAsset CommodityAsset => this.commodityAsset;

    public bool IsMatchCommodity(CommodityAsset commodityAsset)
    {
        if (CommodityAsset.ProcessedGoodsAssets.Count != commodityAsset.ProcessedGoodsAssets.Count)
        {
            return false;
        }

        for (int myProcessedGoods = 0; myProcessedGoods < this.commodityAsset.ProcessedGoodsAssets.Count; myProcessedGoods++)
        {
            if (!(this.commodityAsset.ProcessedGoodsAssets[myProcessedGoods].isMatchProcessedGoods(commodityAsset.ProcessedGoodsAssets[myProcessedGoods])))
            {
                return false;
            }
        }

        return true;
    }
}

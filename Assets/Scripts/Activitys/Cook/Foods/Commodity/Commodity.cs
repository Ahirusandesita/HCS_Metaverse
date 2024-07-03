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
    public bool CanInstanceCommodity(ProcessedGoods right,ProcessedGoods left)
    {
        return IsMatch(new ProcessedGoodsAsset[] { right.ProcessedGoodsAsset, left.ProcessedGoodsAsset});
    }

    public bool CanInstanceCommodity(Commodity commodity, ProcessedGoods processedGoods)
    {
        ProcessedGoodsAsset[] items = new ProcessedGoodsAsset[commodity.CommodityAsset.ProcessedGoodsAssets.Count + 1];

        for(int i = 0; i < commodity.CommodityAsset.ProcessedGoodsAssets.Count; i++)
        {
            items[i] = commodity.CommodityAsset.ProcessedGoodsAssets[i];
        }

        items[items.Length - 1] = processedGoods.ProcessedGoodsAsset;

        return IsMatch(items);
    }

    private bool IsMatch(ProcessedGoodsAsset[] processedGoodsAssets)
    {
        List<ProcessedGoodsAsset> myCommodityProcessedAssets = new List<ProcessedGoodsAsset>();
        foreach(ProcessedGoodsAsset processedGoodsAsset in CommodityAsset.ProcessedGoodsAssets)
        {
            myCommodityProcessedAssets.Add(processedGoodsAsset);
        }
        List<ProcessedGoodsAsset> targetProcessedGoods = new List<ProcessedGoodsAsset>();
        foreach(ProcessedGoodsAsset item in processedGoodsAssets)
        {
            targetProcessedGoods.Add(item);
        }



        if(myCommodityProcessedAssets.Count != processedGoodsAssets.Length)
        {
            return false;
        }

        foreach(ProcessedGoodsAsset item in CommodityAsset.ProcessedGoodsAssets)
        {
            for(int i = 0; i < targetProcessedGoods.Count; i++)
            {
                if (item.isMatchProcessedGoods(targetProcessedGoods[i]))
                {
                    myCommodityProcessedAssets.Remove(item);
                    targetProcessedGoods.Remove(targetProcessedGoods[i]);
                    break;
                }
            }
        }

        if(myCommodityProcessedAssets.Count == 0)
        {
            return true;
        }

        return false;
    }
}

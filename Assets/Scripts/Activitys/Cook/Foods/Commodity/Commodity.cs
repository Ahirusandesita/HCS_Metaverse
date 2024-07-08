using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public interface ICommodityModerator
{
    void SetCommodityAsset(CommodityAsset commodityAsset);
}
public class Commodity : MonoBehaviour,ICommodityModerator
{
    [SerializeField]
    private CommodityAsset commodityAsset;
    public CommodityAsset CommodityAsset => this.commodityAsset;
    void ICommodityModerator.SetCommodityAsset(CommodityAsset commodityAsset)
    {
        this.commodityAsset = commodityAsset;
    }
    public bool IsMatchCommodity(CommodityAsset commodityAsset)
    {
        if (this.commodityAsset.CommodityID == commodityAsset.CommodityID)
        {
            return true;
        }
        return false;
    }

    public bool CanInstanceCommodity(Commodity[] commodities)
    {
        if (commodityAsset.Commodities.Count != commodities.Length)
        {
            return false;
        }

        List<Commodity> targetCommodity = new List<Commodity>();

        foreach (Commodity commodity in commodities)
        {
            targetCommodity.Add(commodity);
        }

        foreach (Commodity commodity in commodityAsset.Commodities)
        {
            foreach (Commodity target in targetCommodity)
            {
                if (commodity.CommodityAsset.CommodityID == target.CommodityAsset.CommodityID)
                {
                    targetCommodity.Remove(target);
                    break;
                }
            }
        }
        if (targetCommodity.Count == 0)
        {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.TryGetComponent<Commodity>(out Commodity commodity))
        {
            if(CommodityAsset.CommodityID > commodity.CommodityAsset.CommodityID)
            {
                Commodity mixCommodity = MixCommodity.Mix(new Commodity[] { this, commodity });
                if(!(mixCommodity is null))
                {
                    Instantiate(mixCommodity, this.transform.position, this.transform.rotation);
                }
            }
        }
    }
}

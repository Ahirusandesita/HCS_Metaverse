using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Oculus.Interaction;
public interface ICommodityModerator
{
    void SetCommodityAsset(CommodityAsset commodityAsset);
}
public class Commodity : MonoBehaviour,ICommodityModerator,ISwitchableGrabbableActive
{
    [SerializeField]
    private CommodityAsset commodityAsset;
    public CommodityAsset CommodityAsset => this.commodityAsset;

    private Grabbable grabbable;
    private IPutableOnDish putableOnDish = new NullPutableOnDish();

    private void Awake()
    {
        grabbable = this.GetComponent<Grabbable>();
    }
    public void InjectGrabbable(Grabbable grabbable)
    {
        this.grabbable = grabbable;
    }
    public void InjectPutableOnDish(IPutableOnDish putableOnDish)
    {
        this.putableOnDish = putableOnDish;
    }
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
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.root.TryGetComponent<Commodity>(out Commodity commodity))
        {
            if(CommodityAsset.CommodityID > commodity.CommodityAsset.CommodityID)
            {
                Commodity mixCommodity = MixCommodity.Mix(new Commodity[] { this, commodity });
                if(!(mixCommodity is null))
                {
                    Instantiate(mixCommodity, this.transform.position, this.transform.rotation).PutOnDish(this.putableOnDish);
                }
            }
        }

        if(collision.gameObject.TryGetComponent<SubmisionTable>(out SubmisionTable table))
        {
            table.Sub(this);
        }

        if(collision.transform.root.gameObject.TryGetComponent<IPutableOnDish>(out IPutableOnDish putableOnDish))
        {
            this.putableOnDish = putableOnDish;
            this.putableOnDish.PutCommodity(this);
        }
    }

    public void Active()
    {
        grabbable.enabled = true;
    }

    public void Inactive()
    {
        grabbable.enabled = false;
    }
    public void PutOnDish(IPutableOnDish putableOnDish)
    {
        this.putableOnDish = putableOnDish;
        this.putableOnDish.PutCommodity(this);
    }
}

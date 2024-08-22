using System.Collections.Generic;
using UnityEngine;
public class MixCommodity : MonoBehaviour
{
    [SerializeField]
    private AllCommodityAsset allCommodity;
    private static AllCommodityAsset allCommodity_static;

    private void Awake()
    {
        allCommodity_static = allCommodity;
    }


    public static Commodity Mix(Commodity[] commodities)
    {
        List<Commodity> workCommodity = new List<Commodity>();

        foreach (Commodity item in commodities)
        {
            if (item.CommodityAsset.Commodities.Count == 0)
            {
                workCommodity.Add(item);
            }
            else
            {
                foreach (Commodity item2 in item.CommodityAsset.Commodities)
                {
                    workCommodity.Add(item2);
                }
            }
        }

        foreach (Commodity item in allCommodity_static.Commodities)
        {
            if (item.CanInstanceCommodity(workCommodity.ToArray()))
            {
                Debug.Log("ê∂ê¨â¬î\" + item.CommodityAsset.name);

                foreach (Commodity commodity in commodities)
                {
                    Destroy(commodity.gameObject);
                }
                return item;
            }
        }

        return null;//å„Ç…NullObject
    }
}


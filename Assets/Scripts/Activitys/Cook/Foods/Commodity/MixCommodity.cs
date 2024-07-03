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
        foreach(Commodity item in allCommodity_static.Commodities)
        {
            if (item.CanInstanceCommodity(commodities))
            {
                Debug.Log("ê∂ê¨â¬î\" + item.CommodityAsset.name);

                foreach(Commodity commodity in commodities)
                {
                    Destroy(commodity.gameObject);
                }
                return item;
            }
        }

        return null;//å„Ç…NullObject
    }
}

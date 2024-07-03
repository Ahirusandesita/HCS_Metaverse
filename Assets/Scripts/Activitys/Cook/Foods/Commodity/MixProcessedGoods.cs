using UnityEngine;

public class MixProcessedGoods : MonoBehaviour
{
    [SerializeField]
    private AllCommodityAsset allCommodity;

    public void Mix(Commodity[] commodities)
    {
        foreach(Commodity item in allCommodity.Commodities)
        {
            if (item.CanInstanceCommodity(commodities))
            {
                Debug.Log("ê∂ê¨â¬î\" + item.CommodityAsset.name);
                //ê∂ê¨â¬î\Ç»äÆê¨ïi
                return;
            }
        }
    }
}

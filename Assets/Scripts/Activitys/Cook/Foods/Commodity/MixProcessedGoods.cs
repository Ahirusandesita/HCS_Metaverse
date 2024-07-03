using UnityEngine;

public class MixProcessedGoods : MonoBehaviour
{
    [SerializeField]
    private AllCommodityAsset allCommodity;
    public void Mix(ProcessedGoods right, ProcessedGoods left)
    {
        foreach (Commodity commodity in allCommodity.Commodities)
        {
            if(commodity.CanInstanceCommodity(right, left))
            {
                Debug.Log("生成可能" + commodity.CommodityAsset.name);
                //生成可能な完成品　Instantiate
                return;
            }
        }
    }

    public void Mix(Commodity commodity,ProcessedGoods processedGoods)
    {
        foreach(Commodity item in allCommodity.Commodities)
        {
            if (item.CanInstanceCommodity(commodity, processedGoods))
            {
                Debug.Log("生成可能" + item.CommodityAsset.name);
                //生成可能な完成品
                return;
            }
        }
    }
}

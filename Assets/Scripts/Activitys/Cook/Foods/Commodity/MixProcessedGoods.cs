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
                Debug.Log("�����\" + commodity.CommodityAsset.name);
                //�����\�Ȋ����i�@Instantiate
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
                Debug.Log("�����\" + item.CommodityAsset.name);
                //�����\�Ȋ����i
                return;
            }
        }
    }
}

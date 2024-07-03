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
                Debug.Log("�����\" + item.CommodityAsset.name);
                //�����\�Ȋ����i
                return;
            }
        }
    }
}

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


    public static void Mix(Commodity[] commodities)
    {
        foreach(Commodity item in allCommodity_static.Commodities)
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

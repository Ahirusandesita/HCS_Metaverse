using UnityEngine;

public class CommodityFactory : MonoBehaviour
{
    [SerializeField]
    private AllCommodityAsset allCommodityAsset;

    public Commodity Generate(Ingrodients ingrodients, ProcessingType processingType)
    {
        foreach (IngrodientsDetailInformation item in ingrodients.IngrodientsAsset.IngrodientsDetailInformations)
        {
            if (item.ProcessingType == processingType)
            {
                return item.Commodity;
            }
        }

        return null;//Œã‚ÉNullObject
    }
    public int CommodityIndex(Commodity commodity)
    {
        return allCommodityAsset.CommodityIndex(commodity);
    }
}

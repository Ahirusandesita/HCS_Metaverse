using UnityEngine;

public class CommodityFactory : MonoBehaviour
{
    public void Generate(Ingrodients ingrodients,ProcessingType processingType)
    {
        foreach(IngrodientsDetailInformation item in ingrodients.IngrodientsAsset.IngrodientsDetailInformations)
        {
            if(item.ProcessingType == processingType)
            {
                //�����@Instantiate(item.Commodity);
            }
        }
    }
}

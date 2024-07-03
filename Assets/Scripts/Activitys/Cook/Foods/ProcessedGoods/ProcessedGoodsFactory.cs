using UnityEngine;

public class ProcessedGoodsFactory : MonoBehaviour
{
    [SerializeField]
    private ProcessedUnityAsset processedUnityAsset;

    public void Generate(Ingrodients ingrodients,ProcessingType processingType)
    {
        foreach(IngrodientsDetailInformation item in ingrodients.IngrodientsAsset.IngrodientsDetailInformations)
        {
            if(item.ProcessingType == processingType)
            {
                //ê∂ê¨Å@Instantiate(item.Commodity);
            }
        }
    }
}

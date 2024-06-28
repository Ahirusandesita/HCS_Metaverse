using UnityEngine;

public class ProcessedGoodsFactory : MonoBehaviour
{
    [SerializeField]
    private ProcessedUnityAsset processedUnityAsset;

    public void Generate(Ingrodients ingrodients,ProcessingType processingType)
    {
        foreach(ProcessedGoods processedGoods in processedUnityAsset.ProcessedGoods)
        {
            if(ingrodients.IngrodientsAsset.IngrodientsType == processedGoods.ProcessedGoodsAsset.IngrodientsType)
            {
                if(processedGoods.ProcessedGoodsAsset.ProcessingType == processingType)
                {
                    // ê∂ê¨Å@Instantiate(processedGoods);
                }
            }
        }
    }
}

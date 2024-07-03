using UnityEngine;

public class ProcessedGoodsFactory : MonoBehaviour
{
    [SerializeField]
    private ProcessedUnityAsset processedUnityAsset;

    public void Generate(Ingrodients ingrodients,ProcessingType processingType)
    {
        foreach(ProcessedGoods processedGoods in processedUnityAsset.ProcessedGoods)
        {
            if (ingrodients.IngrodientsAsset.IngrodientsType.MatchFoodType(processingType, processedGoods.ProcessedGoodsAsset.ProcessedType))
            {
                //ê∂ê¨Å@Instantiate(processedGoods);
            }
        }

        foreach(IngrodientsDetailInformation item in ingrodients.IngrodientsAsset.IngrodientsDetailInformations)
        {
            if(item.ProcessingType == processingType)
            {
                //ê∂ê¨Å@Instantiate(item.Commodity);
            }
        }
    }
}

public static class FoodExtends
{
    public static bool MatchFoodType(this IngrodientsType ingrodientsType,ProcessingType processingType,ProcessedType processedType)
    {
        if(ingrodientsType == IngrodientsType.Meat && processingType == ProcessingType.Bake && processedType == ProcessedType.GrilledMeat)
        {
            return true;
        }
        return false;
    }
}

using UnityEngine;

namespace HCSMeta.Activity.Cook
{
    public class CommodityFactory : MonoBehaviour
    {
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
    }
}

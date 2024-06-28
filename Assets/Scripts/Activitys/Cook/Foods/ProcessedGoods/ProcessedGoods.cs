using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessedGoods : MonoBehaviour
{
    [SerializeField]
    private ProcessedGoodsAsset processedGoodsAsset;

    public ProcessedGoodsAsset ProcessedGoodsAsset => processedGoodsAsset;
}

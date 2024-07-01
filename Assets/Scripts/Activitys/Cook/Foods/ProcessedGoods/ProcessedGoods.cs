using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessedGoods : MonoBehaviour
{
    [SerializeField]
    private ProcessedGoodsAsset processedGoodsAsset;
    [SerializeField]
    private Sprite processedGoodsSprite;

    public ProcessedGoodsAsset ProcessedGoodsAsset => processedGoodsAsset;

    public Sprite ProcessedGoodsSprite => processedGoodsSprite;
}

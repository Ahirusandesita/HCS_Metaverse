using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProcessedAsset", menuName = "ScriptableObjects/Foods/ProcessedGoodsAsset")]
public class ProcessedGoodsAsset : ScriptableObject
{
    [SerializeField]
    private IngrodientsType ingrodientsType;
    [SerializeField]
    private ProcessingType processingType;
    /// <summary>
    /// ���H������ɉ��H���������ꍇ�̐H�i���j�󂳂��܂ł̎���
    /// </summary>
    [SerializeField]
    private float timeToDestruction;

    public IngrodientsType IngrodientsType => ingrodientsType;
    public ProcessingType ProcessingType => processingType;
    public float TimeToDestruction => timeToDestruction;

    public bool isMatchProcessedGoods(ProcessedGoodsAsset processedGoodsAsset)
    {
        if (IngrodientsType == processedGoodsAsset.IngrodientsType &&
            ProcessingType == processedGoodsAsset.ProcessingType)
        {
            return true;
        }
        return false;
    }
}

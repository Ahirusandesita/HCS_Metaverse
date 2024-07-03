using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ProcessedType
{
    GrilledMeat
}
[CreateAssetMenu(fileName = "ProcessedAsset", menuName = "ScriptableObjects/Foods/ProcessedGoodsAsset")]
public class ProcessedGoodsAsset : ScriptableObject
{
    [SerializeField]
    private ProcessedType processedType;

    /// <summary>
    /// ���H������ɉ��H���������ꍇ�̐H�i���j�󂳂��܂ł̎���
    /// </summary>
    [SerializeField]
    private float timeToDestruction;

    public ProcessedType ProcessedType => processedType;
    public float TimeToDestruction => timeToDestruction;

    public bool isMatchProcessedGoods(ProcessedGoodsAsset processedGoodsAsset)
    {
        if(processedType == processedGoodsAsset.ProcessedType)
        {
            return true;
        }
        return false;
    }
}

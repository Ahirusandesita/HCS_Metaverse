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
    /// ‰ÁHŠ®—¹Œã‚É‰ÁH‚µ‘±‚¯‚½ê‡‚ÌH•i‚ª”j‰ó‚³‚ê‚é‚Ü‚Å‚ÌŠÔ
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

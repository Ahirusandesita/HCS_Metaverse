using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProcessedUnityAsset", menuName = "ScriptableObjects/Foods/ProcessedUnityAsset")]
public class ProcessedUnityAsset : ScriptableObject
{
    [SerializeField]
    private List<ProcessedGoods> processedGoods = new List<ProcessedGoods>();

    public IReadOnlyList<ProcessedGoods> ProcessedGoods => processedGoods;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "ScriptableObjects/FishAsset")]
public class FishAsset : ScriptableObject
{
    [SerializeField] private string fishName = default;
    [SerializeField] private int sellingPrice = default;
    [Tooltip("������R����i���[�����񂷁j����[s]")]
    [SerializeField] private float resistTime = default;

    public string FishName => fishName;
    public int SellingPrice => sellingPrice;
    public float ResistTime => resistTime;
}
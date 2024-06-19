using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InitializeAsset", menuName = "ScriptableObjects/InitializeAsset")]
public class InitializeAsset : ScriptableObject
{
    [SerializeField]
    private List<Initialize> initializes = new List<Initialize>();
}
[System.Serializable]
internal class Initialize
{
    [SerializeField]
    private List<GameObject> initializeObjects = new List<GameObject>();
}

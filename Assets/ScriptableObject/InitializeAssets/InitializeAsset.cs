using System.Collections.Generic;
using UnityEngine;
public enum InitialzeType
{
    Reticle
}

[CreateAssetMenu(fileName = "InitializeAsset", menuName = "ScriptableObjects/InitializeAsset")]
public class InitializeAsset : ScriptableObject
{
    [SerializeField]
    private List<GameObject> initializeObjects = new List<GameObject>();
    [SerializeField]
    private InitialzeType initialzeType;

    public List<GameObject> InitializeObjects => initializeObjects;
}

using System.Collections.Generic;
using UnityEngine;
public enum InitializeType
{
    ReticleLeftHand,
    ReticleRightHand,
    PokeableCanvas
}

[CreateAssetMenu(fileName = "InitializeAsset", menuName = "ScriptableObjects/InitializeAsset")]
public class InitializeAsset : ScriptableObject
{
    [SerializeField]
    private List<GameObject> initializeObjects = new List<GameObject>();
    [SerializeField]
    private InitializeType initialzeType;

    public List<GameObject> InitializeObjects => initializeObjects;

    public InitializeType InitializeType => initialzeType;
}

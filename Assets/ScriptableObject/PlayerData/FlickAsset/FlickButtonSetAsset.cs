using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlickAsset", menuName = "ScriptableObjects/FlickAsset")]
public class FlickButtonSetAsset : ScriptableObject
{
    public List<FlickChild> flickChilds = new List<FlickChild>();
}

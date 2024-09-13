using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsIntersectsTest : MonoBehaviour
{
    [SerializeField]
    private Collider _cube1Collider = default;

    [SerializeField]
    private Collider _cube2Collider = default;

    private void Update()
    {
        Debug.Log("<color=green>Cube1Å®Cube2:" + _cube1Collider.bounds.Intersects(_cube2Collider.bounds) + "</color>   <color=blue>Cube2Å®Cube1:" + _cube2Collider.bounds.Intersects(_cube1Collider.bounds) + "</color>");
    }
}

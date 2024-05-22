using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XScrollObject : MonoBehaviour, IBesideOnlyScrollable
{
    public void Scroll(Vector2 move)
    {
        this.transform.position += -new Vector3(move.x, 0f, 0f) / 1000f;
    }
}

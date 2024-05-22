using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YScrollObject : MonoBehaviour, IVerticalOnlyScrollable
{
    public void Scroll(Vector2 move)
    {
        this.transform.position += -new Vector3(0f, move.y, 0f) / 1000f;
    }
}

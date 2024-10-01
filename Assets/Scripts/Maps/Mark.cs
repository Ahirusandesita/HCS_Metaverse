using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mark : MonoBehaviour
{
    [SerializeField]
    RectTransform rectTransform;
    public void MarkViewPosition(Vector2 position)
    {
        rectTransform.localPosition = position;
    }
    public void MarkOutCamera()
    {

    }
}

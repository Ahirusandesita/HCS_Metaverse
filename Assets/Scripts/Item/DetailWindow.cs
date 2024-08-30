using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテム等の詳細表示を出現させるコンポーネント。
/// </summary>
public class DetailWindow : MonoBehaviour
{
    private GameObject canvasObject = default;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
#if UNITY_EDITOR

#endif
    }

    public void Display()
    {

    }

    public void Hide()
    {

    }
}

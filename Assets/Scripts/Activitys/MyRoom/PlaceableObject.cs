using UnityEngine;

/// <summary>
/// ワールドに配置可能なオブジェクト
/// </summary>
public class PlaceableObject : MonoBehaviour
{
    [Tooltip("rootを推奨")]
    [SerializeField] protected GameObject ghostOrigin = default;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        ghostOrigin = transform.root.gameObject;
    }

    public virtual void OnPlacingModeEnter() { }

    public virtual void OnPlacingModeExit() { }
}

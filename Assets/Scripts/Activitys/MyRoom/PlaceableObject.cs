using UnityEngine;

/// <summary>
/// ワールドに配置可能なオブジェクト
/// </summary>
public class PlaceableObject : MonoBehaviour
{
    [Tooltip("rootを推奨")]
    [SerializeField] protected GameObject ghostOrigin = default;

    public GameObject GhostOrigin => ghostOrigin;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    protected virtual void Reset()
    {
        ghostOrigin = transform.root.gameObject;
    }

    public virtual void OnPlacingModeEnter()
    {
        ghostOrigin.SetActive(false);
    }

    public virtual void OnPlacingModeExit()
    {
        ghostOrigin.SetActive(true);
    }
}

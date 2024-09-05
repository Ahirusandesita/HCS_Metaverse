using UnityEngine;

/// <summary>
/// ���[���h�ɔz�u�\�ȃI�u�W�F�N�g
/// </summary>
public class PlaceableObject : MonoBehaviour
{
    [Tooltip("root�𐄏�")]
    [SerializeField] protected GameObject ghostOrigin = default;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        ghostOrigin = transform.root.gameObject;
    }

    public virtual void OnPlacingModeEnter() { }

    public virtual void OnPlacingModeExit() { }
}

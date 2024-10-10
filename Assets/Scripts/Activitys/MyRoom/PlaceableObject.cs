using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���[���h�ɔz�u�\�ȃI�u�W�F�N�g
/// </summary>
public class PlaceableObject : MonoBehaviour
{
    [Tooltip("root�𐄏�")]
    [SerializeField] private GameObject ghostOrigin = default;
    [SerializeField] private GhostModel.PivotType pivotType = default;
    [SerializeField] private List<Collider> colliders = default;

    public GameObject GhostOrigin => ghostOrigin;
    public GhostModel.PivotType PivotType => pivotType;
    public IReadOnlyList<Collider> Colliders => colliders;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    protected virtual void Reset()
    {
        ghostOrigin = transform.root.gameObject;
    }

    //public virtual void OnPlacingModeEnter()
    //{
    //    ghostOrigin.SetActive(false);
    //}

    //public virtual void OnPlacingModeExit()
    //{
    //    ghostOrigin.SetActive(true);
    //}
}

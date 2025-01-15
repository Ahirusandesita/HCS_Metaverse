using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ワールドに配置可能なオブジェクト
/// </summary>
public class PlaceableObject : MonoBehaviour
{
    [Tooltip("rootを推奨")]
    [SerializeField] private GameObject ghostOrigin = default;
    [Tooltip("原点が中心にあるか足元（中心から下方向に伸びた点）にあるか")]
    [SerializeField] private GhostModel.PivotType pivotType = default;
    [Tooltip("配置可能場所")]
    [SerializeField] private GhostModel.PlacingStyle placingStyle = default;
    [SerializeField] private List<Collider> colliders = default;
    private int housingID = -1;

    public GameObject GhostOrigin => ghostOrigin;
    public GhostModel.PivotType PivotType => pivotType;
    public GhostModel.PlacingStyle PlacingStyle => placingStyle;
    public IReadOnlyList<Collider> Colliders => colliders;
    public int HousingID { get => housingID; set => housingID = value; }

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

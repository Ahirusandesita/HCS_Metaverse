using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���[���h�ɔz�u�\�ȃI�u�W�F�N�g
/// </summary>
public class PlaceableObject : MonoBehaviour
{
    [Tooltip("root�𐄏�")]
    [SerializeField] private GameObject ghostOrigin = default;
    [Tooltip("���_�����S�ɂ��邩�����i���S���牺�����ɐL�т��_�j�ɂ��邩")]
    [SerializeField] private GhostModel.PivotType pivotType = default;
    [Tooltip("�z�u�\�ꏊ")]
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

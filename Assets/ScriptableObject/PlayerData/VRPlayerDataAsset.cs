using UnityEngine;

/// <summary>
/// プレイヤーのパラメータクラス（VR）
/// </summary>
[CreateAssetMenu(fileName = "VRPlayerData", menuName = "ScriptableObjects/PlayerDataAsset/VR")]
public class VRPlayerDataAsset : PlayerDataAssetBase
{
    [Header("Camera")]
    [Tooltip("回転角[°]")]
    [SerializeField] private float rotateAngle = 30f;

    /// <summary>
    /// x軸の回転角[°]
    /// </summary>
    public float RotateAngle => rotateAngle;
}

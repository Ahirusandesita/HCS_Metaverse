using UnityEngine;

/// <summary>
/// プレイヤーのパラメータクラス（VR）
/// </summary>
[CreateAssetMenu(fileName = "VRPlayerData", menuName = "ScriptableObjects/PlayerDataAsset/VR")]
public class VRPlayerDataAsset : PlayerDataAssetBase
{
    [Header("Camera")]
    [Tooltip("x軸の回転角[°]")]
    [SerializeField] private float xRotateAngle = 15f;

    /// <summary>
    /// x軸の回転角[°]
    /// </summary>
    public float XRotateAngle => xRotateAngle;
}

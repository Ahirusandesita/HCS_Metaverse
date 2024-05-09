using UnityEngine;

/// <summary>
/// プレイヤーのパラメータクラス（PC)
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerDataAsset/Normal")]
public class PlayerDataAsset : PlayerDataAssetBase
{
    [Header("Camera")]
    [Tooltip("プレイヤーの回転速度")]
    [SerializeField] private float rotationSpeed = 1.0f;
    [Tooltip("カメラの上限角度")]
    [SerializeField] private float verticalMaxAngle = 90.0f;
    [Tooltip("カメラの下限角度")]
    [SerializeField] private float verticalMinAngle = -90.0f;

    /// <summary>
    /// プレイヤーの回転速度
    /// </summary>
    public float RotationSpeed => rotationSpeed;
    /// <summary>
    /// カメラの上限角度
    /// </summary>
    public float VerticalMaxAngle => verticalMaxAngle;
    /// <summary>
    /// カメラの下限角度
    /// </summary>
    public float VerticalMinAngle => verticalMinAngle;
}

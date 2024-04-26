using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerDataAsset")]
/// <summary>
/// プレイヤーのパラメータを扱うクラス
/// </summary>
public class PlayerDataAsset : ScriptableObject
{
    [Header("Player")]
    [Tooltip("プレイヤーの歩行速度[m/s]")]
    [SerializeField] private float walkSpeed = 4.0f;
    [Tooltip("プレイヤーのスプリント速度[m/s]")]
    [SerializeField] private float sprintSpeed = 6.0f;
    [Tooltip("プレイヤーの回転速度")]
    [SerializeField] private float rotationSpeed = 1.0f;
    [Tooltip("加減速レート（Lerp関数でTime.deltaTimeと掛け合わせて使用される）")]
    [SerializeField] private float speedChangeRate = 10.0f;

    [Space]
    [Tooltip("プレイヤーがジャンプ可能な高さ")]
    [SerializeField] private float jumpHeight = 1.2f;
    [Tooltip("プレイヤー独自の重力（UnityEngineの標準は-9.81f）")]
    [SerializeField] private float gravity = -15.0f;

    [Space]
    [Tooltip("再びジャンプできるようになるまでの時間（0fに設定すると、即座に再度ジャンプする）")]
    [SerializeField] private float jumpTimeout = 0.1f;

    [Header("Player Grounded")]
    [Tooltip("地面として使用しているレイヤー")]
    [SerializeField] private LayerMask groundLayers;

    [Header("Cinemachine")]
    [Tooltip("カメラの上限角度")]
    [SerializeField] private float verticalMaxAngle = 90.0f;
    [Tooltip("カメラの下限角度")]
    [SerializeField] private float verticalMinAngle = -90.0f;

    /// <summary>
    /// プレイヤーの歩行速度[m/s]
    /// </summary>
    public float WalkSpeed => walkSpeed;
    /// <summary>
    /// プレイヤーのスプリント速度[m/s]
    /// </summary>
    public float SprintSpeed => sprintSpeed;
    /// <summary>
    /// プレイヤーの回転速度
    /// </summary>
    public float RotationSpeed => rotationSpeed;
    /// <summary>
    /// 加減速レート（Lerp関数でTime.deltaTimeと掛け合わせて使用される）
    /// </summary>
    public float SpeedChangeRate => speedChangeRate;
    /// <summary>
    /// プレイヤーがジャンプ可能な高さ
    /// </summary>
    public float JumpHeight => jumpHeight;
    /// <summary>
    /// プレイヤー独自の重力（UnityEngineの標準は-9.81f）
    /// </summary>
    public float Gravity => gravity;
    /// <summary>
    /// 再びジャンプできるようになるまでの時間（0fに設定すると、即座に再度ジャンプする）
    /// </summary>
    public float JumpTimeout => jumpTimeout;
    /// <summary>
    /// 地面として使用しているレイヤー
    /// </summary>
    public LayerMask GroundLayers => groundLayers;
    /// <summary>
    /// カメラの上限角度
    /// </summary>
    public float VerticalMaxAngle => verticalMaxAngle;
    /// <summary>
    /// カメラの下限角度
    /// </summary>
    public float VerticalMinAngle => verticalMinAngle;
}

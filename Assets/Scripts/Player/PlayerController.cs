using System;
using UnityEngine;

/// <summary>
/// プレイヤーの挙動を扱うクラス（PC）
/// </summary>
public class PlayerController : PlayerControllerBase<PlayerDataAsset>
{
    [Tooltip("ChinemachineVirtualCameraが追従するターゲット（回転させるため、子オブジェクトのEmptyが望ましい）")]
    [SerializeField] private Transform cinemachineCameraTarget = default;

    [Tooltip("カメラの勾配（垂直方向の角度）")]
    private float cinemachineTargetPitch = default;

    protected override void Reset()
    {
        base.Reset();

        try
        {
            cinemachineCameraTarget ??= transform.Find("PlayerCameraRoot").transform;
        }
        catch (NullReferenceException) { }
    }

    /// <summary>
    /// CinemachineCameraおよびプレイヤーの回転処理
    /// </summary>
    protected override void CameraRotation()
    {
        // Inputがない場合、処理を終了
        if (lookDir == Vector2.zero)
        {
            return;
        }

        // マウスの入力にはTime.deltaTimeを掛けない
        float deltaTimeMultiplier = lastLookedDevice == DeviceType.Mouse
            ? 1.0f
            : Time.deltaTime;

        // y軸の入力量に応じて、カメラの勾配（垂直方向の角度）を加算する
        cinemachineTargetPitch += lookDir.y * playerDataAsset.RotationSpeed * deltaTimeMultiplier;
        // カメラの勾配をクランプする
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, playerDataAsset.VerticalMinAngle, playerDataAsset.VerticalMaxAngle);

        // x軸の入力方向への回転を取得
        float rotationVelocity = lookDir.x * playerDataAsset.RotationSpeed * deltaTimeMultiplier;

        // CinemachineCameraのtargetの回転を更新
        cinemachineCameraTarget.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);

        // プレイヤーを左右に回転させる
        myTransform.Rotate(Vector3.up * rotationVelocity);
    }
}
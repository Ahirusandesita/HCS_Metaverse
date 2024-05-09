using UniRx;
using UnityEngine;
using System;

/// <summary>
/// プレイヤーの挙動を扱うクラス（VR)
/// </summary>
public class VRPlayerController : PlayerControllerBase<VRPlayerDataAsset>
{
    [Tooltip("左右どちらに回転するか")]
    private FloatReactiveProperty lookDirX_RP = default;

    protected override void Awake()
    {
        base.Awake();

        // Subscribe
        lookDirX_RP = new FloatReactiveProperty().AddTo(this);
        lookDirX_RP
            // Filter: LookActionの左右いずれかが入力されたとき（Nuetralは弾く）
            .Where(value => value != 0f)
            // プレイヤーを回転させる
            .Subscribe(value => OnRotate(value));
    }

    protected override void Update()
    {
        base.Update();

#if UNITY_EDITOR
        // マウスでの入力は弾く
        if (inputter.LastLookedDevice == Inputter.DeviceType.Mouse)
        {
            return;
        }
#endif

        // Look（回転）の入力を[-1f, 0f, 1f]に加工し代入する
        lookDirX_RP.Value = inputter.LookDir.x == 0f
            ? 0f
            : Mathf.Sign(inputter.LookDir.x);
    }

    /// <summary>
    /// Look（回転）の入力があったとき、プレイヤーを回転させる
    /// </summary>
    /// <param name="leftOrRight">左右を表す符号（-1f, 1fのいずれか）</param>
    private void OnRotate(float leftOrRight)
    {
        myTransform.Rotate(Vector3.up * (15f * leftOrRight));
    }
}

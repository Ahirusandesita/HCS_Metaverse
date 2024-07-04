using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの挙動を扱うクラス（VR)
/// </summary>
public class VRPlayerController : PlayerControllerBase<VRPlayerDataAsset>, IDependencyInjector<PlayerHandDependencyInfomation>
{
    [SerializeField]
    private Transform centerEyeTransform = default;

    // Inspector拡張のAtrribute多め。お気になさらず！
    [Header("移動方式")]
    [SerializeField, CustomField("Move Type", CustomFieldAttribute.DisplayType.Replace)]
    private MoveTypeReactiveProperty moveTypeRP = default;

    [SerializeField, HideForMoveType(nameof(moveTypeEditor), VRMoveType.Natural)]
    private SpriteRenderer warpSymbol = default;

    [SerializeField, HideInInspector]
    private VRMoveType moveTypeEditor = default;

    [Tooltip("左右どちらに回転するか")]
    private FloatReactiveProperty lookDirX_RP = default;

    private WarpPointer warpPointer = default;
    private Transform leftHand = default;
    private Action<Vector3, Vector3, float> updateAction = default;


    protected override void Reset()
    {
        base.Reset();
        centerEyeTransform ??= transform.Find("CenterEyeAnchor").transform;
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void OnValidate()
    {
        moveTypeEditor = moveTypeRP.Value;
    }

    protected override void Awake()
    {
        base.Awake();

        followTransform = centerEyeTransform;
        warpPointer = new WarpPointer(gameObject);

        // Subscribe
        lookDirX_RP = new FloatReactiveProperty().AddTo(this);
        lookDirX_RP
            // Filter: LookActionの左右いずれかが入力されたとき（Nuetralは弾く）
            .Where(value => value != 0f)
            // プレイヤーを回転させる
            .Subscribe(value => OnRotate(value));

#if UNITY_EDITOR
        PlayerActions.Look.AddCompositeBinding("2DVector")
            .With("Left", "<Keyboard>/k")
            .With("Right", "<Keyboard>/semicolon");
        PlayerActions.Look.AddCompositeBinding("2DVector")
            .With("Left", "<Mouse>/leftButton")
            .With("Right", "<Mouse>/rightButton");
#endif

        moveTypeRP
            .AddTo(this)
            .Where(value => value == VRMoveType.Warp)
            .Subscribe(value =>
            {
                IsMovingRP.Subscribe(isMoving =>
                {
                    warpPointer.SetActive(isMoving);

                    if (isMoving)
                    {
                        // UI spawn
                        //warpSymbol.transform.position = myTransform.position;
                        //warpSymbol.enabled = true;
                    }
                    else
                    {
                        // UI despawn
                        //warpSymbol.enabled = false;
                    }
                })
                .AddTo(this);
            });
    }

    protected override void Update()
    {
        base.Update();

#if UNITY_EDITOR
        // マウスでの入力は弾く
        if (lastLookedDevice == DeviceType.Mouse)
        {
            return;
        }
#endif

        // Look（回転）の入力を[-1f, 0f, 1f]に加工し代入する
        lookDirX_RP.Value = lookDir.x == 0f
            ? 0f
            : Mathf.Sign(lookDir.x);
    }

    protected override void Move()
    {
        switch (moveTypeRP.Value)
        {
            case VRMoveType.Natural:
                base.Move();
                break;

            case VRMoveType.Warp:
                WarpMove();
                break;
        }
    }

    /// <summary>
    /// ワープ移動
    /// </summary>
    private void WarpMove()
    {
        if (!IsMovingRP.Value)
        {
            return;
        }

        warpPointer.Draw(leftHand.transform.position, leftHand.transform.forward);
    }

    /// <summary>
    /// Look（回転）の入力があったとき、プレイヤーを回転させる
    /// </summary>
    /// <param name="leftOrRight">左右を表す符号（-1f, 1fのいずれか）</param>
    private void OnRotate(float leftOrRight)
    {
        myTransform.Rotate(Vector3.up * (playerDataAsset.RotateAngle * leftOrRight));
    }

    void IDependencyInjector<PlayerHandDependencyInfomation>.Inject(PlayerHandDependencyInfomation information)
    {
        leftHand = information.LeftHand;
    }
}

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
    private WarpPointer warpPointer = default;

    [SerializeField, HideInInspector]
    private VRMoveType moveTypeEditor = default;

    [Tooltip("左右どちらに回転するか")]
    private FloatReactiveProperty lookDirX_RP = default;

    private Transform leftHand = default;

    private Vector3 warpPos = default;
    private IDisposable isMovingDisposable = default;


    protected override void Reset()
    {
        base.Reset();
        centerEyeTransform ??= transform.Find("CenterEyeAnchor").transform;
        warpPointer ??= GetComponentInChildren<WarpPointer>();
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

        // Subscribe
        lookDirX_RP = new FloatReactiveProperty().AddTo(this);
        lookDirX_RP
            // Filter: LookActionの左右いずれかが入力されたとき（Nuetralは弾く）
            .Where(value => value != 0f)
            // プレイヤーを回転させる
            .Subscribe(value => OnRotate(value));

        Inputter.Player.Warp.performed += _ =>
        {
            if (IsMovingRP.Value)
            {
                Warp();
            }
        };

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
            .Subscribe(value =>
            {
                // Naturalに変わったとき、購読が存在すればDispose
                if (value == VRMoveType.Natural)
                {
                    isMovingDisposable?.Dispose();
                }
                // Warpに変わったとき、IsMovingイベントを購読する
                else
                {
                    isMovingDisposable = IsMovingRP.Subscribe(isMoving =>
                    {
                        // 入力の有無によってビューの状態を切り替える
                        warpPointer.SetActive(isMoving);
                        //warpSymbol.enabled = isMoving;
                    })
                    .AddTo(this);
                }
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

                // ジャンプだけは有効化したいのでジャンプ処理を記述
                // () is order optimizated
                characterController.Move(Vector3.up * (verticalVelocity * Time.deltaTime));
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

        bool canWarp = warpPointer.Draw(leftHand.position, leftHand.forward, ref warpPos);
        //warpSymbol.transform.position = warpPos;

    }

    private void Warp()
    {
        myTransform.position = warpPos;
        //myTransform.rotation = 
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

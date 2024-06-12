using System;
using System.Diagnostics;
using UniRx;
using UnityEngine;

/// <summary>
/// プレイヤーの挙動を扱う基底クラス
/// </summary>
/// <typeparam name="TData">プレイヤーのパラメータクラス</typeparam>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InteractionScopeChecker))]
public abstract class PlayerControllerBase<TData> : MonoBehaviour where TData : PlayerDataAssetBase
{
    [SerializeField] protected CharacterController characterController = default;
    [SerializeField] protected TData playerDataAsset = default;
    [Tooltip("接地判定を行う球の原点となるターゲット")]
    [SerializeField] private Transform groundCheckSphere = default;
    [Tooltip("天井判定を行う球の原点となるターゲット")]
    [SerializeField] private Transform ceilingCheckSphere = default;

    protected Transform myTransform = default;
    protected Inputter inputter = default;

    [Tooltip("移動における追従先Transform")]
    protected Transform followTransform = default;
    [Tooltip("垂直方向の速度")]
    private float verticalVelocity = default;
    [Tooltip("ジャンプタイムアウト[s]（再ジャンプを許可するまでの時間）")]
    private float jumpTimeoutDelta = default;

    [Tooltip("接地しているかどうか")]
    private readonly ReactiveProperty<bool> isGroundedRP = new ReactiveProperty<bool>();
    [Tooltip("天井に当たっているかどうか")]
    private readonly ReactiveProperty<bool> isHitCeilingRP = new ReactiveProperty<bool>();

    [Tooltip("移動中かどうか")]
    protected readonly ReactiveProperty<bool> isMovingRP = new ReactiveProperty<bool>(false);
    [Tooltip("ジャンプ中かどうか")]
    protected readonly ReactiveProperty<bool> isJumpingRP = new ReactiveProperty<bool>(false);

    private const float TERMINAL_VELOCITY = 53f;
    private const float VERTICAL_VELOCITY_COEFFICIENT = -2f;

    /// <summary>
    /// 接地判定をする球の半径（CharacterControllerのRadiusを参照する）
    /// </summary>
    private float DecisionRadius => characterController.radius;

    public IReadOnlyReactiveProperty<bool> IsMovingRP => isMovingRP;
    public IReadOnlyReactiveProperty<bool> IsJumpingRP => isJumpingRP;


    protected virtual void Reset()
    {
        characterController ??= GetComponent<CharacterController>();
        try
        {
            groundCheckSphere ??= transform.Find("GroundChecker").transform;
        }
        catch (NullReferenceException) { }

        try
        {
            ceilingCheckSphere ??= transform.Find("CeilingChecker").transform;
        }
        catch (NullReferenceException) { }
    }

    protected virtual void Awake()
    {
        #region Cache
        myTransform = transform;
        inputter = new Inputter().AddTo(this);
        #endregion

        #region Subscribe
        isMovingRP.AddTo(this);
        isJumpingRP.AddTo(this);

        inputter.IsJumpInputRP
            // Filter: ジャンプ入力があったとき
            .Where(isJumpInput => isJumpInput)
            // 「ジャンプ中」をtrue
            .Subscribe(isJumpInput => isJumpingRP.Value = true)
            .AddTo(this);

        isGroundedRP
            .AddTo(this)
            // Filter: 着地したとき
            .Where(isGrounded => isGrounded)
            // 「ジャンプ中」をfalse
            .Subscribe(isGrounded => isJumpingRP.Value = false);

        isHitCeilingRP
            .AddTo(this)
            // Filter: 天井に当たったとき かつ 上向きの速度があるとき
            .Where(isHitCeiling => isHitCeiling && verticalVelocity > 0f)
            // 垂直方向の速度をリセット
            .Subscribe(isHitCeiling =>
            {
                verticalVelocity = VERTICAL_VELOCITY_COEFFICIENT;
                isJumpingRP.Value = false;
            });

        inputter.OnMoveStartedSubject.Subscribe(_ => isMovingRP.Value = true);
        inputter.OnMoveFinishedSubject.Subscribe(_ => isMovingRP.Value = false);
        #endregion
    }

    protected virtual void Start()
    {
        // Initialize
        jumpTimeoutDelta = playerDataAsset.JumpTimeout;

        if (followTransform is null)
        {
            followTransform = myTransform;
        }
    }

    protected virtual void Update()
    {
        JumpAndGravity();
        GroundedAndCeilingCheck();
        Move();
    }

    protected virtual void LateUpdate()
    {
        CameraRotation();
    }

    /// <summary>
    /// 接地および天井判定処理
    /// </summary>
    private void GroundedAndCeilingCheck()
    {
        isGroundedRP.Value = Physics.CheckSphere(groundCheckSphere.position, DecisionRadius, playerDataAsset.GroundLayers, QueryTriggerInteraction.Ignore);
        isHitCeilingRP.Value = Physics.CheckSphere(ceilingCheckSphere.position, DecisionRadius, playerDataAsset.GroundLayers, QueryTriggerInteraction.Ignore);
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    protected virtual void Move()
    {
        float speed;

        if (inputter.MoveDir == Vector2.zero)
        {
            // 移動の入力がなければ0を代入
            speed = 0f;
        }
        else
        {
            // 歩行速度またはスプリント速度を設定
            speed = inputter.IsSprintInputRP.Value
                ? playerDataAsset.SprintSpeed
                : playerDataAsset.WalkSpeed;
        }

        // 現在の水平方向の速度を取得
        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

        // 「設定した速度に近い値」を表現するためのOffset
        const float SPEED_OFFSET = 0.1f;

        // Inputのベクトルの大きさは1を超さない
        float inputMagnitude = inputter.MoveDir.magnitude <= 1f
            ? inputter.MoveDir.magnitude
            : 1f;

        // 設定した速度まで徐々に加減速を行う
        if (currentHorizontalSpeed < speed - SPEED_OFFSET || currentHorizontalSpeed > speed + SPEED_OFFSET)
        {
            // Note: tは渡された後にクランプされるので、こっちでクランプする必要はない
            speed = Mathf.Lerp(currentHorizontalSpeed, speed * inputMagnitude, Time.deltaTime * playerDataAsset.SpeedChangeRate);

            // 速度は小数点以下3桁に丸める
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }

        // 入力方向の正規化
        Vector3 inputDirection = new Vector3(inputter.MoveDir.x, 0.0f, inputter.MoveDir.y).normalized;

        // Inputがある場合、Inputから移動方向を合成
        // Note: Vector2の != 演算子は近似値を使用するため、浮動小数点エラーが発生しにくく、magnitudeよりも安価である。
        if (inputter.MoveDir != Vector2.zero)
        {
            inputDirection = followTransform.right * inputter.MoveDir.x + followTransform.forward * inputter.MoveDir.y;
        }

        // プレイヤーを移動させる
        characterController.Move(inputDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }

    /// <summary>
    /// 垂直方向の速度の計算処理
    /// </summary>
    protected virtual void JumpAndGravity()
    {
        if (isGroundedRP.Value)
        {
            // 速度が無限に低下するのを防ぐ
            if (verticalVelocity < 0f)
            {
                verticalVelocity = VERTICAL_VELOCITY_COEFFICIENT;
            }

            // ジャンプ入力があるかつ、ジャンプタイムアウトの時間外であれば、ジャンプをする
            if (inputter.IsJumpInputRP.Value && !isHitCeilingRP.Value && jumpTimeoutDelta <= 0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(playerDataAsset.JumpHeight * VERTICAL_VELOCITY_COEFFICIENT * playerDataAsset.Gravity);
            }
            // ジャンプタイムアウト中
            else if (jumpTimeoutDelta >= 0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // ジャンプタイムアウトをリセット
            jumpTimeoutDelta = playerDataAsset.JumpTimeout;

            // 空中にいるとき、ジャンプをリセット
            inputter.IsJumpInputRP.Value = false;
        }

        // 現在の速度が終端速度以下のとき、重力を加算
        if (verticalVelocity < TERMINAL_VELOCITY)
        {
            verticalVelocity += playerDataAsset.Gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// CinemachineCameraおよびプレイヤーの回転処理
    /// </summary>
    protected virtual void CameraRotation() { }

    /// <summary>
    /// 角度のクランプ処理
    /// </summary>
    /// <param name="lfAngle">対象角度</param>
    /// <param name="lfMin">最小角度</param>
    /// <param name="lfMax">最大角度</param>
    /// <returns>クランプ後の角度</returns>
    protected static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f)
        {
            lfAngle += 360f;
        }
        if (lfAngle > 360f)
        {
            lfAngle -= 360f;
        }

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    [Conditional("UNITY_EDITOR")]
    protected virtual void OnDrawGizmosSelected()
    {
        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        // エディタで動作するため、transformのキャッシュは使用しない
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Gizmos.DrawSphere(groundCheckSphere.position, DecisionRadius);
        Gizmos.DrawSphere(ceilingCheckSphere.position, DecisionRadius);
    }
}
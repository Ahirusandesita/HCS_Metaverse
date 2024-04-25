using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerDataAsset playerDataAsset = default;
    [Tooltip("ChinemachineVirtualCameraが追従するターゲット（回転させるため、子オブジェクトのEmptyが望ましい）")]
    [SerializeField] private Transform cinemachineCameraTarget = default;

    private Transform myTransform = default;
    private CharacterController characterController = default;
    private Inputter inputter = default;

    [Tooltip("接地しているかどうか")]
    private bool isGrounded = default;
    [Tooltip("垂直方向の速度")]
    private float verticalVelocity = default;
    [Tooltip("ジャンプタイムアウト[s]（再ジャンプを許可するまでの時間）")]
    private float jumpTimeoutDelta = default;
    [Tooltip("カメラの勾配（垂直方向の角度）")]
    private float cinemachineTargetPitch = default;

    private const float TERMINAL_VELOCITY = 53.0f;

    /// <summary>
    /// 接地判定をする球の半径（CharacterControllerのRadiusを参照する）
    /// </summary>
    private float GroundedRadius => characterController.radius;


    private void Awake()
    {
        // キャッシュ
        myTransform = transform;
        characterController = GetComponent<CharacterController>();
        inputter = new Inputter();
    }

    private void Start()
    {
        // 初期化
        jumpTimeoutDelta = playerDataAsset.JumpTimeout;
    }

    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void OnDestroy()
    {
        inputter.Dispose();
    }

    /// <summary>
    /// 接地判定処理
    /// </summary>
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(myTransform.position.x, myTransform.position.y - playerDataAsset.GroundedOffset, myTransform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, playerDataAsset.GroundLayers, QueryTriggerInteraction.Ignore);
    }

    /// <summary>
    /// CinemachineCameraおよびプレイヤーの回転処理
    /// </summary>
    private void CameraRotation()
    {
        // Inputがない場合、処理を終了
        if (inputter.LookDir == Vector2.zero)
        {
            return;
        }

        // マウスの入力にはTime.deltaTimeを掛けない
        float deltaTimeMultiplier = inputter.LastLookedDevice == Inputter.DeviceType.KeyboardMouse
            ? 1.0f
            : Time.deltaTime;

        // y軸の入力量に応じて、カメラの勾配（垂直方向の角度）を加算する
        cinemachineTargetPitch += inputter.LookDir.y * playerDataAsset.RotationSpeed * deltaTimeMultiplier;
        // カメラの勾配をクランプする
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, playerDataAsset.VerticalMinAngle, playerDataAsset.VerticalMaxAngle);

        // x軸の入力方向への回転を取得
        float rotationVelocity = inputter.LookDir.x * playerDataAsset.RotationSpeed * deltaTimeMultiplier;

        // CinemachineCameraのtargetの回転を更新
        cinemachineCameraTarget.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);

        // プレイヤーを左右に回転させる
        myTransform.Rotate(Vector3.up * rotationVelocity);
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
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
            speed = inputter.IsSprint
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
            // note: tは渡された後にクランプされるので、こっちでクランプする必要はない
            speed = Mathf.Lerp(currentHorizontalSpeed, speed * inputMagnitude, Time.deltaTime * playerDataAsset.SpeedChangeRate);

            // 速度は小数点以下3桁に丸める
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }

        // 入力方向の正規化
        Vector3 inputDirection = new Vector3(inputter.MoveDir.x, 0.0f, inputter.MoveDir.y).normalized;

        // Inputがある場合、Inputから移動方向を合成
        // note: Vector2の != 演算子は近似値を使用するため、浮動小数点エラーが発生しにくく、magnitudeよりも安価である。
        if (inputter.MoveDir != Vector2.zero)
        {
            inputDirection = myTransform.right * inputter.MoveDir.x + myTransform.forward * inputter.MoveDir.y;
        }

        // プレイヤーを移動させる
        characterController.Move(inputDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }

    /// <summary>
    /// 垂直方向の速度の計算処理
    /// </summary>
    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            // 速度が無限に低下するのを防ぐ
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            // ジャンプ入力があるかつ、ジャンプタイムアウトの時間外であれば、ジャンプをする
            if (inputter.IsJump && jumpTimeoutDelta <= 0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(playerDataAsset.JumpHeight * -2f * playerDataAsset.Gravity);
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
            inputter.IsJump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (verticalVelocity < TERMINAL_VELOCITY)
        {
            verticalVelocity += playerDataAsset.Gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// 角度のクランプ処理
    /// </summary>
    /// <param name="lfAngle">対象角度</param>
    /// <param name="lfMin">最小角度</param>
    /// <param name="lfMax">最大角度</param>
    /// <returns>クランプ後の角度</returns>
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
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

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(myTransform.position.x, myTransform.position.y - playerDataAsset.GroundedOffset, myTransform.position.z), GroundedRadius);
    }
}

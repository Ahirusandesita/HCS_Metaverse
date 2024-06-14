using System;
using UniRx;
using UnityEngine;

/// <summary>
/// �v���C���[�̋������������N���X
/// </summary>
/// <typeparam name="TData">�v���C���[�̃p�����[�^�N���X</typeparam>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InteractionScopeChecker))]
public abstract class PlayerControllerBase<TData> : MonoBehaviour where TData : PlayerDataAssetBase
{
    [SerializeField] protected CharacterController characterController = default;
    [SerializeField] protected TData playerDataAsset = default;
    [Tooltip("�ڒn������s�����̌��_�ƂȂ�^�[�Q�b�g")]
    [SerializeField] private Transform groundCheckSphere = default;
    [Tooltip("�V�䔻����s�����̌��_�ƂȂ�^�[�Q�b�g")]
    [SerializeField] private Transform ceilingCheckSphere = default;

    protected Transform myTransform = default;

    [Tooltip("�ړ�����")]
    protected Vector2 moveDir = default;
    [Tooltip("�]�����")]
    protected Vector2 lookDir = default;
    [Tooltip("�X�v�����g���͂����邩�ǂ���")]
    protected bool isSprintInput = default;
    [Tooltip("�W�����v���͂����邩�ǂ���")]
    protected bool isJumpInput = default;
    [Tooltip("�Ō��LookAction�𑀍삵���f�o�C�X")]
    protected DeviceType lastLookedDevice = default;
    [Tooltip("�ړ��ɂ�����Ǐ]��Transform")]
    protected Transform followTransform = default;
    [Tooltip("���������̑��x")]
    private float verticalVelocity = default;
    [Tooltip("�W�����v�^�C���A�E�g[s]�i�ăW�����v��������܂ł̎��ԁj")]
    private float jumpTimeoutDelta = default;

    [Tooltip("�ڒn���Ă��邩�ǂ���")]
    private readonly ReactiveProperty<bool> isGroundedRP = new ReactiveProperty<bool>();
    [Tooltip("�V��ɓ������Ă��邩�ǂ���")]
    private readonly ReactiveProperty<bool> isHitCeilingRP = new ReactiveProperty<bool>();

    [Tooltip("�ړ������ǂ���")]
    protected readonly ReactiveProperty<bool> isMovingRP = new ReactiveProperty<bool>(false);
    [Tooltip("�W�����v�����ǂ���")]
    protected readonly ReactiveProperty<bool> isJumpingRP = new ReactiveProperty<bool>(false);

    private const float TERMINAL_VELOCITY = 53f;
    private const float VERTICAL_VELOCITY_COEFFICIENT = -2f;

    /// <summary>
    /// �ڒn��������鋅�̔��a�iCharacterController��Radius���Q�Ƃ���j
    /// </summary>
    private float DecisionRadius => characterController.radius;
    protected PlayerInputActions.PlayerActions PlayerActions => Inputter.Player;

    public IReadOnlyReactiveProperty<bool> IsMovingRP => isMovingRP;
    public IReadOnlyReactiveProperty<bool> IsJumpingRP => isJumpingRP;


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
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
        myTransform = transform;

        #region Subscribe
        isMovingRP.AddTo(this);
        isJumpingRP.AddTo(this);

        PlayerActions.Enable();
        // �W�����v���͂��w��
        PlayerActions.Jump.performed += _ =>
        {
            isJumpInput = true;
            isJumpingRP.Value = true;
        };
        PlayerActions.Jump.canceled += _ =>
        {
            isJumpInput = false;
        };
        // �ړ����͂��w��
        PlayerActions.Move.started += _ =>
        {
            isMovingRP.Value = true;
        };
        PlayerActions.Move.performed += context =>
        {
            moveDir = context.ReadValue<Vector2>();
        };
        PlayerActions.Move.canceled += _ =>
        {
            moveDir = Vector2.zero;
            isMovingRP.Value = false;
        };
        // �X�v�����g���͂��w��
        PlayerActions.Sprint.performed += _ =>
        {
            isSprintInput = true;
        };
        PlayerActions.Sprint.canceled += _ =>
        {
            isSprintInput = false;
        };
        PlayerActions.Look.performed += context =>
        {
            lookDir = context.ReadValue<Vector2>();

            // ���͂��ꂽ�f�o�C�X�𔻒肷��
            // �{��InputSystem�͓��͎҂��B�����A���͎҂ɂ���ď����̕���͑z�肵�Ă��Ȃ����A
            // �v���C���[�̏����ŕ��򂪕K�v�ɂȂ邽�߁A�������������B
            lastLookedDevice = context.control.layout switch
            {
                "Delta" => DeviceType.Mouse,
                "Stick" => DeviceType.GamepadOrXR,
#if UNITY_EDITOR
                "Key" => DeviceType.Debug,
                "Button" => DeviceType.Debug,
                _ => throw new DeviceException("[����FLook]��Mouse, Keyboard, Gamepad, XR�ȊO�̃f�o�C�X������͂���܂����B"),
#else
                _ => DeviceType.GamepadOrXR,
#endif
            };
        };

        // ���n�����Ƃ����w��
        isGroundedRP
            .AddTo(this)
            .Where(isGrounded => isGrounded)
            // �u�W�����v���v��false
            .Subscribe(isGrounded =>
            {
                isJumpingRP.Value = false;
            });

        // �V��ɓ��������Ƃ����w��
        isHitCeilingRP
            .AddTo(this)
            .Where(isHitCeiling => isHitCeiling && verticalVelocity > 0f)
            // ���������̑��x�����Z�b�g
            .Subscribe(isHitCeiling =>
            {
                verticalVelocity = VERTICAL_VELOCITY_COEFFICIENT;
                isJumpingRP.Value = false;
            });
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

    public void Enable()
    {
        PlayerActions.Enable();
    }

    public void Disable()
    {
        PlayerActions.Disable();
    }

    /// <summary>
    /// �ڒn����ѓV�䔻�菈��
    /// </summary>
    private void GroundedAndCeilingCheck()
    {
        isGroundedRP.Value = Physics.CheckSphere(groundCheckSphere.position, DecisionRadius, playerDataAsset.GroundLayers, QueryTriggerInteraction.Ignore);
        isHitCeilingRP.Value = Physics.CheckSphere(ceilingCheckSphere.position, DecisionRadius, playerDataAsset.GroundLayers, QueryTriggerInteraction.Ignore);
    }

    /// <summary>
    /// �ړ�����
    /// </summary>
    protected virtual void Move()
    {
        float speed;

        if (moveDir == Vector2.zero)
        {
            // �ړ��̓��͂��Ȃ����0����
            speed = 0f;
        }
        else
        {
            // ���s���x�܂��̓X�v�����g���x��ݒ�
            speed = isSprintInput
                ? playerDataAsset.SprintSpeed
                : playerDataAsset.WalkSpeed;
        }

        // ���݂̐��������̑��x���擾
        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

        // �u�ݒ肵�����x�ɋ߂��l�v��\�����邽�߂�Offset
        const float SPEED_OFFSET = 0.1f;

        // Input�̃x�N�g���̑傫����1�𒴂��Ȃ�
        float inputMagnitude = moveDir.magnitude <= 1f
            ? moveDir.magnitude
            : 1f;

        // �ݒ肵�����x�܂ŏ��X�ɉ��������s��
        if (currentHorizontalSpeed < speed - SPEED_OFFSET || currentHorizontalSpeed > speed + SPEED_OFFSET)
        {
            // Note: t�͓n���ꂽ��ɃN�����v�����̂ŁA�������ŃN�����v����K�v�͂Ȃ�
            speed = Mathf.Lerp(currentHorizontalSpeed, speed * inputMagnitude, Time.deltaTime * playerDataAsset.SpeedChangeRate);

            // ���x�͏����_�ȉ�3���Ɋۂ߂�
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }

        // ���͕����̐��K��
        Vector3 inputDirection = new Vector3(moveDir.x, 0.0f, moveDir.y).normalized;

        // Input������ꍇ�AInput����ړ�����������
        // Note: Vector2�� != ���Z�q�͋ߎ��l���g�p���邽�߁A���������_�G���[���������ɂ����Amagnitude���������ł���B
        if (moveDir != Vector2.zero)
        {
            inputDirection = followTransform.right * moveDir.x + followTransform.forward * moveDir.y;
        }

        // �v���C���[���ړ�������
        characterController.Move(inputDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }

    /// <summary>
    /// ���������̑��x�̌v�Z����
    /// </summary>
    protected virtual void JumpAndGravity()
    {
        if (isGroundedRP.Value)
        {
            // ���x�������ɒቺ����̂�h��
            if (verticalVelocity < 0f)
            {
                verticalVelocity = VERTICAL_VELOCITY_COEFFICIENT;
            }

            // �W�����v���͂����邩�A�W�����v�^�C���A�E�g�̎��ԊO�ł���΁A�W�����v������
            if (isJumpInput && !isHitCeilingRP.Value && jumpTimeoutDelta <= 0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(playerDataAsset.JumpHeight * VERTICAL_VELOCITY_COEFFICIENT * playerDataAsset.Gravity);
            }
            // �W�����v�^�C���A�E�g��
            else if (jumpTimeoutDelta >= 0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // �W�����v�^�C���A�E�g�����Z�b�g
            jumpTimeoutDelta = playerDataAsset.JumpTimeout;

            // �󒆂ɂ���Ƃ��A�W�����v�����Z�b�g
            isJumpInput = false;
        }

        // ���݂̑��x���I�[���x�ȉ��̂Ƃ��A�d�͂����Z
        if (verticalVelocity < TERMINAL_VELOCITY)
        {
            verticalVelocity += playerDataAsset.Gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// CinemachineCamera����уv���C���[�̉�]����
    /// </summary>
    protected virtual void CameraRotation() { }

    /// <summary>
    /// �p�x�̃N�����v����
    /// </summary>
    /// <param name="lfAngle">�Ώۊp�x</param>
    /// <param name="lfMin">�ŏ��p�x</param>
    /// <param name="lfMax">�ő�p�x</param>
    /// <returns>�N�����v��̊p�x</returns>
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

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    protected virtual void OnDrawGizmosSelected()
    {
        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        // �G�f�B�^�œ��삷�邽�߁Atransform�̃L���b�V���͎g�p���Ȃ�
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Gizmos.DrawSphere(groundCheckSphere.position, DecisionRadius);
        Gizmos.DrawSphere(ceilingCheckSphere.position, DecisionRadius);
    }
}
public enum DeviceType
{
    Mouse,
    GamepadOrXR,
#if UNITY_EDITOR
    /// <summary>
    /// Editor only
    /// </summary>
    Debug,
#endif
}

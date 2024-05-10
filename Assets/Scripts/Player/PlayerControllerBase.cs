using System.Diagnostics;
using UniRx;
using UnityEngine;

/// <summary>
/// �v���C���[�̋������������N���X
/// </summary>
/// <typeparam name="TData">�v���C���[�̃p�����[�^�N���X</typeparam>
[RequireComponent(typeof(CharacterController))]
public abstract class PlayerControllerBase<TData> : MonoBehaviour where TData : PlayerDataAssetBase
{
    [SerializeField] protected CharacterController characterController = default;
    [SerializeField] protected TData playerDataAsset = default;
    [Tooltip("�ڒn������s�����̌��_�ƂȂ�^�[�Q�b�g")]
    [SerializeField] private Transform groundCheckSphere = default;
    [Tooltip("�V�䔻����s�����̌��_�ƂȂ�^�[�Q�b�g")]
    [SerializeField] private Transform ceilingCheckSphere = default;

    protected Transform myTransform = default;
    protected Inputter inputter = default;

    [Tooltip("�ړ��ɂ�����Ǐ]��Transform")]
    protected Transform followTransform = default;
    [Tooltip("�ڒn���Ă��邩�ǂ���")]
    private bool isGrounded = default;
    [Tooltip("���������̑��x")]
    private float verticalVelocity = default;
    [Tooltip("�W�����v�^�C���A�E�g[s]�i�ăW�����v��������܂ł̎��ԁj")]
    private float jumpTimeoutDelta = default;

    [Tooltip("�V��ɓ������Ă��邩�ǂ���")]
    private readonly BoolReactiveProperty isHitCeilingRP = new BoolReactiveProperty();

    private const float TERMINAL_VELOCITY = 53f;
    private const float VERTICAL_VELOCITY_COEFFICIENT = -2f;

    /// <summary>
    /// �ڒn��������鋅�̔��a�iCharacterController��Radius���Q�Ƃ���j
    /// </summary>
    private float DecisionRadius => characterController.radius;


    protected virtual void Awake()
    {
        // Cache
        myTransform = transform;
        inputter = new Inputter().AddTo(this);

        // Subscribe
        isHitCeilingRP
            // Filter: �l��true�ɕς�����Ƃ� ���� ������̑��x������Ƃ�
            .Where(isHitCeiling => isHitCeiling && verticalVelocity > 0f)
            // ���������̑��x�����Z�b�g
            .Subscribe(isHitCeiling => verticalVelocity = VERTICAL_VELOCITY_COEFFICIENT)
            .AddTo(this);
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
    /// �ڒn����ѓV�䔻�菈��
    /// </summary>
    private void GroundedAndCeilingCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckSphere.position, DecisionRadius, playerDataAsset.GroundLayers, QueryTriggerInteraction.Ignore);
        isHitCeilingRP.Value = Physics.CheckSphere(ceilingCheckSphere.position, DecisionRadius, playerDataAsset.GroundLayers, QueryTriggerInteraction.Ignore);
    }

    /// <summary>
    /// �ړ�����
    /// </summary>
    protected virtual void Move()
    {
        float speed;

        if (inputter.MoveDir == Vector2.zero)
        {
            // �ړ��̓��͂��Ȃ����0����
            speed = 0f;
        }
        else
        {
            // ���s���x�܂��̓X�v�����g���x��ݒ�
            speed = inputter.IsSprint
                ? playerDataAsset.SprintSpeed
                : playerDataAsset.WalkSpeed;
        }

        // ���݂̐��������̑��x���擾
        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

        // �u�ݒ肵�����x�ɋ߂��l�v��\�����邽�߂�Offset
        const float SPEED_OFFSET = 0.1f;

        // Input�̃x�N�g���̑傫����1�𒴂��Ȃ�
        float inputMagnitude = inputter.MoveDir.magnitude <= 1f
            ? inputter.MoveDir.magnitude
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
        Vector3 inputDirection = new Vector3(inputter.MoveDir.x, 0.0f, inputter.MoveDir.y).normalized;

        // Input������ꍇ�AInput����ړ�����������
        // Note: Vector2�� != ���Z�q�͋ߎ��l���g�p���邽�߁A���������_�G���[���������ɂ����Amagnitude���������ł���B
        if (inputter.MoveDir != Vector2.zero)
        {
            inputDirection = followTransform.right * inputter.MoveDir.x + followTransform.forward * inputter.MoveDir.y;
        }

        // �v���C���[���ړ�������
        characterController.Move(inputDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }

    /// <summary>
    /// ���������̑��x�̌v�Z����
    /// </summary>
    protected virtual void JumpAndGravity()
    {
        if (isGrounded)
        {
            // ���x�������ɒቺ����̂�h��
            if (verticalVelocity < 0f)
            {
                verticalVelocity = VERTICAL_VELOCITY_COEFFICIENT;
            }

            // �W�����v���͂����邩�A�W�����v�^�C���A�E�g�̎��ԊO�ł���΁A�W�����v������
            if (inputter.IsJump && !isHitCeilingRP.Value && jumpTimeoutDelta <= 0f)
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
            inputter.IsJump = false;
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

    [Conditional("UNITY_EDITOR")]
    protected virtual void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Color transparentBlue = new Color(0.0f, 0.0f, 1.0f, 0.35f);
        Color transparentYellow = new Color(1.0f, 1.0f, 0.0f, 0.35f);

        if (isGrounded && isHitCeilingRP.Value) Gizmos.color = transparentBlue;
        else if (isGrounded && !isHitCeilingRP.Value) Gizmos.color = transparentGreen;
        else if (!isGrounded && isHitCeilingRP.Value) Gizmos.color = transparentYellow;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        // �G�f�B�^�œ��삷�邽�߁Atransform�̃L���b�V���͎g�p���Ȃ�
        Gizmos.DrawSphere(groundCheckSphere.position, DecisionRadius);
        Gizmos.DrawSphere(ceilingCheckSphere.position, DecisionRadius);
    }
}

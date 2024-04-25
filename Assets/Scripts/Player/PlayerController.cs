using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerDataAsset playerDataAsset = default;
    [Tooltip("ChinemachineVirtualCamera���Ǐ]����^�[�Q�b�g�i��]�����邽�߁A�q�I�u�W�F�N�g��Empty���]�܂����j")]
    [SerializeField] private Transform cinemachineCameraTarget = default;

    private Transform myTransform = default;
    private CharacterController characterController = default;
    private Inputter inputter = default;

    [Tooltip("�ڒn���Ă��邩�ǂ���")]
    private bool isGrounded = default;
    [Tooltip("���������̑��x")]
    private float verticalVelocity = default;
    [Tooltip("�W�����v�^�C���A�E�g[s]�i�ăW�����v��������܂ł̎��ԁj")]
    private float jumpTimeoutDelta = default;
    [Tooltip("�J�����̌��z�i���������̊p�x�j")]
    private float cinemachineTargetPitch = default;

    private const float TERMINAL_VELOCITY = 53.0f;

    /// <summary>
    /// �ڒn��������鋅�̔��a�iCharacterController��Radius���Q�Ƃ���j
    /// </summary>
    private float GroundedRadius => characterController.radius;


    private void Awake()
    {
        // �L���b�V��
        myTransform = transform;
        characterController = GetComponent<CharacterController>();
        inputter = new Inputter();
    }

    private void Start()
    {
        // ������
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
    /// �ڒn���菈��
    /// </summary>
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(myTransform.position.x, myTransform.position.y - playerDataAsset.GroundedOffset, myTransform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, playerDataAsset.GroundLayers, QueryTriggerInteraction.Ignore);
    }

    /// <summary>
    /// CinemachineCamera����уv���C���[�̉�]����
    /// </summary>
    private void CameraRotation()
    {
        // Input���Ȃ��ꍇ�A�������I��
        if (inputter.LookDir == Vector2.zero)
        {
            return;
        }

        // �}�E�X�̓��͂ɂ�Time.deltaTime���|���Ȃ�
        float deltaTimeMultiplier = inputter.LastLookedDevice == Inputter.DeviceType.KeyboardMouse
            ? 1.0f
            : Time.deltaTime;

        // y���̓��͗ʂɉ����āA�J�����̌��z�i���������̊p�x�j�����Z����
        cinemachineTargetPitch += inputter.LookDir.y * playerDataAsset.RotationSpeed * deltaTimeMultiplier;
        // �J�����̌��z���N�����v����
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, playerDataAsset.VerticalMinAngle, playerDataAsset.VerticalMaxAngle);

        // x���̓��͕����ւ̉�]���擾
        float rotationVelocity = inputter.LookDir.x * playerDataAsset.RotationSpeed * deltaTimeMultiplier;

        // CinemachineCamera��target�̉�]���X�V
        cinemachineCameraTarget.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);

        // �v���C���[�����E�ɉ�]������
        myTransform.Rotate(Vector3.up * rotationVelocity);
    }

    /// <summary>
    /// �ړ�����
    /// </summary>
    private void Move()
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
            // note: t�͓n���ꂽ��ɃN�����v�����̂ŁA�������ŃN�����v����K�v�͂Ȃ�
            speed = Mathf.Lerp(currentHorizontalSpeed, speed * inputMagnitude, Time.deltaTime * playerDataAsset.SpeedChangeRate);

            // ���x�͏����_�ȉ�3���Ɋۂ߂�
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }

        // ���͕����̐��K��
        Vector3 inputDirection = new Vector3(inputter.MoveDir.x, 0.0f, inputter.MoveDir.y).normalized;

        // Input������ꍇ�AInput����ړ�����������
        // note: Vector2�� != ���Z�q�͋ߎ��l���g�p���邽�߁A���������_�G���[���������ɂ����Amagnitude���������ł���B
        if (inputter.MoveDir != Vector2.zero)
        {
            inputDirection = myTransform.right * inputter.MoveDir.x + myTransform.forward * inputter.MoveDir.y;
        }

        // �v���C���[���ړ�������
        characterController.Move(inputDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }

    /// <summary>
    /// ���������̑��x�̌v�Z����
    /// </summary>
    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            // ���x�������ɒቺ����̂�h��
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            // �W�����v���͂����邩�A�W�����v�^�C���A�E�g�̎��ԊO�ł���΁A�W�����v������
            if (inputter.IsJump && jumpTimeoutDelta <= 0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(playerDataAsset.JumpHeight * -2f * playerDataAsset.Gravity);
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

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (verticalVelocity < TERMINAL_VELOCITY)
        {
            verticalVelocity += playerDataAsset.Gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// �p�x�̃N�����v����
    /// </summary>
    /// <param name="lfAngle">�Ώۊp�x</param>
    /// <param name="lfMin">�ŏ��p�x</param>
    /// <param name="lfMax">�ő�p�x</param>
    /// <returns>�N�����v��̊p�x</returns>
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

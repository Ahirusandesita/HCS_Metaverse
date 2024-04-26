using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerDataAsset")]
/// <summary>
/// �v���C���[�̃p�����[�^�������N���X
/// </summary>
public class PlayerDataAsset : ScriptableObject
{
    [Header("Player")]
    [Tooltip("�v���C���[�̕��s���x[m/s]")]
    [SerializeField] private float walkSpeed = 4.0f;
    [Tooltip("�v���C���[�̃X�v�����g���x[m/s]")]
    [SerializeField] private float sprintSpeed = 6.0f;
    [Tooltip("�v���C���[�̉�]���x")]
    [SerializeField] private float rotationSpeed = 1.0f;
    [Tooltip("���������[�g�iLerp�֐���Time.deltaTime�Ɗ|�����킹�Ďg�p�����j")]
    [SerializeField] private float speedChangeRate = 10.0f;

    [Space]
    [Tooltip("�v���C���[���W�����v�\�ȍ���")]
    [SerializeField] private float jumpHeight = 1.2f;
    [Tooltip("�v���C���[�Ǝ��̏d�́iUnityEngine�̕W����-9.81f�j")]
    [SerializeField] private float gravity = -15.0f;

    [Space]
    [Tooltip("�ĂуW�����v�ł���悤�ɂȂ�܂ł̎��ԁi0f�ɐݒ肷��ƁA�����ɍēx�W�����v����j")]
    [SerializeField] private float jumpTimeout = 0.1f;

    [Header("Player Grounded")]
    [Tooltip("�n�ʂƂ��Ďg�p���Ă��郌�C���[")]
    [SerializeField] private LayerMask groundLayers;

    [Header("Cinemachine")]
    [Tooltip("�J�����̏���p�x")]
    [SerializeField] private float verticalMaxAngle = 90.0f;
    [Tooltip("�J�����̉����p�x")]
    [SerializeField] private float verticalMinAngle = -90.0f;

    /// <summary>
    /// �v���C���[�̕��s���x[m/s]
    /// </summary>
    public float WalkSpeed => walkSpeed;
    /// <summary>
    /// �v���C���[�̃X�v�����g���x[m/s]
    /// </summary>
    public float SprintSpeed => sprintSpeed;
    /// <summary>
    /// �v���C���[�̉�]���x
    /// </summary>
    public float RotationSpeed => rotationSpeed;
    /// <summary>
    /// ���������[�g�iLerp�֐���Time.deltaTime�Ɗ|�����킹�Ďg�p�����j
    /// </summary>
    public float SpeedChangeRate => speedChangeRate;
    /// <summary>
    /// �v���C���[���W�����v�\�ȍ���
    /// </summary>
    public float JumpHeight => jumpHeight;
    /// <summary>
    /// �v���C���[�Ǝ��̏d�́iUnityEngine�̕W����-9.81f�j
    /// </summary>
    public float Gravity => gravity;
    /// <summary>
    /// �ĂуW�����v�ł���悤�ɂȂ�܂ł̎��ԁi0f�ɐݒ肷��ƁA�����ɍēx�W�����v����j
    /// </summary>
    public float JumpTimeout => jumpTimeout;
    /// <summary>
    /// �n�ʂƂ��Ďg�p���Ă��郌�C���[
    /// </summary>
    public LayerMask GroundLayers => groundLayers;
    /// <summary>
    /// �J�����̏���p�x
    /// </summary>
    public float VerticalMaxAngle => verticalMaxAngle;
    /// <summary>
    /// �J�����̉����p�x
    /// </summary>
    public float VerticalMinAngle => verticalMinAngle;
}

using System;
using UnityEngine;

/// <summary>
/// �v���C���[�̋����������N���X�iPC�j
/// </summary>
public class PlayerController : PlayerControllerBase<PlayerDataAsset>
{
    [Tooltip("ChinemachineVirtualCamera���Ǐ]����^�[�Q�b�g�i��]�����邽�߁A�q�I�u�W�F�N�g��Empty���]�܂����j")]
    [SerializeField] private Transform cinemachineCameraTarget = default;

    [Tooltip("�J�����̌��z�i���������̊p�x�j")]
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
    /// CinemachineCamera����уv���C���[�̉�]����
    /// </summary>
    protected override void CameraRotation()
    {
        // Input���Ȃ��ꍇ�A�������I��
        if (lookDir == Vector2.zero)
        {
            return;
        }

        // �}�E�X�̓��͂ɂ�Time.deltaTime���|���Ȃ�
        float deltaTimeMultiplier = lastLookedDevice == DeviceType.Mouse
            ? 1.0f
            : Time.deltaTime;

        // y���̓��͗ʂɉ����āA�J�����̌��z�i���������̊p�x�j�����Z����
        cinemachineTargetPitch += lookDir.y * playerDataAsset.RotationSpeed * deltaTimeMultiplier;
        // �J�����̌��z���N�����v����
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, playerDataAsset.VerticalMinAngle, playerDataAsset.VerticalMaxAngle);

        // x���̓��͕����ւ̉�]���擾
        float rotationVelocity = lookDir.x * playerDataAsset.RotationSpeed * deltaTimeMultiplier;

        // CinemachineCamera��target�̉�]���X�V
        cinemachineCameraTarget.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);

        // �v���C���[�����E�ɉ�]������
        myTransform.Rotate(Vector3.up * rotationVelocity);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandIK : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Transform _handTarget;  // ��̃^�[�Q�b�g�ʒu (VR�R���g���[���[�Ȃ�)

    [SerializeField]
    private Transform _shoulderTransform;  // ����Transform

    [SerializeField]
    private Transform _bodyTransform;  // �̂�Transform (Spine)

    void OnAnimatorIK(int layerIndex)
    {
        if (_animator)
        {
            // �����IK�S�[���̃E�F�C�g�ƈʒu�E��]��ݒ�
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _handTarget.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, _handTarget.rotation);

            // ���I�̃q���g�ʒu���v�Z���Đݒ�
            Vector3 elbowHintPosition = CalculateElbowHintPosition();
            _animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1.0f);
            _animator.SetIKHintPosition(AvatarIKHint.LeftElbow, elbowHintPosition);
        }
    }

    // �I�̎��R�Ȉʒu���v�Z����֐�
    Vector3 CalculateElbowHintPosition()
    {
        Vector3 shoulderPosition = _shoulderTransform.position;
        Vector3 handPosition = _handTarget.position;

        // �肩�猨�ւ̃x�N�g�����擾
        Vector3 shoulderToHand = handPosition - shoulderPosition;

        // �肩�猨�ւ̃x�N�g���ɐ����ȕ����ɕI���I�t�Z�b�g
        Vector3 perpendicularOffset = Vector3.Cross(shoulderToHand, _bodyTransform.up).normalized;

        // �I�̈ʒu�����Ǝ�̒��Ԃɔz�u���A�I�t�Z�b�g��������
        return shoulderPosition + shoulderToHand * 0.5f + perpendicularOffset * 0.2f;  // �������K�v�ȏꍇ�A�I�t�Z�b�g�l��ύX
    }
}
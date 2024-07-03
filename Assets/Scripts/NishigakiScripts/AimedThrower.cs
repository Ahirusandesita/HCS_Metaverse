using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimedThrower : MonoBehaviour
{
    #region �R���X�g���N�^
    /// <summary>
    /// �␳�������s�����߂̃��\�b�h
    /// </summary>
    /// <param name="pointerTransform">�|�C���^�[��Transform</param>
    /// <param name="throwPointerType">�|�C���^�[�̎w����@</param>
    public AimedThrower(Transform pointerTransform, ThrowPointerType throwPointerType)
    {
        // �|�C���^�[��Transform��ݒ�
        _aimPointer = pointerTransform;

        // �|�C���^�[�̎w����@��ݒ�
        _pointerType = throwPointerType;
    }
    #endregion

    #region �ϐ��E�v���p�e�B
    [SerializeField, Tooltip("�_�������������|�C���^�[��Transform")]
    private Transform _aimPointer = default;

    [SerializeField, Tooltip("�|�C���^�[�̎w����@\n�|�C���^�[���p�x�������Ȃ�Direction\n�|�C���^�[�����W�������Ȃ�Target")]
    private ThrowPointerType _pointerType = default;

    /// <summary>
    /// �����␳�̂��߂̃x�N�g�����擾���邽�߂̃v���p�e�B
    /// </summary>
    public Vector3 GetAimVector
    { 
        get 
        {
            // �|�C���^�[�̎w����@���Ƃɕ���
            switch (_pointerType)
            {
                case ThrowPointerType.Direction:
                    // �|�C���^�[�̐��ʕ����̃x�N�g����Ԃ�
                    return _aimPointer.forward;

                case ThrowPointerType.Target:
                    // �|�C���^�[�̍��W�Ɍ������x�N�g����Ԃ�
                    return (_aimPointer.position - this.transform.position).normalized;

                default:
                    // ��O
                    Debug.LogError($"AimedThrower��GetAimVector�ňُ�F_pointerType���w��O");
                    return Vector3.zero;
            }
        } 
    }

    /// <summary>
    /// �_�������������|�C���^�[�̎w����@
    /// </summary>
    public enum ThrowPointerType
    {
        Direction,
        Target
    }
    #endregion
}

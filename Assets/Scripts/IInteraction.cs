using UnityEngine;

/// <summary>
/// �C���^���N�g�����I�u�W�F�N�g�̃C���^�[�t�F�[�X
/// </summary>
public interface IInteraction
{
    GameObject gameObject { get; }
    ISelectedNotification SelectedNotification { get; }
    /// <summary>
    /// �I�u�W�F�N�g���C���^���N�g���ꂽ�Ƃ��ɌĂ΂�鏈��
    /// <br>�����̏ꍇ�A�v���C���[���I�u�W�F�N�g�̃R���C�_�[�ɐG�ꂽ�Ƃ��ɌĂ΂��</br>
    /// </summary>
    void Open();
    /// <summary>
    /// �I�u�W�F�N�g�̃C���^���N�g��Ԃ��痣���Ƃ��ɌĂ΂�鏈��
    /// <br>�����̏ꍇ�A�v���C���[���I�u�W�F�N�g�̃R���C�_�[���痣�ꂽ�Ƃ��ɌĂ΂��</br>
    /// </summary>
    void Close();
}
using UnityEngine;

/// <summary>
/// �C���^���N�g�����I�u�W�F�N�g�̃C���^�[�t�F�[�X
/// </summary>
public interface IInteraction
{
    /// <summary>
    /// �C���^���N�g���ꂽ�Ƃ��ɁA�I�u�W�F�N�g����v���C���[���֑��M������N���X
    /// </summary>
    public abstract class InteractionInfo { }
    /// <summary>
    /// InteractionInfo��Null�N���X
    /// </summary>
    public class NullInteractionInfo : InteractionInfo { }

    GameObject gameObject { get; }
    ISelectedNotification SelectedNotification { get; }
    /// <summary>
    /// �I�u�W�F�N�g���C���^���N�g���ꂽ�Ƃ��ɌĂ΂�鏈��
    /// <br>�����̏ꍇ�A�v���C���[���I�u�W�F�N�g�̃R���C�_�[�ɐG�ꂽ�Ƃ��ɌĂ΂��</br>
    /// </summary>
    InteractionInfo Open();
    /// <summary>
    /// �I�u�W�F�N�g�̃C���^���N�g��Ԃ��痣���Ƃ��ɌĂ΂�鏈��
    /// <br>�����̏ꍇ�A�v���C���[���I�u�W�F�N�g�̃R���C�_�[���痣�ꂽ�Ƃ��ɌĂ΂��</br>
    /// </summary>
    void Close();
}

public interface IInteractionInfoReceiver
{
    /// <summary>
    /// InteractionInfo��Set�����
    /// <br><b>���ӁF���O�ɃV�[�����PlayerInteraction�C���X�^���X�ւ�Add���K�v�B</b></br>
    /// </summary>
    void SetInfo(IInteraction.InteractionInfo interactionInfo);
}
using UnityEngine;

/// <summary>
/// ��d�̃C���^���N�g����������I�u�W�F�N�g�̊��N���X
/// <br>�i�R���C�_�[���ɓ���A�����͂��K�v�j</br>
/// </summary>
public abstract class SafetyInteractionObject : MonoBehaviour, IInteraction, ISelectedNotification
{
    ISelectedNotification IInteraction.SelectedNotification => this;
    private PlayerInputActions.InteractionActions Interaction => Inputter.Interaction;
    private PlayerInputActions.PlayerActions Player => Inputter.Player;

    protected virtual void Awake()
    {
        // Interact���͂̍w��
        Interaction.Interact.performed += _ =>
        {
            SafetyOpen();
            Interaction.Interact.Disable();
            Interaction.Disengage.Enable();
            // �v���C���[�̊�{������~�i�ړ��A�W�����v�A�]��j
            Player.Disable();
        };
        // Disengage���͂̍w��
        Interaction.Disengage.performed += _ =>
        {
            SafetyClose();
            Interaction.Interact.Enable();
            Interaction.Disengage.Disable();
            // �v���C���[�̊�{������ĊJ�i�ړ��A�W�����v�A�]��j
            Player.Enable();
        };
    }

    void IInteraction.Open()
    {
        // UI��\��
        NotificationUIManager.Instance.DisplayInteraction();
        Interaction.Interact.Enable();
    }

    void IInteraction.Close()
    {
        // UI���\��
        NotificationUIManager.Instance.HideInteraction();
        Interaction.Disable();
    }

    /// <summary>
    /// �I�u�W�F�N�g���C���^���N�g���ꂽ�Ƃ��ɌĂ΂�鏈��
    /// <br>�v���C���[���I�u�W�F�N�g�͈̔͏�œ��͂������Ƃ��ɌĂ΂��</br>
    /// </summary>
    protected abstract void SafetyOpen();
    /// <summary>
    /// �I�u�W�F�N�g�̃C���^���N�g��Ԃ��痣���Ƃ��ɌĂ΂�鏈��
    /// <br>�v���C���[���I�u�W�F�N�g�͈̔͏�œ��͂������Ƃ��ɌĂ΂��</br>
    /// </summary>
    protected abstract void SafetyClose();

    public abstract void Select(SelectArgs selectArgs);
    public abstract void Unselect(SelectArgs selectArgs);

    public virtual void Hover(SelectArgs selectArgs) { }
    public virtual void Unhover(SelectArgs selectArgs) { }
}
using UnityEngine;
using UniRx;
using System;

/// <summary>
/// ��d�̃C���^���N�g����������I�u�W�F�N�g�̊��N���X
/// <br>�i�R���C�_�[���ɓ���A�����͂��K�v�j</br>
/// </summary>
public abstract class SafetyInteractionObject : MonoBehaviour, IInteraction, ISelectedNotification
{
	protected bool canInteract = false;
	protected Subject<IInteraction.InteractionInfo> interactionInfoSubject = new Subject<IInteraction.InteractionInfo>();

	ISelectedNotification IInteraction.SelectedNotification => this;
	private PlayerInputActions.PlayerActions Player => Inputter.Player;
	public IObservable<IInteraction.InteractionInfo> InteractionInfoSubject => interactionInfoSubject;


	protected virtual void Awake()
	{
		// Interact���͂̍w��
		Player.Interact.performed += _ =>
		{
			XDebug.Log(canInteract, "magenta");
			if (canInteract)
			{
				SafetyOpen();
			}
		};
	}

	IInteraction.InteractionInfo IInteraction.Open()
	{
		canInteract = true;
		// UI��\��
		NotificationUIManager.Instance.DisplayInteraction();
		return new IInteraction.NullInteractionInfo();
	}

	void IInteraction.Close()
	{
		canInteract = false;
		// UI���\��
		NotificationUIManager.Instance.HideInteraction();
	}

	/// <summary>
	/// �I�u�W�F�N�g���C���^���N�g���ꂽ�Ƃ��ɌĂ΂�鏈��
	/// <br>�v���C���[���I�u�W�F�N�g�͈̔͏�œ��͂������Ƃ��ɌĂ΂��</br>
	/// </summary>
	protected abstract void SafetyOpen();
	/// <summary>
	/// �I�u�W�F�N�g�̃C���^���N�g��Ԃ��痣���Ƃ��ɌĂ΂�鏈��
	/// <br>�v���C���[���I�u�W�F�N�g�͈̔͏�œ��͂������Ƃ��ɌĂ΂��</br>
	/// <br>���e���p����ŌĂяo������</br>
	/// </summary>
	protected abstract void SafetyClose();

	public abstract void Select(SelectArgs selectArgs);
	public abstract void Unselect(SelectArgs selectArgs);

	public virtual void Hover(SelectArgs selectArgs) { }
	public virtual void Unhover(SelectArgs selectArgs) { }
}
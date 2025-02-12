using System;
using UnityEngine;

/// <summary>
/// ��d�̃C���^���N�g����������I�u�W�F�N�g�̊��N���X
/// <br>�i�R���C�_�[���ɓ���A�����͂��K�v�j</br>
/// </summary>
public abstract class SafetyInteractionObject : MonoBehaviour, IInteraction, ISelectedNotification
{
	public class SafetyInteractionInfo : IInteraction.InteractionInfo
	{
		public abstract class OnSafetyActionInfo { }
		public class NullOnSafetyActionInfo : OnSafetyActionInfo { }

		/// <summary>
		/// SafetyOpen�i�v���C���[��Interactable�ȃI�u�W�F�N�g�ɐG��A�����͂��������ꍇ�j���N�������ۂɔ��΂���f���Q�[�g
		/// <br>SafetyClose�����΂����^�C�~���O�Œ��g�����ׂ�Dispose�����</br>
		/// </summary>
		public event Action<OnSafetyActionInfo> OnSafetyOpenAction = default;
		/// <summary>
		/// SafetyClose�i�v���C���[��Interactable�������̑���ŗ��ꂽ�ꍇ�j���N�������ۂɔ��΂���f���Q�[�g
		/// <br>SafetyClose�����΂����^�C�~���O�Œ��g�����ׂ�Dispose�����</br>
		/// </summary>
		public event Action<OnSafetyActionInfo> OnSafetyCloseAction = default;

		/// <summary>
		/// SafetyOpen���Ƀf���Q�[�g�����s����
		/// <br>���s��SafetyInteractionObject�N���X�Ɍ���i�����Ŏ��g��n���j</br>
		/// </summary>
		/// <param name="_">�������g</param>
		/// <param name="data">Action�̈���</param>
		public void InvokeOpen(SafetyInteractionObject _, OnSafetyActionInfo data)
		{
			OnSafetyOpenAction?.Invoke(data);
		}
		/// <summary>
		/// SafetyClose���Ƀf���Q�[�g�����s����
		/// <br>���s��SafetyInteractionObject�N���X�Ɍ���i�����Ŏ��g��n���j</br>
		/// </summary>
		/// <param name="_">�������g</param>
		/// <param name="data">Action�̈���</param>
		public void InvokeClose(SafetyInteractionObject _, OnSafetyActionInfo data)
		{
			OnSafetyCloseAction?.Invoke(data);
		}
		/// <summary>
		/// SafetyOpen���̃f���Q�[�g�����Z�b�g����
		/// <br>���s��SafetyInteractionObject�N���X�Ɍ���i�����Ŏ��g��n���j</br>
		/// </summary>
		/// <param name="_">�������g</param>
		public void ClearOpen(SafetyInteractionObject _)
		{
			OnSafetyOpenAction = null;
		}
		/// <summary>
		/// SafetyClose���̃f���Q�[�g�����Z�b�g����
		/// <br>���s��SafetyInteractionObject�N���X�Ɍ���i�����Ŏ��g��n���j</br>
		/// </summary>
		/// <param name="_">�������g</param>
		public void ClearClose(SafetyInteractionObject _)
		{
			OnSafetyCloseAction = null;
		}
	}

	protected bool canInteract = false;
	protected bool canInteractLooking = false;

	ISelectedNotification IInteraction.SelectedNotification => this;
	private PlayerInputActions.PlayerActions Player => Inputter.Player;


	protected virtual void Awake()
	{
		// Interact���͂̍w��
		Player.Interact.performed += _ =>
		{
			if (canInteract)
			{
				SafetyOpen();
			}
			if (canInteractLooking)
			{
				SafetyOpenLooking();
			}
		};
	}

	public virtual IInteraction.InteractionInfo Open()
	{
		canInteract = true;
		// UI��\��
		//NotificationUIManager.Instance.DisplayInteraction();
		return new SafetyInteractionInfo();
	}

	public virtual IInteraction.InteractionInfo OpenLooking()
	{
		canInteractLooking = true;
		//NotificationUIManager.Instance.DisplayInteraction();
		return new SafetyInteractionInfo();
	}

	public virtual void Close()
	{
		canInteract = false;
		canInteractLooking = false;
		// UI���\��
		//NotificationUIManager.Instance.HideInteraction();
	}

	/// <summary>
	/// �I�u�W�F�N�g���C���^���N�g���ꂽ�Ƃ��ɌĂ΂�鏈��
	/// <br>�v���C���[���I�u�W�F�N�g�͈̔͏�œ��͂������Ƃ��ɌĂ΂��</br>
	/// </summary>
	protected virtual void SafetyOpen() { }
	protected virtual void SafetyOpenLooking() { }
	/// <summary>
	/// �I�u�W�F�N�g�̃C���^���N�g��Ԃ��痣���Ƃ��ɌĂ΂�鏈��
	/// <br>�v���C���[���I�u�W�F�N�g�͈̔͏�œ��͂������Ƃ��ɌĂ΂��</br>
	/// <br>���e���p����ŌĂяo������</br>
	/// </summary>
	protected abstract void SafetyClose();

	public virtual void Select(SelectArgs selectArgs) { }
	public virtual void Unselect(SelectArgs selectArgs) { }

	public virtual void Hover(SelectArgs selectArgs) { }
	public virtual void Unhover(SelectArgs selectArgs) { }
}
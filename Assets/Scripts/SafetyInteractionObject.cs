using UnityEngine;
using UniRx;
using System;

/// <summary>
/// ��d�̃C���^���N�g����������I�u�W�F�N�g�̊��N���X
/// <br>�i�R���C�_�[���ɓ���A�����͂��K�v�j</br>
/// </summary>
public abstract class SafetyInteractionObject : MonoBehaviour, IInteraction, ISelectedNotification
{
	public class SafetyInteractionInfo : IInteraction.InteractionInfo
	{
		/// <summary>
		/// SafetyOpen�i�v���C���[��Interactable�ȃI�u�W�F�N�g�ɐG��A�����͂��������ꍇ�j���N�������ۂɔ��΂���f���Q�[�g
		/// <br>SafetyClose�����΂����^�C�~���O�Œ��g�����ׂ�Dispose�����</br>
		/// </summary>
		public event Action OnSafetyOpenAction = default;
		/// <summary>
		/// SafetyClose�i�v���C���[��Interactable�������̑���ŗ��ꂽ�ꍇ�j���N�������ۂɔ��΂���f���Q�[�g
		/// <br>SafetyClose�����΂����^�C�~���O�Œ��g�����ׂ�Dispose�����</br>
		/// </summary>
		public event Action OnSafetyCloseAction = default;

		/// <summary>
		/// SafetyOpen���Ƀf���Q�[�g�����s����
		/// <br>���s��SafetyInteractionObject�N���X�Ɍ���i�����Ŏ��g��n���j</br>
		/// </summary>
		/// <param name="safetyInteractionObject">�������g</param>
		public void InvokeOpen(SafetyInteractionObject safetyInteractionObject)
		{
			OnSafetyOpenAction?.Invoke();
		}
		/// <summary>
		/// SafetyClose���Ƀf���Q�[�g�����s����
		/// <br>���s��SafetyInteractionObject�N���X�Ɍ���i�����Ŏ��g��n���j</br>
		/// </summary>
		/// <param name="safetyInteractionObject">�������g</param>
		public void InvokeClose(SafetyInteractionObject safetyInteractionObject)
		{
			OnSafetyCloseAction?.Invoke();
		}
		/// <summary>
		/// SafetyOpen���̃f���Q�[�g�����Z�b�g����
		/// <br>���s��SafetyInteractionObject�N���X�Ɍ���i�����Ŏ��g��n���j</br>
		/// </summary>
		/// <param name="safetyInteractionObject">�������g</param>
		public void ClearOpen(SafetyInteractionObject safetyInteractionObject)
		{
			OnSafetyOpenAction = null;
		}
		/// <summary>
		/// SafetyClose���̃f���Q�[�g�����Z�b�g����
		/// <br>���s��SafetyInteractionObject�N���X�Ɍ���i�����Ŏ��g��n���j</br>
		/// </summary>
		/// <param name="safetyInteractionObject">�������g</param>
		public void ClearClose(SafetyInteractionObject safetyInteractionObject)
		{
			OnSafetyCloseAction = null;
		}
	}

	protected bool canInteract = false;

	ISelectedNotification IInteraction.SelectedNotification => this;
	private PlayerInputActions.PlayerActions Player => Inputter.Player;


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

	public virtual IInteraction.InteractionInfo Open()
	{
		canInteract = true;
		// UI��\��
		NotificationUIManager.Instance.DisplayInteraction();
		return new SafetyInteractionInfo();
	}

	public virtual void Close()
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
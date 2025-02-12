using System;
using UnityEngine;

/// <summary>
/// 二重のインタラクトを実装するオブジェクトの基底クラス
/// <br>（コライダー内に入り、かつ入力が必要）</br>
/// </summary>
public abstract class SafetyInteractionObject : MonoBehaviour, IInteraction, ISelectedNotification
{
	public class SafetyInteractionInfo : IInteraction.InteractionInfo
	{
		public abstract class OnSafetyActionInfo { }
		public class NullOnSafetyActionInfo : OnSafetyActionInfo { }

		/// <summary>
		/// SafetyOpen（プレイヤーがInteractableなオブジェクトに触れ、かつ入力があった場合）が起動した際に発火するデリゲート
		/// <br>SafetyCloseが発火したタイミングで中身がすべてDisposeされる</br>
		/// </summary>
		public event Action<OnSafetyActionInfo> OnSafetyOpenAction = default;
		/// <summary>
		/// SafetyClose（プレイヤーがInteractableから特定の操作で離れた場合）が起動した際に発火するデリゲート
		/// <br>SafetyCloseが発火したタイミングで中身がすべてDisposeされる</br>
		/// </summary>
		public event Action<OnSafetyActionInfo> OnSafetyCloseAction = default;

		/// <summary>
		/// SafetyOpen時にデリゲートを実行する
		/// <br>実行はSafetyInteractionObjectクラスに限る（引数で自身を渡す）</br>
		/// </summary>
		/// <param name="_">自分自身</param>
		/// <param name="data">Actionの引数</param>
		public void InvokeOpen(SafetyInteractionObject _, OnSafetyActionInfo data)
		{
			OnSafetyOpenAction?.Invoke(data);
		}
		/// <summary>
		/// SafetyClose時にデリゲートを実行する
		/// <br>実行はSafetyInteractionObjectクラスに限る（引数で自身を渡す）</br>
		/// </summary>
		/// <param name="_">自分自身</param>
		/// <param name="data">Actionの引数</param>
		public void InvokeClose(SafetyInteractionObject _, OnSafetyActionInfo data)
		{
			OnSafetyCloseAction?.Invoke(data);
		}
		/// <summary>
		/// SafetyOpen時のデリゲートをリセットする
		/// <br>実行はSafetyInteractionObjectクラスに限る（引数で自身を渡す）</br>
		/// </summary>
		/// <param name="_">自分自身</param>
		public void ClearOpen(SafetyInteractionObject _)
		{
			OnSafetyOpenAction = null;
		}
		/// <summary>
		/// SafetyClose時のデリゲートをリセットする
		/// <br>実行はSafetyInteractionObjectクラスに限る（引数で自身を渡す）</br>
		/// </summary>
		/// <param name="_">自分自身</param>
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
		// Interact入力の購読
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
		// UIを表示
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
		// UIを非表示
		//NotificationUIManager.Instance.HideInteraction();
	}

	/// <summary>
	/// オブジェクトがインタラクトされたときに呼ばれる処理
	/// <br>プレイヤーがオブジェクトの範囲上で入力をしたときに呼ばれる</br>
	/// </summary>
	protected virtual void SafetyOpen() { }
	protected virtual void SafetyOpenLooking() { }
	/// <summary>
	/// オブジェクトのインタラクト状態から離れるときに呼ばれる処理
	/// <br>プレイヤーがオブジェクトの範囲上で入力をしたときに呼ばれる</br>
	/// <br>※各自継承先で呼び出すこと</br>
	/// </summary>
	protected abstract void SafetyClose();

	public virtual void Select(SelectArgs selectArgs) { }
	public virtual void Unselect(SelectArgs selectArgs) { }

	public virtual void Hover(SelectArgs selectArgs) { }
	public virtual void Unhover(SelectArgs selectArgs) { }
}
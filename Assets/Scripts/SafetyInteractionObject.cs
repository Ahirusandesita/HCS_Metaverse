using UnityEngine;

/// <summary>
/// 二重のインタラクトを実装するオブジェクトの基底クラス
/// <br>（コライダー内に入り、かつ入力が必要）</br>
/// </summary>
public abstract class SafetyInteractionObject : MonoBehaviour, IInteraction, ISelectedNotification
{
	protected bool canInteract = false;

	ISelectedNotification IInteraction.SelectedNotification => this;
	private PlayerInputActions.PlayerActions Player => Inputter.Player;


	protected virtual void Awake()
	{
		// Interact入力の購読
		Player.Interact.performed += _ =>
		{
			XDebug.Log(canInteract, "magenta");
			if (canInteract)
			{
				SafetyOpen();
			}
		};
	}

	void IInteraction.Open()
	{
		canInteract = true;
		// UIを表示
		NotificationUIManager.Instance.DisplayInteraction();
	}

	void IInteraction.Close()
	{
		canInteract = false;
		// UIを非表示
		NotificationUIManager.Instance.HideInteraction();
	}

	/// <summary>
	/// オブジェクトがインタラクトされたときに呼ばれる処理
	/// <br>プレイヤーがオブジェクトの範囲上で入力をしたときに呼ばれる</br>
	/// </summary>
	protected abstract void SafetyOpen();
	/// <summary>
	/// オブジェクトのインタラクト状態から離れるときに呼ばれる処理
	/// <br>プレイヤーがオブジェクトの範囲上で入力をしたときに呼ばれる</br>
	/// <br>※各自継承先で呼び出すこと</br>
	/// </summary>
	protected abstract void SafetyClose();

	public abstract void Select(SelectArgs selectArgs);
	public abstract void Unselect(SelectArgs selectArgs);

	public virtual void Hover(SelectArgs selectArgs) { }
	public virtual void Unhover(SelectArgs selectArgs) { }
}
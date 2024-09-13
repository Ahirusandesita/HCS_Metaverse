using UnityEngine;

/// <summary>
/// 二重のインタラクトを実装するオブジェクトの基底クラス
/// <br>（コライダー内に入り、かつ入力が必要）</br>
/// </summary>
public abstract class SafetyInteractionObject : MonoBehaviour, IInteraction, ISelectedNotification
{
    ISelectedNotification IInteraction.SelectedNotification => this;
    private PlayerInputActions.InteractionActions Interaction => Inputter.Interaction;
    private PlayerInputActions.PlayerActions Player => Inputter.Player;

    protected virtual void Awake()
    {
        // Interact入力の購読
        Interaction.Interact.performed += _ =>
        {
            SafetyOpen();
            Interaction.Interact.Disable();
            Interaction.Disengage.Enable();
            // プレイヤーの基本操作を停止（移動、ジャンプ、転回）
            Player.Disable();
        };
        // Disengage入力の購読
        Interaction.Disengage.performed += _ =>
        {
            SafetyClose();
            Interaction.Interact.Enable();
            Interaction.Disengage.Disable();
            // プレイヤーの基本操作を再開（移動、ジャンプ、転回）
            Player.Enable();
        };
    }

    void IInteraction.Open()
    {
        // UIを表示
        NotificationUIManager.Instance.DisplayInteraction();
        Interaction.Interact.Enable();
    }

    void IInteraction.Close()
    {
        // UIを非表示
        NotificationUIManager.Instance.HideInteraction();
        Interaction.Disable();
    }

    /// <summary>
    /// オブジェクトがインタラクトされたときに呼ばれる処理
    /// <br>プレイヤーがオブジェクトの範囲上で入力をしたときに呼ばれる</br>
    /// </summary>
    protected abstract void SafetyOpen();
    /// <summary>
    /// オブジェクトのインタラクト状態から離れるときに呼ばれる処理
    /// <br>プレイヤーがオブジェクトの範囲上で入力をしたときに呼ばれる</br>
    /// </summary>
    protected abstract void SafetyClose();

    public abstract void Select(SelectArgs selectArgs);
    public abstract void Unselect(SelectArgs selectArgs);

    public virtual void Hover(SelectArgs selectArgs) { }
    public virtual void Unhover(SelectArgs selectArgs) { }
}
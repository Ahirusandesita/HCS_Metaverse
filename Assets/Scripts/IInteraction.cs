using UnityEngine;

/// <summary>
/// インタラクトされるオブジェクトのインターフェース
/// </summary>
public interface IInteraction
{
    /// <summary>
    /// インタラクトされたときに、オブジェクトからプレイヤー等へ送信する情報クラス
    /// </summary>
    public abstract class InteractionInfo { }
    /// <summary>
    /// InteractionInfoのNullクラス
    /// </summary>
    public class NullInteractionInfo : InteractionInfo { }

    GameObject gameObject { get; }
    ISelectedNotification SelectedNotification { get; }
    /// <summary>
    /// オブジェクトがインタラクトされたときに呼ばれる処理
    /// <br>多くの場合、プレイヤーがオブジェクトのコライダーに触れたときに呼ばれる</br>
    /// </summary>
    InteractionInfo Open();
    /// <summary>
    /// オブジェクトのインタラクト状態から離れるときに呼ばれる処理
    /// <br>多くの場合、プレイヤーがオブジェクトのコライダーから離れたときに呼ばれる</br>
    /// </summary>
    void Close();
}

public interface IInteractionInfoReceiver
{
    /// <summary>
    /// InteractionInfoがSetされる
    /// <br><b>注意：事前にシーン上のPlayerInteractionインスタンスへのAddが必要。</b></br>
    /// </summary>
    void SetInfo(IInteraction.InteractionInfo interactionInfo);
}
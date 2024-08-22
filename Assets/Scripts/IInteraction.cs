using UnityEngine;

/// <summary>
/// インタラクトされるオブジェクトのインターフェース
/// </summary>
public interface IInteraction
{
    GameObject gameObject { get; }
    ISelectedNotification SelectedNotification { get; }
    /// <summary>
    /// オブジェクトがインタラクトされたときに呼ばれる処理
    /// <br>多くの場合、プレイヤーがオブジェクトのコライダーに触れたときに呼ばれる</br>
    /// </summary>
    void Open();
    /// <summary>
    /// オブジェクトのインタラクト状態から離れるときに呼ばれる処理
    /// <br>多くの場合、プレイヤーがオブジェクトのコライダーから離れたときに呼ばれる</br>
    /// </summary>
    void Close();
}
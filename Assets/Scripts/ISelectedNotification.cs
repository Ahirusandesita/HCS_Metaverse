
/// <summary>
/// 各アイテムが掴まれる/離される情報を受け取るインターフェース
/// </summary>
public interface ISelectedNotification
{
    /// <summary>
    /// アイテムが掴まれたときに呼ばれる処理
    /// </summary>
    /// <param name="selectArgs">送信データ</param>
    void Select(SelectArgs selectArgs);
    /// <summary>
    /// アイテムが離されたときに呼ばれる処理
    /// </summary>
    /// <param name="selectArgs">送信データ</param>
    void Unselect(SelectArgs selectArgs);
    /// <summary>
    /// アイテムがポイントされたときに呼ばれる処理
    /// </summary>
    /// <param name="selectArgs">送信データ</param>
    void Hover(SelectArgs selectArgs) { }
    /// <summary>
    /// アイテムがポイント状態から離れたときに呼ばれる処理
    /// </summary>
    /// <param name="selectArgs">送信データ</param>
    void Unhover(SelectArgs selectArgs) { }
}

public sealed class NullSelectedNotification : ISelectedNotification
{
    void ISelectedNotification.Select(SelectArgs selectArgs) { }
    void ISelectedNotification.Unselect(SelectArgs selectArgs) { }
}
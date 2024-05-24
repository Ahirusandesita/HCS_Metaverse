/// <summary>
/// インベントリ
/// </summary>
public interface IInventoryOneFrame
{
    /// <summary>
    /// インベントリに格納されているか
    /// </summary>
    bool HasItem { get; }
    /// <summary>
    /// インベントリに格納する
    /// </summary>
    /// <param name="item"></param>
    void PutAway(IItem item);
    /// <summary>
    /// インベントリから取り出す
    /// </summary>
    /// <returns></returns>
    IItem TakeOut();
}
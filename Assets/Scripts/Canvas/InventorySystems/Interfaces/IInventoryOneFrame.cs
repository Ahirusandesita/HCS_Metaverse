/// <summary>
/// インベントリ
/// </summary>
public interface IInventoryOneFrame
{
    /// <summary>
    /// インベントリに格納されているか
    /// </summary>
    bool HasItem { get; }
    bool MatchItem(ItemAsset itemAsset);
    /// <summary>
    /// インベントリに格納する
    /// </summary>
    /// <param name="item"></param>
    void PutAway(ItemAsset itemAsset);
    /// <summary>
    /// インベントリから取り出す
    /// </summary>
    /// <returns></returns>
    void TakeOut();
    void Inject(InventoryManager inventoryManager);
    void SelectItemInject(SelectItem selectItem,NotExistIcon notExistIcon);
}
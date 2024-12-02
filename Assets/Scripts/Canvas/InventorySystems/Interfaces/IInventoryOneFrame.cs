/// <summary>
/// �C���x���g��
/// </summary>
public interface IInventoryOneFrame
{
    /// <summary>
    /// �C���x���g���Ɋi�[����Ă��邩
    /// </summary>
    bool HasItem { get; }
    bool MatchItem(ItemAsset itemAsset);
    /// <summary>
    /// �C���x���g���Ɋi�[����
    /// </summary>
    /// <param name="item"></param>
    void PutAway(ItemAsset itemAsset);
    /// <summary>
    /// �C���x���g��������o��
    /// </summary>
    /// <returns></returns>
    void TakeOut();
    void Inject(InventoryManager inventoryManager);
    void SelectItemInject(SelectItem selectItem,NotExistIcon notExistIcon);
}
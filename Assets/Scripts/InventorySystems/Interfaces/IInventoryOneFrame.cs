/// <summary>
/// �C���x���g��
/// </summary>
public interface IInventoryOneFrame
{
    /// <summary>
    /// �C���x���g���Ɋi�[����Ă��邩
    /// </summary>
    bool HasItem { get; }
    /// <summary>
    /// �C���x���g���Ɋi�[����
    /// </summary>
    /// <param name="item"></param>
    void PutAway(IItem item);
    /// <summary>
    /// �C���x���g��������o��
    /// </summary>
    /// <returns></returns>
    IItem TakeOut();
}
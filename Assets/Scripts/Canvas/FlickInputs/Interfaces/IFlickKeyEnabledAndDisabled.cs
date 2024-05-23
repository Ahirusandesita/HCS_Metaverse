/// <summary>
/// フリックキーを有効化、無効化できる
/// </summary>
public interface IFlickKeyEnabledAndDisabled
{
    /// <summary>
    /// キーの有効化
    /// </summary>
    void Enabled();
    /// <summary>
    /// キーの無効化
    /// </summary>
    void Disabled();
}
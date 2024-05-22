using UnityEngine;
/// <summary>
/// スクロール機能
/// </summary>
public interface IScrollable
{
    /// <summary>
    /// スクロールする
    /// </summary>
    /// <param name="moveValue">接触点の移動量</param>
    /// <param name="sensitivity">感度</param>
    void Scroll(Vector2 moveValue, float sensitivity);
}

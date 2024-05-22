using UnityEngine;
/// <summary>
/// X軸のみのスクロール機能
/// </summary>
public interface IHorizontalOnlyScrollable : IScrollable
{
    /// <summary>
    /// スクロールする
    /// </summary>
    /// <param name="moveValue">接触点の移動量</param>
    /// <param name="sensitivity">感度</param>
    new void Scroll(Vector2 moveValue, float sensitivity);
}
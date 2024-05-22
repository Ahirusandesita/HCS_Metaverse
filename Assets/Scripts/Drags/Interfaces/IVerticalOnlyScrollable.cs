using UnityEngine;
/// <summary>
/// Y軸のみのスクロール機能
/// </summary>
public interface IVerticalOnlyScrollable : IScrollable
{
    /// <summary>
    /// スクロールする
    /// </summary>
    /// <param name="move">接触点の移動量</param>
    /// <param name="sensitivity">感度</param>
    new void Scroll(Vector2 move, float sensitivity);
}

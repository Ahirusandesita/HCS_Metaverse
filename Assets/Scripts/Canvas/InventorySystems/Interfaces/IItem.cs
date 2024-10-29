using UnityEngine;
/// <summary>
/// アイテム
/// </summary>
public interface IItem
{
    /// <summary>
    /// アイテム使用
    /// </summary>
    void Use();
    /// <summary>
    /// アイテムをしまう
    /// </summary>
    void CleanUp();
    /// <summary>
    /// アイテムを取り出す
    /// </summary>
    /// <param name="position">取り出す場所</param>
    void TakeOut(Vector3 position);
}


using UnityEngine;
/// <summary>
/// X���݂̂̃X�N���[���@�\
/// </summary>
public interface IHorizontalOnlyScrollable : IScrollable
{
    /// <summary>
    /// �X�N���[������
    /// </summary>
    /// <param name="moveValue">�ڐG�_�̈ړ���</param>
    /// <param name="sensitivity">���x</param>
    new void Scroll(Vector2 moveValue, float sensitivity);
}
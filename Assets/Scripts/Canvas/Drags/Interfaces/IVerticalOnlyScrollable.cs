using UnityEngine;
/// <summary>
/// Y���݂̂̃X�N���[���@�\
/// </summary>
public interface IVerticalOnlyScrollable : IScrollable
{
    /// <summary>
    /// �X�N���[������
    /// </summary>
    /// <param name="move">�ڐG�_�̈ړ���</param>
    /// <param name="sensitivity">���x</param>
    new void Scroll(Vector2 move, float sensitivity);
}

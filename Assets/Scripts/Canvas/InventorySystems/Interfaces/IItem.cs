using UnityEngine;
/// <summary>
/// �A�C�e��
/// </summary>
public interface IItem
{
    /// <summary>
    /// �A�C�e���g�p
    /// </summary>
    void Use();
    /// <summary>
    /// �A�C�e�������܂�
    /// </summary>
    void CleanUp();
    /// <summary>
    /// �A�C�e�������o��
    /// </summary>
    /// <param name="position">���o���ꏊ</param>
    void TakeOut(Vector3 position);
}


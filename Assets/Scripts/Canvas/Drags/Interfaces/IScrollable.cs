using System;
using UnityEngine;
/// <summary>
/// �X�N���[���@�\
/// </summary>
public interface IScrollable
{
    /// <summary>
    /// �X�N���[������
    /// </summary>
    /// <param name="moveValue">�ڐG�_�̈ړ���</param>
    /// <param name="sensitivity">���x</param>
    void Scroll(Vector2 moveValue, float sensitivity);
    void UnSubscribe(Action action);
}

public interface ITransformInjectable
{
    void TransformInject(Transform transform);
}
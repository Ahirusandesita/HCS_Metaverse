using System;
using UnityEngine;

/// <summary>
/// �v�[���I�u�W�F�N�g�i�v�[�����O�����I�u�W�F�N�g�j�̊��N���X
/// </summary>
public abstract class PoolObject : MonoBehaviour
{
    protected Transform myTransform = default;


    protected virtual void Awake()
    {
        myTransform = this.transform;
    }

    /// <summary>
    /// ���I�u�W�F�N�g�̐����Ɠ����ɁA�Ăяo��������Pool�̎Q�Ƃ��Z�b�g�����
    /// <br>- �h���N���X����Pool.Return()���g�p���ĕԋp���邱��</br>
    /// <br>- Setter�͎g�p���Ȃ�����</br>
    /// </summary>
    public IReturnablePool Pool { get; set; }

    /// <summary>
    /// �����������ꂽ�C���X�^���X���ǂ���
    /// </summary>
    public bool IsInitialCreate { get; set; } = false;

    /// <summary>
    /// ���������̒���ɌĂ΂�鏈��
    /// </summary>
    public virtual void Initialize()
    {
        this.gameObject.SetActive(false);
    }

    public PoolObject Initialize(Action action)
    {
        action?.Invoke();
        return this;
    }

    public PoolObject Initialize<T>(Action<T> action, T t)
    {
        action?.Invoke(t);
        return this;
    }

    /// <summary>
    /// Dequeue(Get)���ꂽ����ɌĂ΂�鏈��
    /// <br>base: �����ʒu�Ə����p�x�̑���A��A�N�e�B�u��</br>
    /// </summary>
    public virtual void Enable(Vector2 initialPos, Quaternion initialDir)
    {
        myTransform.position = initialPos;
        myTransform.rotation = initialDir;
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Enqueue(Return)����钼�O�ɌĂ΂�鏈��
    /// <br>base: ��A�N�e�B�u��</br>
    /// </summary>
    public virtual void Disable()
    {
        this.gameObject.SetActive(false);
    }
}

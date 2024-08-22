using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// �I�u�W�F�N�g�v�[���ɕԋp�\�ȃC���^�[�t�F�[�X
/// </summary>
public interface IReturnablePool
{
    /// <summary>
    /// �I�u�W�F�N�g���v�[���ɕԋp����
    /// </summary>
    /// <param name="thisObj">�������g</param>
    void Return(PoolObject thisObj);
}

/// <summary>
/// �I�u�W�F�N�g�v�[������擾�\�ȃC���^�[�t�F�[�X
/// </summary>
public interface IGettablePool
{
    public IReadOnlyCollection<PoolObject> ObjectPool { get; }

    /// <summary>
    ///  �v�[������I�u�W�F�N�g���擾����
    ///  <br>- �������FAuto</br>
    /// </summary>
    /// <param name="initialPos">�����ʒu</param>
    /// <param name="initialDir">�����p�x</param>
    /// <returns>�擾�����I�u�W�F�N�g</returns>
    PoolObject Get(Vector2 initialPos, Quaternion initialDir);

    /// <summary>
    ///  �v�[������I�u�W�F�N�g���擾����
    ///  <br>- �������FManual�iEnable���蓮�Ŏ��s����K�v������j</br>
    /// </summary>
    /// <returns>�擾�����I�u�W�F�N�g</returns>
    PoolObject Get();

    /// <summary>
    /// �v�[�����폜����
    /// </summary>
    void Dispose();
}

/// <summary>
/// �I�u�W�F�N�g�v�[���𐶐�����N���X
/// </summary>
public class ObjectPooler : IReturnablePool, IGettablePool, IDisposable
{
    private PoolObject prefab = default;
    private GameObject parent = default;
    private Queue<PoolObject> objectPool = default;

    public IReadOnlyCollection<PoolObject> ObjectPool => objectPool;


    #region �R���X�g���N�^
    /// <summary>
    /// �I�u�W�F�N�g�v�[���𐶐�����R���X�g���N�^
    /// <br>- new�����i�K��Instantiate�����邽�ߒ���</br>
    /// <br>- new�̖߂�l��IGettablePool�C���^�[�t�F�[�X�Ŏ󂯎�邱��</br>
    /// </summary>
    public ObjectPooler(PoolObjectAsset createObjectData, string parentName = null, Transform setParent = null)
    {
        prefab = createObjectData.Prefab;
        objectPool = new();

        // �e�I�u�W�F�N�g�𐶐�
        if (parentName is null)
        {
            parentName = "PooledObjects";
        }
        parent = new GameObject(parentName);

        // �e�I�u�W�F�N�g��ݒ�
        if (setParent is not null)
        {
            parent.transform.SetParent(setParent);
        }

        // ��������
        for (int i = 0; i < createObjectData.MaxCreateCount; i++)
        {
            PoolObject obj = Object.Instantiate(prefab, parent.transform);
            obj.Pool = this;            // ObjectPooler�N���X�̃C���X�^���X��Set����
            obj.IsInitialCreate = true;
            obj.Initialize();           // ���������I�u�W�F�N�g�̏������������Ăяo��
            objectPool.Enqueue(obj);
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g�v�[���𐶐�����R���X�g���N�^
    /// <br>- new�����i�K��Instantiate�����邽�ߒ���</br>
    /// <br>- new�̖߂�l��IGettablePool�C���^�[�t�F�[�X�Ŏ󂯎�邱��</br>
    /// </summary>
    public ObjectPooler(PoolObjectAsset createObjectData, Action initializeAction, string parentName = null, Transform setParent = null)
    {
        prefab = createObjectData.Prefab;
        objectPool = new();

        // �e�I�u�W�F�N�g�𐶐�
        if (parentName is null)
        {
            parentName = "PooledObjects";
        }
        parent = new GameObject(parentName);

        // �e�I�u�W�F�N�g��ݒ�
        if (setParent is not null)
        {
            parent.transform.SetParent(setParent);
        }

        // ��������
        for (int i = 0; i < createObjectData.MaxCreateCount; i++)
        {
            PoolObject obj = Object.Instantiate(prefab, parent.transform);
            obj.Pool = this;            // ObjectPooler�N���X�̃C���X�^���X��Set����
            obj.IsInitialCreate = true;
            obj.Initialize(initializeAction);           // ���������I�u�W�F�N�g�̏������������Ăяo��
            objectPool.Enqueue(obj);
        }
    }
    #endregion


    public PoolObject Get(Vector2 initialPos, Quaternion initialDir)
    {
        PoolObject obj;

        if (objectPool.Count > 0)
        {
            obj = objectPool.Dequeue();
        }
        // �L���[�̒��g����ł���΁A�V���ɐ�������
        else
        {
            obj = Object.Instantiate(prefab, parent.transform);
            obj.Initialize();
            obj.Pool = this;
        }

        obj.Enable(initialPos, initialDir);
        return obj;
    }

    public PoolObject Get()
    {
        PoolObject obj;

        if (objectPool.Count > 0)
        {
            obj = objectPool.Dequeue();
        }
        // �L���[�̒��g����ł���΁A�V���ɐ�������
        else
        {
            obj = Object.Instantiate(prefab, parent.transform);
            obj.Initialize();
            obj.Pool = this;
        }

        return obj;
    }

    public void Return(PoolObject thisObj)
    {
        thisObj.Disable();
        objectPool.Enqueue(thisObj);
    }

    public void Dispose()
    {
        Object.Destroy(parent);

        objectPool.Clear();
        objectPool.TrimExcess();

        prefab = null;
        parent = null;
        objectPool = null;
    }
}


/// <summary>
/// �I�u�W�F�N�g�v�[���𐶐�����N���X
/// </summary>
public class ObjectPooler<T> : IReturnablePool, IGettablePool, IDisposable
{
    private PoolObject prefab = default;
    private GameObject parent = default;
    private Queue<PoolObject> objectPool = default;
    private readonly Action<T> initializeAction = default;
    private readonly T t = default;

    public IReadOnlyCollection<PoolObject> ObjectPool => objectPool;


    /// <summary>
    /// �I�u�W�F�N�g�v�[���𐶐�����R���X�g���N�^
    /// <br>- new�����i�K��Instantiate�����邽�ߒ���</br>
    /// <br>- new�̖߂�l��IGettablePool�C���^�[�t�F�[�X�Ŏ󂯎�邱��</br>
    /// </summary>
    public ObjectPooler(PoolObjectAsset createObjectData, Action<T> initializeAction, T t, string parentName = null)
    {
        prefab = createObjectData.Prefab;
        objectPool = new();
        this.initializeAction = initializeAction;
        this.t = t;

        // �e�I�u�W�F�N�g�𐶐�
        if (parentName is null)
        {
            parentName = "PooledObjects";
        }
        parent = new GameObject(parentName);

        // ��������
        for (int i = 0; i < createObjectData.MaxCreateCount; i++)
        {
            PoolObject obj = Object.Instantiate(prefab, parent.transform);
            obj.Pool = this;            // ObjectPooler�N���X�̃C���X�^���X��Set����
            obj.IsInitialCreate = true;
            obj.Initialize(initializeAction, t);           // ���������I�u�W�F�N�g�̏������������Ăяo��
            objectPool.Enqueue(obj);
        }
    }


    public PoolObject Get(Vector2 initialPos, Quaternion initialDir)
    {
        PoolObject obj;

        if (objectPool.Count > 0)
        {
            obj = objectPool.Dequeue();
        }
        // �L���[�̒��g����ł���΁A�V���ɐ�������
        else
        {
            obj = Object.Instantiate(prefab, parent.transform);
            obj.Initialize(initializeAction, t);
            obj.Pool = this;
        }

        obj.Enable(initialPos, initialDir);
        return obj;
    }

    public PoolObject Get()
    {
        PoolObject obj;

        if (objectPool.Count > 0)
        {
            obj = objectPool.Dequeue();
        }
        // �L���[�̒��g����ł���΁A�V���ɐ�������
        else
        {
            obj = Object.Instantiate(prefab, parent.transform);
            obj.Initialize(initializeAction, t);
            obj.Pool = this;
        }

        return obj;
    }

    public void Return(PoolObject thisObj)
    {
        thisObj.Disable();
        objectPool.Enqueue(thisObj);
    }

    public void Dispose()
    {
        Object.Destroy(parent);

        objectPool.Clear();
        objectPool.TrimExcess();

        prefab = null;
        parent = null;
        objectPool = null;
    }
}

public class ParticlePooler
{
    private ParticleSystem particle = default;
    private GameObject parent = default;
    private Queue<ParticleSystem> objectPool = default;

    public IReadOnlyCollection<ParticleSystem> ObjectPool => objectPool;


    /// <summary>
    /// �I�u�W�F�N�g�v�[���𐶐�����R���X�g���N�^
    /// <br>- new�����i�K��Instantiate�����邽�ߒ���</br>
    /// <br>- new�̖߂�l��IGettablePool�C���^�[�t�F�[�X�Ŏ󂯎�邱��</br>
    /// </summary>
    public ParticlePooler(PoolParticleAsset createEfectData, string parentName = null, Transform setParent = null)
    {
        particle = createEfectData.Partilce;
        objectPool = new();

        // �e�I�u�W�F�N�g�𐶐�
        if (parentName is null)
        {
            parentName = "PooledObjects";
        }
        parent = new GameObject(parentName);

        // �e�I�u�W�F�N�g��ݒ�
        if (setParent is not null)
        {
            parent.transform.SetParent(setParent);
        }

        // ��������
        for (int i = 0; i < createEfectData.MaxCreateCount; i++)
        {
            ParticleSystem obj = Object.Instantiate(particle, parent.transform);
            obj.gameObject.SetActive(true);
            obj.Stop();
            objectPool.Enqueue(obj);
        }
    }

    public ParticleSystem Play(Vector2 initialPos, Quaternion initialDir)
    {
        ParticleSystem obj;

        if (objectPool.Count > 0)
        {
            obj = objectPool.Dequeue();
        }
        // �L���[�̒��g����ł���΁A�V���ɐ�������
        else
        {
            obj = Object.Instantiate(particle, parent.transform);
            obj.gameObject.SetActive(true);
            obj.Stop();
            objectPool.Enqueue(obj);
        }

        obj.transform.position = initialPos;
        obj.transform.rotation = initialDir;
        obj.Play();
        return obj;
    }
}
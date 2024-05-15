using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// オブジェクトプールに返却可能なインターフェース
/// </summary>
public interface IReturnablePool
{
    /// <summary>
    /// オブジェクトをプールに返却する
    /// </summary>
    /// <param name="thisObj">自分自身</param>
    void Return(PoolObject thisObj);
}

/// <summary>
/// オブジェクトプールから取得可能なインターフェース
/// </summary>
public interface IGettablePool
{
    public IReadOnlyCollection<PoolObject> ObjectPool { get; }

    /// <summary>
    ///  プールからオブジェクトを取得する
    ///  <br>- 初期化：Auto</br>
    /// </summary>
    /// <param name="initialPos">初期位置</param>
    /// <param name="initialDir">初期角度</param>
    /// <returns>取得したオブジェクト</returns>
    PoolObject Get(Vector2 initialPos, Quaternion initialDir);

    /// <summary>
    ///  プールからオブジェクトを取得する
    ///  <br>- 初期化：Manual（Enableを手動で実行する必要がある）</br>
    /// </summary>
    /// <returns>取得したオブジェクト</returns>
    PoolObject Get();

    /// <summary>
    /// プールを削除する
    /// </summary>
    void Dispose();
}

/// <summary>
/// オブジェクトプールを生成するクラス
/// </summary>
public class ObjectPooler : IReturnablePool, IGettablePool, IDisposable
{
    private PoolObject prefab = default;
    private GameObject parent = default;
    private Queue<PoolObject> objectPool = default;

    public IReadOnlyCollection<PoolObject> ObjectPool => objectPool;


    #region コンストラクタ
    /// <summary>
    /// オブジェクトプールを生成するコンストラクタ
    /// <br>- newした段階でInstantiateが走るため注意</br>
    /// <br>- newの戻り値はIGettablePoolインターフェースで受け取ること</br>
    /// </summary>
    public ObjectPooler(PoolObjectAsset createObjectData, string parentName = null, Transform setParent = null)
    {
        prefab = createObjectData.Prefab;
        objectPool = new();

        // 親オブジェクトを生成
        if (parentName is null)
        {
            parentName = "PooledObjects";
        }
        parent = new GameObject(parentName);

        // 親オブジェクトを設定
        if (setParent is not null)
        {
            parent.transform.SetParent(setParent);
        }

        // 初期生成
        for (int i = 0; i < createObjectData.MaxCreateCount; i++)
        {
            PoolObject obj = Object.Instantiate(prefab, parent.transform);
            obj.Pool = this;            // ObjectPoolerクラスのインスタンスをSetする
            obj.IsInitialCreate = true;
            obj.Initialize();           // 生成したオブジェクトの初期化処理を呼び出す
            objectPool.Enqueue(obj);
        }
    }

    /// <summary>
    /// オブジェクトプールを生成するコンストラクタ
    /// <br>- newした段階でInstantiateが走るため注意</br>
    /// <br>- newの戻り値はIGettablePoolインターフェースで受け取ること</br>
    /// </summary>
    public ObjectPooler(PoolObjectAsset createObjectData, Action initializeAction, string parentName = null, Transform setParent = null)
    {
        prefab = createObjectData.Prefab;
        objectPool = new();

        // 親オブジェクトを生成
        if (parentName is null)
        {
            parentName = "PooledObjects";
        }
        parent = new GameObject(parentName);

        // 親オブジェクトを設定
        if (setParent is not null)
        {
            parent.transform.SetParent(setParent);
        }

        // 初期生成
        for (int i = 0; i < createObjectData.MaxCreateCount; i++)
        {
            PoolObject obj = Object.Instantiate(prefab, parent.transform);
            obj.Pool = this;            // ObjectPoolerクラスのインスタンスをSetする
            obj.IsInitialCreate = true;
            obj.Initialize(initializeAction);           // 生成したオブジェクトの初期化処理を呼び出す
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
        // キューの中身が空であれば、新たに生成する
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
        // キューの中身が空であれば、新たに生成する
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
/// オブジェクトプールを生成するクラス
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
    /// オブジェクトプールを生成するコンストラクタ
    /// <br>- newした段階でInstantiateが走るため注意</br>
    /// <br>- newの戻り値はIGettablePoolインターフェースで受け取ること</br>
    /// </summary>
    public ObjectPooler(PoolObjectAsset createObjectData, Action<T> initializeAction, T t, string parentName = null)
    {
        prefab = createObjectData.Prefab;
        objectPool = new();
        this.initializeAction = initializeAction;
        this.t = t;

        // 親オブジェクトを生成
        if (parentName is null)
        {
            parentName = "PooledObjects";
        }
        parent = new GameObject(parentName);

        // 初期生成
        for (int i = 0; i < createObjectData.MaxCreateCount; i++)
        {
            PoolObject obj = Object.Instantiate(prefab, parent.transform);
            obj.Pool = this;            // ObjectPoolerクラスのインスタンスをSetする
            obj.IsInitialCreate = true;
            obj.Initialize(initializeAction, t);           // 生成したオブジェクトの初期化処理を呼び出す
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
        // キューの中身が空であれば、新たに生成する
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
        // キューの中身が空であれば、新たに生成する
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
    /// オブジェクトプールを生成するコンストラクタ
    /// <br>- newした段階でInstantiateが走るため注意</br>
    /// <br>- newの戻り値はIGettablePoolインターフェースで受け取ること</br>
    /// </summary>
    public ParticlePooler(PoolParticleAsset createEfectData, string parentName = null, Transform setParent = null)
    {
        particle = createEfectData.Partilce;
        objectPool = new();

        // 親オブジェクトを生成
        if (parentName is null)
        {
            parentName = "PooledObjects";
        }
        parent = new GameObject(parentName);

        // 親オブジェクトを設定
        if (setParent is not null)
        {
            parent.transform.SetParent(setParent);
        }

        // 初期生成
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
        // キューの中身が空であれば、新たに生成する
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
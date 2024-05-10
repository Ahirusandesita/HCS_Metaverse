using System;
using UnityEngine;

/// <summary>
/// プールオブジェクト（プーリングされるオブジェクト）の基底クラス
/// </summary>
public abstract class PoolObject : MonoBehaviour
{
    protected Transform myTransform = default;


    protected virtual void Awake()
    {
        myTransform = this.transform;
    }

    /// <summary>
    /// 当オブジェクトの生成と同時に、呼び出し元からPoolの参照がセットされる
    /// <br>- 派生クラスからPool.Return()を使用して返却すること</br>
    /// <br>- Setterは使用しないこと</br>
    /// </summary>
    public IReturnablePool Pool { get; set; }

    /// <summary>
    /// 初期生成されたインスタンスかどうか
    /// </summary>
    public bool IsInitialCreate { get; set; } = false;

    /// <summary>
    /// 初期生成の直後に呼ばれる処理
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
    /// Dequeue(Get)された直後に呼ばれる処理
    /// <br>base: 初期位置と初期角度の代入、非アクティブ化</br>
    /// </summary>
    public virtual void Enable(Vector2 initialPos, Quaternion initialDir)
    {
        myTransform.position = initialPos;
        myTransform.rotation = initialDir;
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Enqueue(Return)される直前に呼ばれる処理
    /// <br>base: 非アクティブ化</br>
    /// </summary>
    public virtual void Disable()
    {
        this.gameObject.SetActive(false);
    }
}

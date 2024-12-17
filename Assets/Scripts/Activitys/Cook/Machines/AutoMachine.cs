using System;
using UnityEngine;
using Fusion;
using Oculus.Interaction;

public class AutoMachine : Machine, IObjectLocker
{
    [SerializeField, Tooltip("行う加工の種類")]
    private ProcessingType _processingType = default;

    [SerializeField, Tooltip("オブジェクトの取得範囲を指定するCollider")]
    private Collider _cuttingAreaCollider = default;

    // 
    private IngrodientCatcher _ingrodientCatcher = default;

    // 固定しているオブジェクトのPuttable
    private Puttable _processingPuttable = default;

    // 掴んだ時や離した時にイベントを実行するクラス
    private PointableUnityEventWrapper _pointableUnityEventWrapper;

    // 
    public Transform GetObjectLockTransform => _machineTransform;

    protected override void Start()
    {
        // 親の処理を実行する
        base.Start();

        // IngrodientCatcherのインスタンスを生成する
        _ingrodientCatcher = new IngrodientCatcher();
    }

    protected override void Update()
    {
        base.Update();

        // processingIngrodientを設定する
        ProcessingIngrodientSetting();

        // 常に加工を進めていく
        //bool isEndProcessing = ProcessingAction(_processingType, Time.deltaTime);
    }

    public void Select()
    {
        RPC_Select();
    }

    [Rpc]
    private void RPC_Select() //RPC
    {
        // 
        if (_processingPuttable is not null)
        {
            // 
            _processingPuttable.DestroyThis();
        }
    }

    private void ProcessingIngrodientSetting()
    {
        // すでにprocessingIngrodientが設定されていた場合
        //if (_processingIngrodient != default)
        //{
        //    // 何もしない
        //    return;
        //}

        // 指定した判定に接触したIngrodientがあった場合
        if (_ingrodientCatcher.SearchIngrodient(_cuttingAreaCollider.bounds.center, _cuttingAreaCollider.bounds.size / 2, transform.rotation, out NetworkObject hitObject))
        {
            // processingIngrodientを設定する
            //RPC_HitIngrodients(hitObject);

            // 処理を終了する
            return;
        }
    }

    /// <summary>
    /// 食材に当たったときの処理
    /// </summary>
    /// <param name="hitObject">当たった食材のNetworkObject</param>
    [Rpc]
    private void RPC_HitIngrodients(GameObject hitObject) // RPC
    {
        // 
        

        // 当たったオブジェクトのIngrodientを取得する
        //_processingIngrodient = hitObject.GetComponent<Ingrodients>();

        // 当たったオブジェクトにPuttableを追加して取得する
        _processingPuttable = hitObject.gameObject.AddComponent<Puttable>();

        // Puttableに自身を渡す
        _processingPuttable.SetLockedCuttingObject(this);

        // 
        _pointableUnityEventWrapper = hitObject.GetComponentInChildren<PointableUnityEventWrapper>();
        _pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });

        Debug.Log($"<color=green>{gameObject.name}</color>　が　<color=blue>{hitObject.name}</color>　を固定");
    }

    public void CanselLock()
    {
        // processingIngrodientを初期化する
        //_processingIngrodient = default;

        // Selectの登録を解除する
        _pointableUnityEventWrapper.WhenSelect.RemoveListener((action) => { Select(); });
    }
}
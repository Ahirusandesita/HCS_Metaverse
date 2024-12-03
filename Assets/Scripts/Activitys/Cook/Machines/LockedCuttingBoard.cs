using UnityEngine;
using Oculus.Interaction;
using Fusion;

public class LockedCuttingBoard : Machine, IObjectLocker, IManualProcessing, IStopper
{
    [SerializeField, Tooltip("オブジェクトの取得範囲を指定するCollider")]
    private Collider _cuttingAreaCollider = default;

    [Tooltip("行う加工の種類")]
    private ProcessingType _processingType = ProcessingType.Cut;

    // processingEvent１回あたりの作業進行度
    private int _processingValue = 1;

    // オブジェクトの取得範囲を表す値 -----------------------
    // 中心
    private Vector3 _hitBoxCenter = default;

    // サイズ
    private Vector3 _hitBoxSize = default;

    // 角度
    private Quaternion _hitBoxRotation = default;
    // -------------------------------------------------

    // オブジェクトを固定しているかどうか
    private bool _isLockedObject = default;

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

        // オブジェクトの取得範囲の各値を設定する
        // 中心
        _hitBoxCenter = _cuttingAreaCollider.bounds.center;

        // サイズ
        _hitBoxSize = _cuttingAreaCollider.bounds.size / 2;

        // 角度
        _hitBoxRotation = this.transform.rotation;

        // IngrodientCatcherのインスタンスを生成する
        _ingrodientCatcher = new IngrodientCatcher();
    }

    private void Update()
    {
        // オブジェクトを固定している場合
        if (_isLockedObject)
        {
            // 何もしない
            return;
        }

        // 指定した判定に接触したIngrodientがないか判定する
        bool isHitIngrodient = _ingrodientCatcher.SearchIngrodient(_hitBoxCenter, _hitBoxSize, _hitBoxRotation, out NetworkObject hitObject);

        // Ingrodientと当たった場合
        if (isHitIngrodient)
        {
            // 食材に当たったときの処理を行う
            RPC_HitIngrodients(hitObject);
        }  
    }

    public void ProcessingEvent()
    {
        // オブジェクトが固定されている　かつ　固定されているオブジェクトにIngrodientがついている場合
        if (_isLockedObject && _processingIngrodient != default)
        {
            // 加工を進める
            bool isEndProcessing = ProcessingAction(_processingType, _processingValue, out Commodity createdCommodity);

            // 加工が完了した場合
            if (isEndProcessing)
            {
                // 
                _processingPuttable.DestroyThis();

                // 
                _processingPuttable = createdCommodity.gameObject.AddComponent<Puttable>();

                // 
                if (createdCommodity.gameObject.TryGetComponent<Ingrodients>(out Ingrodients ingrodients))
                {
                    // 
                    _processingIngrodient = ingrodients;
                }
                else
                {
                    // 
                    _processingIngrodient = default;
                }
            }
        }
    }

    /// <summary>
    /// 食材に当たったときの処理
    /// </summary>
    /// <param name="hitObject">当たった食材のNetworkObject</param>
    [Rpc]
    private void RPC_HitIngrodients(NetworkObject hitObject) // RPC
    {
        // 当たったオブジェクトのIngrodientを取得する
        _processingIngrodient = hitObject.GetComponent<Ingrodients>();

        // オブジェクトを固定している状態にする
        _isLockedObject = true;

        // 当たったオブジェクトにPuttableを追加して取得する
        _processingPuttable = hitObject.gameObject.AddComponent<Puttable>();

        // Puttableに自身を渡す
        _processingPuttable.SetLockedCuttingObject(this);

        // 
        _pointableUnityEventWrapper = hitObject.GetComponentInChildren<PointableUnityEventWrapper>();
        _pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });

        Debug.LogWarning($"<color=green>{gameObject.name}</color>　が　<color=blue>{hitObject.name}</color>　を固定");
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

    public void Select()
    {
        RPC_Select();
    }

    public void CanselLock()
    {
        // 
        _isLockedObject = false;
        _pointableUnityEventWrapper.WhenSelect.RemoveListener((action) => { Select(); });
    }
}
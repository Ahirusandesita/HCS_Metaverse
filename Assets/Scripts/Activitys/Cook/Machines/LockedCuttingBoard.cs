using UnityEngine;
using Oculus.Interaction;
using Fusion;

public class LockedCuttingBoard : StopperObject, ILockedObjectBoard
{
    [SerializeField, Tooltip("オブジェクトの取得範囲を指定するCollider")]
    private Collider _cuttingAreaCollider = default;

    [SerializeField, Tooltip("固定位置")]
    private Transform _machineTransform = default;

    // オブジェクトの取得範囲を表す値 -----------------------
    // 中心
    private Vector3 _hitBoxCenter = default;

    // サイズ
    private Vector3 _hitBoxSize = default;

    // 角度
    private Quaternion _hitBoxRotation = default;
    // -------------------------------------------------

    // GrabbableのActiveを切り替えるための変数
    private ISwitchableGrabbableActive _grabbableActiveSwicher = default;

    // オブジェクトを固定しているかどうか
    private bool _isLockedObject = default;

    // 固定しているオブジェクトのIngrodients
    private Ingrodients _lockingIngrodients = default;

    // 固定しているオブジェクトのPuttable
    private Puttable _lockedPuttable = default;

    // 
    private IPracticableRPCEvent _practicableRPCEvent = default;

    // 
    private LockedCuttingBoard _parentLockedCuttingObject = default;

    // 掴んだ時や離した時にイベントを実行するクラス
    private PointableUnityEventWrapper _pointableUnityEventWrapper;

    // 
    public Transform GetObjectLockTransform => _machineTransform;

    // 
    private NetworkObject myNetwork = default;

    private void Start()
    {
        // オブジェクトの取得範囲の各値を設定する
        // 中心
        _hitBoxCenter = _cuttingAreaCollider.bounds.center;

        // サイズ
        _hitBoxSize = _cuttingAreaCollider.bounds.size / 2;

        // 角度
        _hitBoxRotation = this.transform.rotation;

        // 
        myNetwork = GetComponent<NetworkObject>();
    }

    private void Update()
    {
        // オブジェクトを固定している場合
        if (_isLockedObject)
        {
            // 何もしない
            return;
        }

        // オブジェクトの取得範囲を形成して接触しているColliderを取得する
        Collider[] hitColliders = Physics.OverlapBox(_hitBoxCenter, _hitBoxSize, _hitBoxRotation);

        // 何も当たっていなかった場合
        if (hitColliders is null)
        {
            // 何もしない
            Debug.Log($"なにも当たってないよん");
            return;
        }

        // 範囲内のオブジェクトをすべて探索する
        foreach (Collider hitCollider in hitColliders)
        {
            // NetworkObjectを持たない場合 または 移動権限を持たない場合
            if (!hitCollider.transform.root.TryGetComponent<NetworkObject>(out var network) || !network.HasStateAuthority)
            {
                // 次のオブジェクトに移る
                continue;
            }

            // Ingrodientsがついていた場合
            if (hitCollider.transform.root.TryGetComponent<Ingrodients>(out var _))
            {
                // RigidbodyのKinematicがついている場合
                if (hitCollider.GetComponent<Rigidbody>().isKinematic)
                {
                    // 次のオブジェクトに移る
                    continue;
                }

                // 固定するオブジェクトを取得する
                NetworkObject lockObject = hitCollider.transform.root.GetComponent<NetworkObject>();

                // 食材に当たったときの処理を行う
                RPC_HitIngrodients(lockObject);
            }
        }
    }

    public override void KnifeHitEvent()
    {
        // オブジェクトが固定されている　かつ　固定されているオブジェクトにIngrodientがついている場合
        if (_isLockedObject && _lockingIngrodients is not null)
        {
            // 切断フラグを立てる
            bool isEndCut = _lockingIngrodients.SubToIngrodientsDetailInformationsTimeItTakes(ProcessingType.Cut, 1);

            // 切断グラグを立っていなかった
            if (isEndCut)
            {
                // 当たったオブジェクトのCommodityを取得しておく
                Commodity processedCommodity = default;

                // 
                processedCommodity = _lockingIngrodients.ProcessingStart(ProcessingType.Cut, _machineTransform);

                // 
                _lockedPuttable.DestroyThis();

                // 
                _lockedPuttable = processedCommodity.gameObject.AddComponent<Puttable>();

                // 
                if (processedCommodity.gameObject.TryGetComponent<Ingrodients>(out Ingrodients ingrodients))
                {
                    // 
                    _lockingIngrodients = ingrodients;
                }
                else
                {
                    // 
                    _lockingIngrodients = null;
                }
            }
        }
    }

    /// <summary>
    /// 食材に当たったときの処理
    /// </summary>
    /// <param name="lockObject">当たった食材のNetworkObject</param>
    [Rpc]
    private void RPC_HitIngrodients(NetworkObject lockObject) // RPC
    {
        // 固定するオブジェクトのIngrodientを取得する
        _lockingIngrodients = lockObject.GetComponent<Ingrodients>();

        // オブジェクトを固定している状態にする
        _isLockedObject = true;

        // 固定しているオブジェクトにPuttableを追加して取得する
        _lockedPuttable = lockObject.gameObject.AddComponent<Puttable>();

        // Puttableに自身を渡す
        _lockedPuttable.SetLockedCuttingObject(this);

        // 
        _pointableUnityEventWrapper = lockObject.GetComponentInChildren<PointableUnityEventWrapper>();
        _pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });

        Debug.LogWarning("まな板が" + lockObject.name + "を固定したよ");
    }

    [Rpc]
    private void RPC_Select() //RPC
    {
        // 
        if (_lockedPuttable is not null)
        {
            // 
            _lockedPuttable.DestroyThis();
        }
    }

    public void Select()
    {
        RPC_Select();
    }

    public void CanselCutting()
    {
        // 
        _isLockedObject = false;
        _pointableUnityEventWrapper.WhenSelect.RemoveListener((action) => { Select(); });
    }

    public void Inject(IPracticableRPCEvent practicableRPCEvent)
    {
        _practicableRPCEvent = practicableRPCEvent;
    }
}
using Fusion;
using UnityEngine;
using Oculus.Interaction;
using Cysharp.Threading.Tasks;

public class StoppingKnife : NetworkBehaviour, IStopViewData
{
    [SerializeField, Tooltip("見た目用オブジェクトのTransform")]
    private Transform _visualObjectTransform = default;

    [SerializeField, Tooltip("接触判定用Collider")]
    private Collider _knifeCollider = default;

    [Tooltip("初期位置　離したらここに戻る")]
    private Transform _originTransform = default;

    // 
    private InteractorDetailEventIssuer _detailEventIssuer = default;

    // 
    private HandType _detailEventsHandType = default;

    // 
    private StopData _stopData = default;

    // 
    private Rigidbody _rigidbody = default;

    // 停止するオブジェクトに重なっているかどうかを判定するbool
    private bool _onStopperObject = false;

    // 掴んだ時や離した時にイベントを実行するクラス
    private PointableUnityEventWrapper pointableUnityEventWrapper = default;

    // 
    private LocalView _localView = default;

    /// <summary>
    /// 
    /// </summary>
    public Transform GetVisualObjectTransform => _visualObjectTransform;

    /// <summary>
    /// 
    /// </summary>
    public HandType GetDetailHandType => _detailEventsHandType;

    private void Start()
    {
        // 
        _originTransform = GameObject.Find("KnifeOrigin").transform;

        // 
        _localView = GetComponent<LocalView>();

        // 
        _rigidbody = GetComponent<Rigidbody>();

        // 
        pointableUnityEventWrapper = this.transform.root.GetComponent<PointableUnityEventWrapper>();
        pointableUnityEventWrapper.WhenUnselect.AddListener((action) => { UnSelect(); });

        pointableUnityEventWrapper.WhenSelect.AddListener((data) => GateOfFusion.Instance.Grab(this.GetComponent<NetworkObject>()).Forget());
        pointableUnityEventWrapper.WhenUnselect.AddListener((data) => GateOfFusion.Instance.Release(this.GetComponent<NetworkObject>()));

        // 
        _detailEventIssuer = GameObject.FindObjectOfType<InteractorDetailEventIssuer>();

        // 掴んだ時の情報を講読できるようにする
        _detailEventIssuer.OnInteractor += (handler) =>
        {
            _detailEventsHandType = handler.HandType;
        };
    }

    private void Update()
    {
        // 接触したColliderを判定して格納する
        Collider[] hitColliders = Physics.OverlapBox(_knifeCollider.bounds.center, _knifeCollider.bounds.size / 2, this.transform.rotation);

        // 
        if (_onStopperObject)
        {
            // 接触しているオブジェクトがあった場合
            if (hitColliders is not null)
            {
                // 接触したColliderすべてに判定を行う
                foreach (Collider hitCollider in hitColliders)
                {
                    // Stoppableを持っているオブジェクトがあった場合
                    if (hitCollider.TryGetComponent<IStopper>(out var _))
                    {
                        // 処理を終了
                        return;
                    }
                }
            }

            // 固定を解除
            _localView.NetworkView.GetComponent<NetworkKnife>().RPC_UnLockKnife();

            return;
        }
        // 
        else
        {
            // 接触したColliderがなかった場合
            if (hitColliders is null)
            {
                // なにもしない
                return;
            }

            // 接触したColliderすべてに判定を行う
            foreach (Collider hitCollider in hitColliders)
            {
                // Stoppableを持っていない場合
                if (!hitCollider.TryGetComponent<IStopper>(out var _))
                {
                    // 次のColliderへ
                    continue;
                }

                // 
                NetworkObject networkObject = hitCollider.transform.root.GetComponent<NetworkObject>();

                // 
                RPC_HitBoard(networkObject);
                return;
            }
        }
    }

    public void UnSelect()
    {
        // オブジェクトの固定を解除する
        _localView.NetworkView.GetComponent<NetworkKnife>();
        RPC_UnSelect();
    }

    /// <summary>
    /// Viewオブジェクトの固定を解除するメソッド
    /// </summary>
    private void DestroyStopData()
    {
        // StopDataがある場合
        if (_stopData is not null)
        {
            // StopDataを削除する
            Destroy(_stopData);
        }
    }

    /// <summary>
    /// 停止するオブジェクトに接触したときの処理を行うメソッド
    /// </summary>
    /// <param name="hitObject">接触したオブジェクトのNetworkObject</param>
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    private void RPC_HitBoard(NetworkObject hitObject)
    {
        // フラグを立てる
        _onStopperObject = true;

        // 自身にStopDataをAddして動きを止めれるようにする
        _stopData = gameObject.AddComponent<StopData>();

        // StopDataのセットアップを行う
        _stopData.DataSetUp(this);

        // 接触したオブジェクトが持つ接触時の処理を実行する
        hitObject.GetComponent<IManualProcess>().ManualProcessEvent();
    }

    /// <summary>
    /// オブジェクトの固定を解除する処理
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_UnlockedObject()
    {
        // Viewオブジェクトの固定を解除する
        DestroyStopData();

        // フラグを消す
        _onStopperObject = false;
    }

    /// <summary>
    /// オブジェクトから手を離した時の処理
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_UnSelect()
    {
        // 座標を初期状態に戻す
        this.transform.position = _originTransform.position;

        // 角度を初期状態に戻す
        this.transform.rotation = _originTransform.rotation;

        // Viewオブジェクトの固定を解除する
        DestroyStopData();

        // フラグを消す
        _onStopperObject = false;
        Debug.Log($"<color=yellow>はなしたよん：ほーちょー</color>");
    }
}
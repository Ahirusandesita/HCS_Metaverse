using Fusion;
using UnityEngine;
using Oculus.Interaction;

public class StoppingKnife : NetworkBehaviour, IStopViewData
{
    [SerializeField, Tooltip("見た目用オブジェクトのTransform")]
    private Transform _visualObjectTransform = default;

    [SerializeField, Tooltip("初期位置　離したらここに戻る")]
    private Transform _originTransform = default;

    [SerializeField, Tooltip("接触判定用Collider")]
    private Collider _knifeCollider = default;

    // 
    private InteractorDetailEventIssuer _detailEventIssuer = default;

    // 
    private HandType _detailEventsHandType = default;

    // 
    private StopData _stopData = default;

    // 停止するオブジェクトに重なっているかどうかを判定するbool
    private bool _onStopperObject = false;

    // 掴んだ時や離した時にイベントを実行するクラス
    private PointableUnityEventWrapper pointableUnityEventWrapper = default;

    // 
    private NetworkObject _myNetwork = default;

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
        if (_originTransform == null)
        {
            _originTransform = GameObject.Find("KnifeOrigin").transform;
        }

        // 
        pointableUnityEventWrapper = this.transform.root.GetComponent<PointableUnityEventWrapper>();
        pointableUnityEventWrapper.WhenUnselect.AddListener((action) => { UnSelect(); });

        // 
        _detailEventIssuer = GameObject.FindObjectOfType<InteractorDetailEventIssuer>();

        // 掴んだ時の情報を講読できるようにする
        _detailEventIssuer.OnInteractor += (handler) =>
        {
            _detailEventsHandType = handler.HandType;
        };

        // 
        _myNetwork = transform.root.GetComponent<NetworkObject>();
    }

    private void Update()
    {
        Debug.Log("包丁の権限：" + _myNetwork.HasStateAuthority);

        // オブジェクトの操作権限がない場合
        if (!_myNetwork.HasStateAuthority)
        {
            // 処理を中断
            return;
        }

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
            RPC_ReleaseObject();

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
                RPC_HitStopCollider(networkObject);
                return;
            }
        }
    }

    public void UnSelect()
    {
        // 
        RPC_ReleaseObject();
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
    [Rpc]
    private void RPC_HitStopCollider(NetworkObject hitObject)
    {
        // フラグを立てる
        _onStopperObject = true;

        // 自身にStopDataをAddして動きを止めれるようにする
        _stopData = gameObject.AddComponent<StopData>();

        // StopDataのセットアップを行う
        _stopData.DataSetUp(this);

        // 接触したオブジェクトが持つ接触時の処理を実行する
        hitObject.GetComponent<IManualProcessing>().ProcessingEvent();

        Debug.Log($"<color=red>掴んだよん：ほーちょー</color>");
    }

    /// <summary>
    /// オブジェクトから手を離した時の処理
    /// </summary>
    [Rpc]
    private void RPC_ReleaseObject()
    {
        // 座標を初期状態に戻す
        this.transform.position = _originTransform.position;

        // 角度を初期状態に戻す
        this.transform.rotation = _originTransform.rotation;

        // Viewオブジェクトの固定を解除する
        DestroyStopData();

        // フラグを消す
        _onStopperObject = false;

        Debug.Log($"<color=red>離したよん：ほーちょー</color>");
    }
}
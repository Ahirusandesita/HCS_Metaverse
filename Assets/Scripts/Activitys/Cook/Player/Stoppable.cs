using Photon;
using UnityEngine;
using Oculus.Interaction;

public class Stoppable : MonoBehaviour, IAction, IAction<StopperObject>, IStopViewData
{
    [SerializeField, Tooltip("見た目用オブジェクトのTransform")]
    private Transform _visualObjectTransform = default;

    [SerializeField, Tooltip("IStoppingEventを持つGameObject")]
    private GameObject _stoppingEventObject = default;

    [SerializeField, Tooltip("初期位置　離したらここに戻る")]
    private Transform _originTransform = default;

    [SerializeField, Tooltip("接触判定用Collider")]
    private Collider _knifeCollider = default;

    private InteractorDetailEventIssuer _detailEventIssuer = default;

    private HandType _detailEventsHandType = default;

    public Transform GetVisualObjectTransform => _visualObjectTransform;

    public HandType GetDetailHandType => _detailEventsHandType;

    private IKnifeHitEvent _iStoppingEvent = default;

    private StopData _stopData = default;

    // 掴んだ時や離した時にイベントを実行するクラス
    private PointableUnityEventWrapper pointableUnityEventWrapper = default;

    // 
    private StateAuthorityData _stateAuthorityData = default;

    // 
    private IPracticableRPCEvent _practicableRPCEvent = default;

    // 停止するオブジェクトに重なっているかどうかを判定するbool
    private bool _onStopperObject = false;

    private void Start()
    {
        // 
        pointableUnityEventWrapper = this.transform.root.GetComponent<PointableUnityEventWrapper>();
        pointableUnityEventWrapper.WhenUnselect.AddListener((action) => { UnSelect(); });

        // 
        if (_stoppingEventObject.TryGetComponent<IKnifeHitEvent>(out var iStoppingEvent))
        {
            // 
            _iStoppingEvent = iStoppingEvent;
        }
        else
        {
            // 
            Debug.LogError($"<color=green>{this.name} </color>に IStoppingEventが付いていないからバグるよ");
        }

        // 
        _detailEventIssuer = GameObject.FindObjectOfType<InteractorDetailEventIssuer>();

        // 掴んだ時の情報を講読できるようにする
        _detailEventIssuer.OnInteractor += (handler) =>
        {
            _detailEventsHandType = handler.HandType;
        };

        // 
        _stateAuthorityData = transform.root.GetComponent<StateAuthorityData>();
    }

    private void Update()
    {
        if (_stateAuthorityData.IsGrabbable)
        {
            // 接触したColliderを判定して格納する
            Collider[] hitColliders = Physics.OverlapBox(_knifeCollider.bounds.center, _knifeCollider.bounds.size, this.transform.rotation);

            // 
            if (_onStopperObject)
            {
                // 
                if (hitColliders is null)
                {
                    // フラグを消す
                    _onStopperObject = false;

                    // 固定を解除
                    DestroyStopData();

                    return;
                }
            }
            // 
            else
            {
                // 接触したColliderがなかった場合
                if (hitColliders is null)
                {
                    // なにもしない
                    Debug.Log($"なにも当たってないよん");
                    return;
                }

                // 接触したColliderすべてに判定を行う
                foreach (Collider hitCollider in hitColliders)
                {
                    // Stoppableを持っていない場合
                    if (!hitCollider.transform.root.TryGetComponent<StopperObject>(out var stopperObject))
                    {
                        // 次のColliderへ
                        continue;
                    }

                    // 
                    Action(stopperObject);
                    return;
                }
            }
        }
    }

    public void UnSelect()
    {
        // 
        if (_stateAuthorityData.IsGrabbable)
        {
            // 
            Action();
        }
    }

    /// <summary>
    /// 停止するオブジェクトに接触したときの処理を行うメソッド
    /// </summary>
    /// <param name="hitEvent">接触したオブジェクトのIKnifeHitEvent</param>
    private void HitStopCollier(IKnifeHitEvent hitEvent)
    {
        // 
        _stopData = gameObject.AddComponent<StopData>();

        // 
        _stopData.DataSetUp(this);

        // 
        hitEvent.KnifeHitEvent();

        // フラグを立てる
        _onStopperObject = true;
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
    /// オブジェクトから手を離した時の処理
    /// </summary>
    private void ReleaseObject()
    {
        // 座標を初期状態に戻す
        this.transform.position = _originTransform.position;

        // 角度を初期状態に戻す
        this.transform.rotation = _originTransform.rotation;

        // Viewオブジェクトの固定を解除する
        DestroyStopData();

        // フラグを消す
        _onStopperObject = false;
    }

    /// <summary>
    /// UnSelectEvent
    /// </summary>
    public void Action()
    {
        // 手を離した時の処理を行う
        ReleaseObject();
    }

    /// <summary>
    /// CollisionEvent
    /// </summary>
    /// <param name="collisionTransform">接触したオブジェクト</param>
    public void Action(StopperObject stopperObject)
    {
        // 停止するオブジェクトに接触したときの処理を行う
        HitStopCollier(stopperObject);
    }

    public void Inject(IPracticableRPCEvent practicableRPCEvent)
    {
        _practicableRPCEvent = practicableRPCEvent;
    }
}
using Photon;
using UnityEngine;
using Oculus.Interaction;

public class Stoppable : MonoBehaviour, IAction, IAction<GameObject>
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

    [HideInInspector]
    public Transform GetVisualObjectTransform => _visualObjectTransform;

    [HideInInspector]
    public HandType GetDetailHandType => _detailEventsHandType;

    private IKnifeHitEvent _iStoppingEvent = default;

    [HideInInspector]
    // 
    public StopData _stopData = default;

    // 掴んだ時や離した時にイベントを実行するクラス
    private PointableUnityEventWrapper pointableUnityEventWrapper = default;

    // 
    private StateAuthorityData _stateAuthorityData = default;

    // 
    private IPracticableRPCEvent _practicableRPCEvent = default;

    // 
    private bool _onStopperObject = false;

    public void StoppingEvent()
    {
        if (_iStoppingEvent != default)
        {
            // 
            _iStoppingEvent.KnifeHitEvent();
        }
    }

    public void UnSelect()
    {
        if (_stopData is not null)
        {
            // 
            Destroy(_stopData);
        }

        // 
        this.transform.position = _originTransform.position;
        this.transform.rotation = _originTransform.rotation;

        // 
        _visualObjectTransform.localPosition = default;
        _visualObjectTransform.localRotation = Quaternion.Euler(0f, 90f, 0f);
    }

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
                    if (!hitCollider.transform.root.TryGetComponent<Stoppable>(out var stoppable))
                    {
                        // 次のColliderへ
                        continue;
                    }

                    HitStopCollier();
                    return;
                }
            }
            else
            {
                if (hitColliders is null)
                {
                    // 
                    _onStopperObject = false;

                    // 解除

                    return;
                }
            }
        }
    }

    private void HitStopCollier()
    {

    }

    /// <summary>
    /// UnSelectEvent
    /// </summary>
    public void Action()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// CollisionEvent
    /// </summary>
    /// <param name="collisionObject">接触したオブジェクト</param>
    public void Action(GameObject collisionObject)
    {
        ;
    }

    public void Inject(IPracticableRPCEvent practicableRPCEvent)
    {
        _practicableRPCEvent = practicableRPCEvent;
    }
}
using Photon;
using UnityEngine;
using Oculus.Interaction;

public class Stoppable : MonoBehaviour, IAction, IAction<GameObject>
{
    [SerializeField, Tooltip("�����ڗp�I�u�W�F�N�g��Transform")]
    private Transform _visualObjectTransform = default;

    [SerializeField, Tooltip("IStoppingEvent������GameObject")]
    private GameObject _stoppingEventObject = default;

    [SerializeField, Tooltip("�����ʒu�@�������炱���ɖ߂�")]
    private Transform _originTransform = default;

    [SerializeField, Tooltip("�ڐG����pCollider")]
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

    // �͂񂾎��◣�������ɃC�x���g�����s����N���X
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
            Debug.LogError($"<color=green>{this.name} </color>�� IStoppingEvent���t���Ă��Ȃ�����o�O���");
        }

        // 
        _detailEventIssuer = GameObject.FindObjectOfType<InteractorDetailEventIssuer>();

        // �͂񂾎��̏����u�ǂł���悤�ɂ���
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
            // �ڐG����Collider�𔻒肵�Ċi�[����
            Collider[] hitColliders = Physics.OverlapBox(_knifeCollider.bounds.center, _knifeCollider.bounds.size, this.transform.rotation);

            // 
            if (_onStopperObject)
            {
                // �ڐG����Collider���Ȃ������ꍇ
                if (hitColliders is null)
                {
                    // �Ȃɂ����Ȃ�
                    Debug.Log($"�Ȃɂ��������ĂȂ����");
                    return;
                }

                // �ڐG����Collider���ׂĂɔ�����s��
                foreach (Collider hitCollider in hitColliders)
                {
                    // Stoppable�������Ă��Ȃ��ꍇ
                    if (!hitCollider.transform.root.TryGetComponent<Stoppable>(out var stoppable))
                    {
                        // ����Collider��
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

                    // ����

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
    /// <param name="collisionObject">�ڐG�����I�u�W�F�N�g</param>
    public void Action(GameObject collisionObject)
    {
        ;
    }

    public void Inject(IPracticableRPCEvent practicableRPCEvent)
    {
        _practicableRPCEvent = practicableRPCEvent;
    }
}
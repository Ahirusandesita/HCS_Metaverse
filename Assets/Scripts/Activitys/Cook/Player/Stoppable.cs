using Photon;
using UnityEngine;
using Oculus.Interaction;

public class Stoppable : MonoBehaviour, IAction, IAction<StopperObject>, IStopViewData
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

    public Transform GetVisualObjectTransform => _visualObjectTransform;

    public HandType GetDetailHandType => _detailEventsHandType;

    private IKnifeHitEvent _iStoppingEvent = default;

    private StopData _stopData = default;

    // �͂񂾎��◣�������ɃC�x���g�����s����N���X
    private PointableUnityEventWrapper pointableUnityEventWrapper = default;

    // 
    private StateAuthorityData _stateAuthorityData = default;

    // 
    private IPracticableRPCEvent _practicableRPCEvent = default;

    // ��~����I�u�W�F�N�g�ɏd�Ȃ��Ă��邩�ǂ����𔻒肷��bool
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
                // 
                if (hitColliders is null)
                {
                    // �t���O������
                    _onStopperObject = false;

                    // �Œ������
                    DestroyStopData();

                    return;
                }
            }
            // 
            else
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
                    if (!hitCollider.transform.root.TryGetComponent<StopperObject>(out var stopperObject))
                    {
                        // ����Collider��
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
    /// ��~����I�u�W�F�N�g�ɐڐG�����Ƃ��̏������s�����\�b�h
    /// </summary>
    /// <param name="hitEvent">�ڐG�����I�u�W�F�N�g��IKnifeHitEvent</param>
    private void HitStopCollier(IKnifeHitEvent hitEvent)
    {
        // 
        _stopData = gameObject.AddComponent<StopData>();

        // 
        _stopData.DataSetUp(this);

        // 
        hitEvent.KnifeHitEvent();

        // �t���O�𗧂Ă�
        _onStopperObject = true;
    }

    /// <summary>
    /// View�I�u�W�F�N�g�̌Œ���������郁�\�b�h
    /// </summary>
    private void DestroyStopData()
    {
        // StopData������ꍇ
        if (_stopData is not null)
        {
            // StopData���폜����
            Destroy(_stopData);
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g�����𗣂������̏���
    /// </summary>
    private void ReleaseObject()
    {
        // ���W��������Ԃɖ߂�
        this.transform.position = _originTransform.position;

        // �p�x��������Ԃɖ߂�
        this.transform.rotation = _originTransform.rotation;

        // View�I�u�W�F�N�g�̌Œ����������
        DestroyStopData();

        // �t���O������
        _onStopperObject = false;
    }

    /// <summary>
    /// UnSelectEvent
    /// </summary>
    public void Action()
    {
        // ��𗣂������̏������s��
        ReleaseObject();
    }

    /// <summary>
    /// CollisionEvent
    /// </summary>
    /// <param name="collisionTransform">�ڐG�����I�u�W�F�N�g</param>
    public void Action(StopperObject stopperObject)
    {
        // ��~����I�u�W�F�N�g�ɐڐG�����Ƃ��̏������s��
        HitStopCollier(stopperObject);
    }

    public void Inject(IPracticableRPCEvent practicableRPCEvent)
    {
        _practicableRPCEvent = practicableRPCEvent;
    }
}
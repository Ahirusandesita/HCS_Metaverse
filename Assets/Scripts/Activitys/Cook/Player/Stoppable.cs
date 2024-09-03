using Fusion;
using UnityEngine;
using Oculus.Interaction;

public class Stoppable : NetworkBehaviour, IStopViewData
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
    private NetworkObject _myNetwork = default;

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
        _myNetwork = transform.root.GetComponent<NetworkObject>();
    }

    private void Update()
    {
        if (_myNetwork.HasStateAuthority)
        {
            // �ڐG����Collider�𔻒肵�Ċi�[����
            Collider[] hitColliders = Physics.OverlapBox(_knifeCollider.bounds.center, _knifeCollider.bounds.size, this.transform.rotation);

            // 
            if (_onStopperObject)
            {
                // �ڐG���Ă���I�u�W�F�N�g���������ꍇ
                if (hitColliders is not null)
                {
                    // �ڐG����Collider���ׂĂɔ�����s��
                    foreach (Collider hitCollider in hitColliders)
                    {
                        // Stoppable�������Ă���I�u�W�F�N�g���������ꍇ
                        if (hitCollider.gameObject.TryGetComponent<StopperObject>(out var _))
                        {
                            // �������I��
                            return;
                        }
                    }
                }

                // �t���O������
                _onStopperObject = false;

                // �Œ������
                DestroyStopData();

                return;
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
                    if (!hitCollider.TryGetComponent<StopperObject>(out var stopperObject))
                    {
                        Debug.LogError("Stoppable:Collider��" + hitCollider.name);
                        // ����Collider��
                        continue;
                    }

                    // 
                    NetworkObject networkObject = stopperObject.GetComponent<NetworkObject>();

                    // 
                    RPC_HitStopCollider(networkObject);
                    return;
                }
            }
        }
    }

    public void UnSelect()
    {
        // 
        if (_myNetwork.HasStateAuthority)
        {
            // 
            RPC_ReleaseObject();
        }
    }

    /// <summary>
    /// ��~����I�u�W�F�N�g�ɐڐG�����Ƃ��̏������s�����\�b�h
    /// </summary>
    /// <param name="hitEvent">�ڐG�����I�u�W�F�N�g��IKnifeHitEvent</param>
    [Rpc]
    private void RPC_HitStopCollider(NetworkObject hitObject)
    {
        // 
        IKnifeHitEvent hitEvent = hitObject.GetComponent<IKnifeHitEvent>();

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
    [Rpc]
    private void RPC_ReleaseObject()
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


    public void Inject(IPracticableRPCEvent practicableRPCEvent)
    {
        _practicableRPCEvent = practicableRPCEvent;
    }
}
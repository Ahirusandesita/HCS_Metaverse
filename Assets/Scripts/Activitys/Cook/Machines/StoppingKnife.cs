using Fusion;
using UnityEngine;
using Oculus.Interaction;
using Cysharp.Threading.Tasks;

public class StoppingKnife : NetworkBehaviour, IStopViewData
{
    [SerializeField, Tooltip("�����ڗp�I�u�W�F�N�g��Transform")]
    private Transform _visualObjectTransform = default;

    [SerializeField, Tooltip("�ڐG����pCollider")]
    private Collider _knifeCollider = default;

    [Tooltip("�����ʒu�@�������炱���ɖ߂�")]
    private Transform _originTransform = default;

    // 
    private InteractorDetailEventIssuer _detailEventIssuer = default;

    // 
    private HandType _detailEventsHandType = default;

    // 
    private StopData _stopData = default;

    // 
    private Rigidbody _rigidbody = default;

    // ��~����I�u�W�F�N�g�ɏd�Ȃ��Ă��邩�ǂ����𔻒肷��bool
    private bool _onStopperObject = false;

    // �͂񂾎��◣�������ɃC�x���g�����s����N���X
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

        // �͂񂾎��̏����u�ǂł���悤�ɂ���
        _detailEventIssuer.OnInteractor += (handler) =>
        {
            _detailEventsHandType = handler.HandType;
        };
    }

    private void Update()
    {
        // �ڐG����Collider�𔻒肵�Ċi�[����
        Collider[] hitColliders = Physics.OverlapBox(_knifeCollider.bounds.center, _knifeCollider.bounds.size / 2, this.transform.rotation);

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
                    if (hitCollider.TryGetComponent<IStopper>(out var _))
                    {
                        // �������I��
                        return;
                    }
                }
            }

            // �Œ������
            _localView.NetworkView.GetComponent<NetworkKnife>().RPC_UnLockKnife();

            return;
        }
        // 
        else
        {
            // �ڐG����Collider���Ȃ������ꍇ
            if (hitColliders is null)
            {
                // �Ȃɂ����Ȃ�
                return;
            }

            // �ڐG����Collider���ׂĂɔ�����s��
            foreach (Collider hitCollider in hitColliders)
            {
                // Stoppable�������Ă��Ȃ��ꍇ
                if (!hitCollider.TryGetComponent<IStopper>(out var _))
                {
                    // ����Collider��
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
        // �I�u�W�F�N�g�̌Œ����������
        _localView.NetworkView.GetComponent<NetworkKnife>();
        RPC_UnSelect();
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
    /// ��~����I�u�W�F�N�g�ɐڐG�����Ƃ��̏������s�����\�b�h
    /// </summary>
    /// <param name="hitObject">�ڐG�����I�u�W�F�N�g��NetworkObject</param>
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    private void RPC_HitBoard(NetworkObject hitObject)
    {
        // �t���O�𗧂Ă�
        _onStopperObject = true;

        // ���g��StopData��Add���ē������~�߂��悤�ɂ���
        _stopData = gameObject.AddComponent<StopData>();

        // StopData�̃Z�b�g�A�b�v���s��
        _stopData.DataSetUp(this);

        // �ڐG�����I�u�W�F�N�g�����ڐG���̏��������s����
        hitObject.GetComponent<IManualProcess>().ManualProcessEvent();
    }

    /// <summary>
    /// �I�u�W�F�N�g�̌Œ���������鏈��
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_UnlockedObject()
    {
        // View�I�u�W�F�N�g�̌Œ����������
        DestroyStopData();

        // �t���O������
        _onStopperObject = false;
    }

    /// <summary>
    /// �I�u�W�F�N�g�����𗣂������̏���
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_UnSelect()
    {
        // ���W��������Ԃɖ߂�
        this.transform.position = _originTransform.position;

        // �p�x��������Ԃɖ߂�
        this.transform.rotation = _originTransform.rotation;

        // View�I�u�W�F�N�g�̌Œ����������
        DestroyStopData();

        // �t���O������
        _onStopperObject = false;
        Debug.Log($"<color=yellow>�͂Ȃ������F�ف[����[</color>");
    }
}
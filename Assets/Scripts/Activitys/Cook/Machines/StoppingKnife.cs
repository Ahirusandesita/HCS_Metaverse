using Fusion;
using UnityEngine;
using Oculus.Interaction;

public class StoppingKnife : NetworkBehaviour, IStopViewData
{
    [SerializeField, Tooltip("�����ڗp�I�u�W�F�N�g��Transform")]
    private Transform _visualObjectTransform = default;

    [SerializeField, Tooltip("�����ʒu�@�������炱���ɖ߂�")]
    private Transform _originTransform = default;

    [SerializeField, Tooltip("�ڐG����pCollider")]
    private Collider _knifeCollider = default;

    // 
    private InteractorDetailEventIssuer _detailEventIssuer = default;

    // 
    private HandType _detailEventsHandType = default;

    // 
    private StopData _stopData = default;

    // ��~����I�u�W�F�N�g�ɏd�Ȃ��Ă��邩�ǂ����𔻒肷��bool
    private bool _onStopperObject = false;

    // �͂񂾎��◣�������ɃC�x���g�����s����N���X
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
        Debug.Log("��̌����F" + _myNetwork.HasStateAuthority);

        // �I�u�W�F�N�g�̑��쌠�����Ȃ��ꍇ
        if (!_myNetwork.HasStateAuthority)
        {
            // �����𒆒f
            return;
        }

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
            RPC_ReleaseObject();

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
    [Rpc]
    private void RPC_HitStopCollider(NetworkObject hitObject)
    {
        // �t���O�𗧂Ă�
        _onStopperObject = true;

        // ���g��StopData��Add���ē������~�߂��悤�ɂ���
        _stopData = gameObject.AddComponent<StopData>();

        // StopData�̃Z�b�g�A�b�v���s��
        _stopData.DataSetUp(this);

        // �ڐG�����I�u�W�F�N�g�����ڐG���̏��������s����
        hitObject.GetComponent<IManualProcessing>().ProcessingEvent();

        Debug.Log($"<color=red>�͂񂾂��F�ف[����[</color>");
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

        Debug.Log($"<color=red>���������F�ف[����[</color>");
    }
}
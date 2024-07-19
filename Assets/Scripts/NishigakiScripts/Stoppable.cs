using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class Stoppable : MonoBehaviour
{
    [SerializeField, Tooltip("�����ڗp�I�u�W�F�N�g��Transform")]
    private Transform _visualObjectTransform = default;

    [SerializeField, Tooltip("IStoppingEvent������GameObject")]
    private GameObject _stoppingEventObject = default;

    [SerializeField, Tooltip("�����ʒu�@�������炱���ɖ߂�")]
    private Transform _originTransform = default;

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
    private PointableUnityEventWrapper pointableUnityEventWrapper;

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
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoppable : MonoBehaviour
{
    [SerializeField, Tooltip("�����ڗp�I�u�W�F�N�g��Transform")]
    private Transform _visualObjectTransform = default;

    [SerializeField, Tooltip("IStoppingEvent������GameObject")]
    private GameObject _stoppingEventObject = default;

    private InteractorDetailEventIssuer _detailEventIssuer = default;

    private HandType _detailEventsHandType = default;

    public Transform GetVisualObjectTransform => _visualObjectTransform;

    public HandType GetDetailHandType => _detailEventsHandType;

    private IKnifeHitEvent _iStoppingEvent = default;

    public void StoppingEvent()
    {
        if (_iStoppingEvent != default)
        {
            // 
            _iStoppingEvent.KnifeHitEvent();
        }
    }

    private void Start()
    {
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

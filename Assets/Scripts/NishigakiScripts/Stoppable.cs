using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoppable : MonoBehaviour
{
    [SerializeField, Tooltip("�����ڗp�I�u�W�F�N�g��Transform")]
    private Transform _viewObjectTransform = default;

    [SerializeField, Tooltip("IStoppingEvent������GameObject")]
    private GameObject _stoppingEventObject = default;

    private InteractorDetailEventIssuer _detailEventIssuer = default;

    private HandType _detailEventsHandType = default;

    public Transform GetVisualObjectTransform => _viewObjectTransform;

    public HandType GetDetailHandType => _detailEventsHandType;

    private IStoppingEvent _iStoppingEvent = default;

    public void StoppingEvent()
    {
        if (_iStoppingEvent != default)
        {
            // 
            _iStoppingEvent.StoppingEvent();
        }
    }

    private void Start()
    {
        // 
        if (_stoppingEventObject.TryGetComponent<IStoppingEvent>(out var iStoppingEvent))
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

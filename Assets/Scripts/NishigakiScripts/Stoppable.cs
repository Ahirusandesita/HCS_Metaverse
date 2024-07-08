using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoppable : MonoBehaviour
{
    [SerializeField, Tooltip("�����ڗp�I�u�W�F�N�g��Transform")]
    private Transform _viewObjectTransform = default;

    private InteractorDetailEventIssuer _detailEventIssuer = default;

    private HandType _detailEventsHandType = default;

    public Transform GetVisualObjectTransform => _viewObjectTransform;

    public HandType GetDetailHandType => _detailEventsHandType;

    private void Start()
    {
        // 
        _detailEventIssuer = GameObject.FindObjectOfType<InteractorDetailEventIssuer>();

        // �͂񂾎��̏����u�ǂł���悤�ɂ���
        _detailEventIssuer.OnInteractor += (handler) => {
            _detailEventsHandType = handler.HandType;
        };
    }
}

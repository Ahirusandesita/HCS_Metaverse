using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoppable : MonoBehaviour
{
    [SerializeField, Tooltip("見た目用オブジェクトのTransform")]
    private Transform _viewObjectTransform = default;

    private InteractorDetailEventIssuer _detailEventIssuer = default;

    private HandType _detailEventsHandType = default;

    public Transform GetVisualObjectTransform => _viewObjectTransform;

    public HandType GetDetailHandType => _detailEventsHandType;

    private void Start()
    {
        // 
        _detailEventIssuer = GameObject.FindObjectOfType<InteractorDetailEventIssuer>();

        // 掴んだ時の情報を講読できるようにする
        _detailEventIssuer.OnInteractor += (handler) => {
            _detailEventsHandType = handler.HandType;
        };
    }
}

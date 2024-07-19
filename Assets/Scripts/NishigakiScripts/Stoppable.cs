using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class Stoppable : MonoBehaviour
{
    [SerializeField, Tooltip("見た目用オブジェクトのTransform")]
    private Transform _visualObjectTransform = default;

    [SerializeField, Tooltip("IStoppingEventを持つGameObject")]
    private GameObject _stoppingEventObject = default;

    [SerializeField, Tooltip("初期位置　離したらここに戻る")]
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

    // 掴んだ時や離した時にイベントを実行するクラス
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
            Debug.LogError($"<color=green>{this.name} </color>に IStoppingEventが付いていないからバグるよ");
        }

        // 
        _detailEventIssuer = GameObject.FindObjectOfType<InteractorDetailEventIssuer>();

        // 掴んだ時の情報を講読できるようにする
        _detailEventIssuer.OnInteractor += (handler) =>
        {
            _detailEventsHandType = handler.HandType;
        };
    }
}

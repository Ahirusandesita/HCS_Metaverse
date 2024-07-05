using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeStopper : MonoBehaviour, IDependencyInjector<PlayerVisualHandDependencyInformation>
{
    [SerializeField, Tooltip("停止させる見た目用オブジェクト")]
    private GameObject _visualObject = default;

    [SerializeField, Tooltip("接触する対象のコライダー")]
    private Collider _targetCollider = default;

    [SerializeField, Tooltip("ナイフが当たったときに実行したい処理を持ったオブジェクト")]
    private GameObject _knifeHitEvent = default;

    // 掴んだ時の情報を取得するためのクラスのインスタンス用変数
    private InteractorDetailEventIssuer _detailEventer = default;

    // 
    private PlayerVisualHandDependencyInformation _handVisualInformation = default;

    // 
    private bool _isHitTarget = false;

    // 
    private bool _isGrabbing = false;

    // 
    private HandType _grabbingHandType = default;

    // 停止させるTransform群 ---------------------------------
    private Transform _visualObjectTransform = default;

    private Transform _visualHandTransform = default;

    private Transform _visualControllerTransform = default;

    private Transform _visualControllerHandTransform = default;
    // ------------------------------------------------------

    // 停止する座標群と角度群 --------------------------------
    private Vector3 _visualObjectPosition = default;
    private Quaternion _visualObjectRotation = default;

    private Vector3 _visualHandPosition = default;
    private Quaternion _visualHandRotation = default;

    private Vector3 _visualControllerPosition = default;
    private Quaternion _visualControllerRotation = default;

    private Vector3 _visualControllerHandPosition = default;
    private Quaternion _visualControllerHandRotation = default;
    // ------------------------------------------------------

    private void Start()
    {
        // 掴んだ時の手の方向を講読しておく
        _detailEventer.OnInteractor += (handler) => {_grabbingHandType = handler.HandType;};
    }

    private void Update()
    {
        if (_isGrabbing)
        {
            if (_isHitTarget)
            {
                // 
                LockTransform();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == _targetCollider)
        {
            // 
            _isHitTarget = true;

            // 
            SetLockTransform(_grabbingHandType);

            // 
            IKnifeHitEvent knifeHitEvent;

            // 
            if (_knifeHitEvent.TryGetComponent<IKnifeHitEvent>(out knifeHitEvent))
            {
                // 
                knifeHitEvent.KnifeHitEvent();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == _targetCollider)
        {
            // 
            _isHitTarget = false;
        }
    }
    private void SetDetailHandTransform(HandType detailHand)
    {
        // 
        switch (detailHand)
        {
            case HandType.Right:
                _visualHandTransform = _handVisualInformation.VisualRightHand;
                _visualControllerTransform = _handVisualInformation.VisualRightController;
                _visualControllerHandTransform = _handVisualInformation.VisualRightControllerHand;
                break;

            case HandType.Left:
                _visualHandTransform = _handVisualInformation.VisualRightHand;
                _visualControllerTransform = _handVisualInformation.VisualRightController;
                _visualControllerHandTransform = _handVisualInformation.VisualRightControllerHand;
                break;
        }
    }

    private void SetLockTransform(HandType detailHand)
    {
        // 
        _visualObjectPosition = _visualObject.transform.position;
        _visualObjectRotation = _visualObject.transform.rotation;

        // 
        switch (detailHand)
        {
            case HandType.Right:
                _visualHandPosition = _handVisualInformation.VisualRightHand.position;
                _visualHandRotation = _handVisualInformation.VisualRightHand.rotation;
                _visualControllerPosition = _handVisualInformation.VisualRightController.position;
                _visualControllerRotation = _handVisualInformation.VisualRightController.rotation;
                _visualControllerHandPosition = _handVisualInformation.VisualRightControllerHand.position;
                _visualControllerHandRotation = _handVisualInformation.VisualRightControllerHand.rotation;
                break;

            case HandType.Left:
                _visualHandPosition = _handVisualInformation.VisualLeftHand.position;
                _visualHandRotation = _handVisualInformation.VisualLeftHand.rotation;
                _visualControllerPosition = _handVisualInformation.VisualLeftController.position;
                _visualControllerRotation = _handVisualInformation.VisualLeftController.rotation;
                _visualControllerHandPosition = _handVisualInformation.VisualLeftControllerHand.position;
                _visualControllerHandRotation = _handVisualInformation.VisualLeftControllerHand.rotation;
                break;
        }
    }

    private void LockTransform()
    {
        // 
        _visualObjectTransform.position = _visualObjectPosition;
        _visualObjectTransform.rotation = _visualObjectRotation;
        _visualHandTransform.position = _visualHandPosition;
        _visualHandTransform.rotation = _visualHandRotation;
        _visualControllerTransform.position = _visualControllerPosition;
        _visualControllerTransform.rotation = _visualControllerRotation;
        _visualControllerHandTransform.position = _visualControllerHandPosition;
        _visualControllerHandTransform.rotation = _visualControllerHandRotation;
    }

    private void Select()
    {
        // 
        _isGrabbing = true;

        // 
        SetDetailHandTransform(_grabbingHandType);
    }

    private void UnSelect()
    {
        // 
        _isGrabbing = false;
    }

    public void Inject(PlayerVisualHandDependencyInformation information)
    {
        // 手のView情報を取得しておく
        _handVisualInformation = information;
    }
}

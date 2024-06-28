using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ViewLocker : MonoBehaviour, IDependencyInjector<PlayerVisualHandDependencyInformation>
{
    [SerializeField, Tooltip("見た目用オブジェクト")]
    private GameObject _visualObject = default;

    [SerializeField, Tooltip("見た目を固定するための境界線との判定を行う座標群")]
    private Transform[] _lockPositionCheckerTransforms = new Transform[4];

    [SerializeField, Tooltip("見た目を固定するための境界線の高さ")]
    private float _lockPositionHeight = default;

    // 
    private bool _isGrabbing = false;

    // 
    private InteractorDetailEventIssuer _detailEventer = default;

    // 
    private PlayerVisualHandDependencyInformation _handVisualInformation = default;

    // 
    private bool _isSetTransforms = default;

    // 
    private HandType _detailHandType = default;

    // 
    private HandType _grabbingHandType = default;

    // 
    private Vector3 _lockingvisualObjectPosition = default;
    private Quaternion _lockingVisualObjectRotation = default;

    // 
    private Vector3 _lockingVisualRightHandPosition = default;
    private Quaternion _lockingVisualRightHandRotation = default;

    // 
    private Vector3 _lockingVisualLeftHandPosition = default;
    private Quaternion _lockingVisualLeftHandRotation = default;

    // 
    private Vector3 _lockingVisualRightControllerPosition = default;
    private Quaternion _lockingVisualRightControllerRotation = default;

    // 
    private Vector3 _lockingVisualLeftControllerPosition = default;
    private Quaternion _lockingVisualLeftControllerRotation = default;

    // 
    private Vector3 _lockingVisualRightControllerHandPosition = default;
    private Quaternion _lockingVisualRightControllerHandRotation = default;

    // 
    private Vector3 _lockingVisualLeftControllerHandPosition = default;
    private Quaternion _lockingVisualLeftControllerHandRotation = default;

    private void Start()
    {
        // 掴んだ時の手の方向を講読しておく
        _detailEventer.OnInteractor += (handler) => { _detailHandType = handler.HandType; };
    }

    private void LateUpdate()
    {
        // 
        if (_isGrabbing)
        {
            // 
            if (CheckInLockPosition())
            {
                // 
                if (_isSetTransforms)
                {
                    // 
                    LockingViewTransforms(_grabbingHandType);
                }

                // 
                SetViewParameters();
            }
            // 
            else if(_isSetTransforms)
            {
                // 
                ResetViewParameters();
            }
        }
        // 
        else if(_isSetTransforms)
        {
            // 
            ResetViewParameters();
        }
    }

    public void Select()
    {
        // 
        _isGrabbing = true;

        // 
        _grabbingHandType = _detailHandType;
    }

    public void UnSelect()
    {
        // 
        _isGrabbing = false;
    }

    private void SetViewParameters()
    {
        // 
        _lockingvisualObjectPosition = _visualObject.transform.position;
        _lockingVisualObjectRotation = _visualObject.transform.rotation;

        // 
        switch (_grabbingHandType)
        {
            case HandType.Right:
                // 
                _lockingVisualRightHandPosition = _handVisualInformation.VisualRightHand.transform.position;
                _lockingVisualRightHandRotation = _handVisualInformation.VisualRightHand.transform.rotation;

                // 
                _lockingVisualRightControllerPosition = _handVisualInformation.VisualRightController.transform.position;
                _lockingVisualRightControllerRotation = _handVisualInformation.VisualRightController.transform.rotation;

                // 
                _lockingVisualRightControllerHandPosition = _handVisualInformation.VisualRightControllerHand.transform.position;
                _lockingVisualRightControllerHandRotation = _handVisualInformation.VisualRightControllerHand.transform.rotation;

                break;

            case HandType.Left:
                // 
                _lockingVisualLeftHandPosition = _handVisualInformation.VisualLeftHand.transform.position;
                _lockingVisualLeftHandRotation = _handVisualInformation.VisualLeftHand.transform.rotation;

                // 
                _lockingVisualLeftControllerPosition = _handVisualInformation.VisualLeftController.transform.position;
                _lockingVisualLeftControllerRotation = _handVisualInformation.VisualLeftController.transform.rotation;

                // 
                _lockingVisualLeftControllerHandPosition = _handVisualInformation.VisualLeftControllerHand.transform.position;
                _lockingVisualLeftControllerHandRotation = _handVisualInformation.VisualLeftControllerHand.transform.rotation;

                break;
        }
    }

    private void ResetViewParameters()
    {
        // 
        _lockingvisualObjectPosition = default;
        _lockingVisualObjectRotation = default;

        // 
        _lockingVisualRightHandPosition = default;
        _lockingVisualRightHandRotation = default;

        // 
        _lockingVisualLeftHandPosition = default;
        _lockingVisualLeftHandRotation = default;

        // 
        _lockingVisualRightControllerPosition = default;
        _lockingVisualRightControllerRotation = default;

        // 
        _lockingVisualLeftControllerPosition = default;
        _lockingVisualLeftControllerRotation = default;

        // 
        _lockingVisualRightControllerHandPosition = default;
        _lockingVisualRightControllerHandRotation = default;

        // 
        _lockingVisualLeftControllerHandPosition = default;
        _lockingVisualLeftControllerHandRotation = default;
    }

    private void LockingViewTransforms(HandType grabbingHandType)
    {
        // 
        _visualObject.transform.position = _lockingvisualObjectPosition;
        _visualObject.transform.rotation = _lockingVisualObjectRotation;

        // 
        switch (grabbingHandType)
        {
            case HandType.Right:
                // 
                _handVisualInformation.VisualRightHand.transform.position = _lockingVisualRightHandPosition;
                _handVisualInformation.VisualRightHand.transform.rotation = _lockingVisualRightHandRotation;

                // 
                _handVisualInformation.VisualRightController.transform.position = _lockingVisualRightControllerPosition;
                _handVisualInformation.VisualRightController.transform.rotation = _lockingVisualRightControllerRotation;

                // 
                _handVisualInformation.VisualRightControllerHand.transform.position = _lockingVisualRightControllerHandPosition;
                _handVisualInformation.VisualRightControllerHand.transform.rotation = _lockingVisualRightControllerHandRotation;

                break;

            case HandType.Left:
                // 
                _handVisualInformation.VisualLeftHand.transform.position = _lockingVisualLeftHandPosition;
                _handVisualInformation.VisualLeftHand.transform.rotation = _lockingVisualLeftHandRotation;

                // 
                _handVisualInformation.VisualLeftController.transform.position = _lockingVisualLeftControllerPosition;
                _handVisualInformation.VisualLeftController.transform.rotation = _lockingVisualLeftControllerRotation;

                // 
                _handVisualInformation.VisualLeftControllerHand.transform.position = _lockingVisualLeftControllerHandPosition;
                _handVisualInformation.VisualLeftControllerHand.transform.rotation = _lockingVisualLeftControllerHandRotation;

                break;
        }
    }

    /// <summary>
    /// 見た目を固定するための境界線より下に位置する座標がないか判定するプロパティ
    /// </summary>
    /// <returns>境界線より下の座標があるかないか<br/>trueならある　falseならない</returns>
    private bool CheckInLockPosition()
    {
        // 各座標で判定を行う
        foreach (Transform lockingPositionChecker in _lockPositionCheckerTransforms)
        {
            // 境界線より下にあるかどうか
            if (lockingPositionChecker.position.y <= _lockPositionHeight)
            {
                // 境界線より下の座標があった場合
                return true;
            }
        }

        // すべての座標が境界線より上だった場合
        return false;
    }

    public void Inject(PlayerVisualHandDependencyInformation information)
    {
        // 手のView情報を取得しておく
        _handVisualInformation = information;
    }
}

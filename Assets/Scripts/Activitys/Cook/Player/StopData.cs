using HCSMeta.Activity;
using HCSMeta.Function.Initialize;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopData : MonoBehaviour, IDependencyInjector<PlayerVisualHandDependencyInformation>
{
    // 
    private bool _isEndSetUp = true;

    // 
    private Transform _visualObjectTransform = default;

    // 
    private Vector3 _visualObjectPosition = default;

    // 
    private Quaternion _visualObjectRotation = default;

    // 
    private Vector3 _visualHandObjectPosition = default;

    // 
    private Quaternion _visualHandObjectRotation = default;

    // 
    private Vector3 _visualControllerObjectPosition = default;

    // 
    private Quaternion _visualControllerObjectRotation = default;

    // 
    private Vector3 _visualControllerHandObjectPosition = default;

    // 
    private Quaternion _visualControllerHandObjectRotation = default;

    // 
    private HandType _detailHandType = default;

    // 
    private PlayerVisualHandDependencyInformation _visualInformation = default;

    private void Awake()
    {
        PlayerInitialize.ConsignmentInject_static(this);
    }

    private void LateUpdate()
    {
        // 
        if (_isEndSetUp)
        {
            // 
            _visualObjectTransform.position = _visualObjectPosition;

            // 
            _visualObjectTransform.rotation = _visualObjectRotation;

            // 
            switch (_detailHandType)
            {
                case HandType.Right:
                    // 
                    _visualInformation.VisualRightHand.position = _visualHandObjectPosition;

                    // 
                    _visualInformation.VisualRightHand.rotation = _visualHandObjectRotation;

                    // 
                    _visualInformation.VisualRightController.position = _visualControllerObjectPosition;

                    // 
                    _visualInformation.VisualRightController.rotation = _visualControllerObjectRotation;

                    // 
                    _visualInformation.VisualRightControllerHand.position = _visualControllerHandObjectPosition;

                    // 
                    _visualInformation.VisualRightControllerHand.rotation = _visualControllerHandObjectRotation;

                    Debug.Log($"âEéËé~ÇﬂÇƒÇÈÇÊÇÒ");
                    break;

                case HandType.Left:
                    // 
                    _visualInformation.VisualLeftHand.position = _visualHandObjectPosition;

                    // 
                    _visualInformation.VisualLeftHand.rotation = _visualHandObjectRotation;

                    // 
                    _visualInformation.VisualLeftController.position = _visualControllerObjectPosition;

                    // 
                    _visualInformation.VisualLeftController.rotation = _visualControllerObjectRotation;

                    // 
                    _visualInformation.VisualLeftControllerHand.position = _visualControllerHandObjectPosition;

                    // 
                    _visualInformation.VisualLeftControllerHand.rotation = _visualControllerHandObjectRotation;

                    Debug.Log($"ç∂éËé~ÇﬂÇƒÇÈÇÊÇÒ");
                    break;

                default:

                    return;
            }
        }
    }

    public void DataSetUp(Stoppable stoppable)
    {
        // 
        _visualObjectTransform = stoppable.GetVisualObjectTransform;

        // 
        _visualObjectPosition = _visualObjectTransform.position;

        // 
        _visualObjectRotation = _visualObjectTransform.rotation;

        // 
        _detailHandType = stoppable.GetDetailHandType;

        // 
        stoppable._stopData = this;

        switch (_detailHandType)
        {
            case HandType.Right:
                // 
                _visualHandObjectPosition = _visualInformation.VisualRightHand.position;

                // 
                _visualHandObjectRotation = _visualInformation.VisualRightHand.rotation;

                // 
                _visualControllerObjectPosition = _visualInformation.VisualRightController.position;

                // 
                _visualControllerObjectRotation = _visualInformation.VisualRightController.rotation;

                // 
                _visualControllerHandObjectPosition = _visualInformation.VisualRightControllerHand.position;

                // 
                _visualControllerHandObjectRotation = _visualInformation.VisualRightControllerHand.rotation;
                break;

            case HandType.Left:
                // 
                _visualHandObjectPosition = _visualInformation.VisualLeftHand.position;

                // 
                _visualHandObjectRotation = _visualInformation.VisualLeftHand.rotation;

                // 
                _visualControllerObjectPosition = _visualInformation.VisualLeftController.position;

                // 
                _visualControllerObjectRotation = _visualInformation.VisualLeftController.rotation;

                // 
                _visualControllerHandObjectPosition = _visualInformation.VisualLeftControllerHand.position;

                // 
                _visualControllerHandObjectRotation = _visualInformation.VisualLeftControllerHand.rotation;
                break;

            default:

                return;
        }
    }



    private void OnDestroy()
    {
        // 
        _visualObjectTransform.localPosition = default;
        _visualObjectTransform.localRotation = Quaternion.Euler(0f, 90f, 0f);
    }

    /// <summary>
    /// ê⁄êGîªíËÇê›íËÇ∑ÇÈÇΩÇﬂÇÃSetterÉvÉçÉpÉeÉB
    /// </summary>
    /// <param name="state">ê›íËÇ∑ÇÈstate</param>
    public void StopEnd(bool state)
    {
        // 
        Destroy(this);
    }

    public void Inject(PlayerVisualHandDependencyInformation information)
    {
        _visualInformation = information;
    }
}

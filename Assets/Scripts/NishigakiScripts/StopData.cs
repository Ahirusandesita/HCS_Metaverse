using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopData : MonoBehaviour, IDependencyInjector<PlayerVisualHandDependencyInformation>
{
    // 
    private bool _isHitStopper = default;

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

    private void Start()
    {
        // 
        if (TryGetComponent<Stoppable>(out var stoppable))
        {
            // 
            _visualObjectTransform = stoppable.GetVisualObjectTransform;

            // 
            _visualObjectPosition = _visualObjectTransform.position;

            // 
            _visualObjectRotation = _visualObjectTransform.rotation;

            // 
            _detailHandType = stoppable.GetDetailHandType;

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

                    return ;
            }
        }
        else
        {
            // 
            Destroy(this);
        }
    }

    private void LateUpdate()
    {
        // 
        if (_isHitStopper)
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
                    break;

                default:

                    return;
            }

        }
        else
        {
            // 
            Destroy(this);
        }
    }

    /// <summary>
    /// �ڐG�����ݒ肷�邽�߂�Setter�v���p�e�B
    /// </summary>
    /// <param name="state">�ݒ肷��state</param>
    public void SetIsHitStopper(bool state)
    {
        // �ڐG�����ݒ肷��
        _isHitStopper = state;
    }

    public void Inject(PlayerVisualHandDependencyInformation information)
    {
        _visualInformation = information;
    }
}

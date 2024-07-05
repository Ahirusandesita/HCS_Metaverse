using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeStopper : MonoBehaviour, IDependencyInjector<PlayerVisualHandDependencyInformation>
{
    [SerializeField, Tooltip("��~�����錩���ڗp�I�u�W�F�N�g")]
    private GameObject _visualObject = default;

    [SerializeField, Tooltip("�ڐG����Ώۂ̃R���C�_�[")]
    private Collider _targetCollider = default;

    [SerializeField, Tooltip("�i�C�t�����������Ƃ��Ɏ��s�������������������I�u�W�F�N�g")]
    private GameObject _knifeHitEvent = default;

    // �͂񂾎��̏����擾���邽�߂̃N���X�̃C���X�^���X�p�ϐ�
    private InteractorDetailEventIssuer _detailEventer = default;

    // 
    private PlayerVisualHandDependencyInformation _handVisualInformation = default;

    // 
    private bool _isHitTarget = false;

    // 
    private bool _isGrabbing = false;

    // 
    private HandType _grabbingHandType = default;

    // ��~������Transform�Q ---------------------------------
    private Transform _visualObjectTransform = default;

    private Transform _visualHandTransform = default;

    private Transform _visualControllerTransform = default;

    private Transform _visualControllerHandTransform = default;
    // ------------------------------------------------------

    // ��~������W�Q�Ɗp�x�Q --------------------------------
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
        // �͂񂾎��̎�̕������u�ǂ��Ă���
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
        // ���View�����擾���Ă���
        _handVisualInformation = information;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ViewLocker : MonoBehaviour, IDependencyInjector<PlayerVisualHandDependencyInformation>
{
    [SerializeField, Tooltip("�����ڗp�I�u�W�F�N�g")]
    private GameObject _visualObject = default;

    [SerializeField, Tooltip("�����ڂ��Œ肷�邽�߂̋��E���Ƃ̔�����s�����W�Q")]
    private Transform[] _lockPositionCheckerTransforms = new Transform[4];

    [SerializeField, Tooltip("�����ڂ��Œ肷�邽�߂̋��E���̍���")]
    private float _lockPositionHeight = default;

    // ���ݒ͂܂�Ă��邩�ǂ���
    private bool _isGrabbing = false;

    // �͂񂾎��̏����擾���邽�߂̃N���X�̃C���X�^���X�p�ϐ�
    private InteractorDetailEventIssuer _detailEventer = default;

    // ��̌����ڃI�u�W�F�N�g�̏���n���Ă����N���X�̃C���X�^���X�p�ϐ�
    private PlayerVisualHandDependencyInformation _handVisualInformation = default;

    // ���b�N�p��Transform���ݒ肳��Ă��邩�ǂ���
    private bool _isSetTransforms = default;

    // �͂񂾎�̕���
    private HandType _detailHandType = default;

    // ���ݒ͂�ł����̕���
    private HandType _grabbingHandType = default;

    // ���b�N���錩���ڃI�u�W�F�N�g��Transform
    private Vector3 _lockingvisualObjectPosition = default;
    private Quaternion _lockingVisualObjectRotation = default;

    // �E��̌����ڃI�u�W�F�N�g��Transform
    private Vector3 _lockingVisualRightHandPosition = default;
    private Quaternion _lockingVisualRightHandRotation = default;

    // ����̌����ڃI�u�W�F�N�g��Transform
    private Vector3 _lockingVisualLeftHandPosition = default;
    private Quaternion _lockingVisualLeftHandRotation = default;

    // �E��̃R���g���[���[�̌����ڃI�u�W�F�N�g��Transform
    private Vector3 _lockingVisualRightControllerPosition = default;
    private Quaternion _lockingVisualRightControllerRotation = default;

    // ����̃R���g���[���[�̌����ڃI�u�W�F�N�g�̂�Transform
    private Vector3 _lockingVisualLeftControllerPosition = default;
    private Quaternion _lockingVisualLeftControllerRotation = default;

    // �E��̃R���g���[���[�������Ă���Ƃ��̉E��̌����ڃI�u�W�F�N�g��Transform
    private Vector3 _lockingVisualRightControllerHandPosition = default;
    private Quaternion _lockingVisualRightControllerHandRotation = default;

    // ����̃R���g���[���[�������Ă���Ƃ��̍���̌����ڃI�u�W�F�N�g��Transform
    private Vector3 _lockingVisualLeftControllerHandPosition = default;
    private Quaternion _lockingVisualLeftControllerHandRotation = default;

    private void Start()
    {
        // �͂񂾎��̎�̕������u�ǂ��Ă���
        _detailEventer.OnInteractor += (handler) => { _detailHandType = handler.HandType; };
    }

    private void LateUpdate()
    {
        // �͂܂�Ă��邩�ǂ���
        if (_isGrabbing)
        {
            // ���E���𒴂��Ă��邩�ǂ���
            if (CheckInLockPosition())
            {
                // ���b�N�pTransform���ݒ肳��Ă��邩�ǂ���
                if (_isSetTransforms)
                {
                    // �ݒ肳��Ă����ꍇ�͊eTransform�����b�N�pTransform�ŏ㏑������
                    LockingViewTransforms(_grabbingHandType);

                    // �㏑�����I�������I������
                    return;
                }

                // �ݒ肳��Ă��Ȃ������ꍇ��Transform��ݒ肷��
                SetViewParameters();
            }
            // ���E�����z���Ă��Ȃ� ���� ���b�N�pTransform���ݒ肳��Ă��邩�ǂ���
            else if(_isSetTransforms)
            {
                // �ݒ肳��Ă����珉��������
                ResetViewParameters();
            }
        }
        // �͂܂�Ă��Ȃ� ���� ���b�N�pTransform���ݒ肳��Ă��邩�ǂ���
        else if(_isSetTransforms)
        {
            // �ݒ肳��Ă����珉��������
            ResetViewParameters();
        }
    }

    public void Select()
    {
        // �͂܂�Ă����Ԃɂ���
        _isGrabbing = true;

        // �͂�ł����̕������L�^����
        _grabbingHandType = _detailHandType;
    }

    public void UnSelect()
    {
        // �͂܂�Ă��Ȃ���Ԃɂ���
        _isGrabbing = false;
    }

    /// <summary>
    /// ���݂�Transform���烍�b�N�pTransform��ݒ肷�郁�\�b�h
    /// </summary>
    private void SetViewParameters()
    {
        // ���b�N���錩���ڃI�u�W�F�N�g��Transform���L�^����
        _lockingvisualObjectPosition = _visualObject.transform.position;
        _lockingVisualObjectRotation = _visualObject.transform.rotation;

        // ��̕��������Ƃɕ���
        switch (_grabbingHandType)
        {
            // �E��
            case HandType.Right:
                // �E��̌����ڃI�u�W�F�N�g��Transform���L�^����
                _lockingVisualRightHandPosition = _handVisualInformation.VisualRightHand.transform.position;
                _lockingVisualRightHandRotation = _handVisualInformation.VisualRightHand.transform.rotation;

                // �E��̃R���g���[���[�̌����ڃI�u�W�F�N�g��Transform���L�^����
                _lockingVisualRightControllerPosition = _handVisualInformation.VisualRightController.transform.position;
                _lockingVisualRightControllerRotation = _handVisualInformation.VisualRightController.transform.rotation;

                // �E��̃R���g���[���[�������Ă���Ƃ��̌����ڃI�u�W�F�N�g��Transform���L�^����
                _lockingVisualRightControllerHandPosition = _handVisualInformation.VisualRightControllerHand.transform.position;
                _lockingVisualRightControllerHandRotation = _handVisualInformation.VisualRightControllerHand.transform.rotation;

                break;

            // ����
            case HandType.Left:
                // ����̌����ڃI�u�W�F�N�g��Transform���L�^����
                _lockingVisualLeftHandPosition = _handVisualInformation.VisualLeftHand.transform.position;
                _lockingVisualLeftHandRotation = _handVisualInformation.VisualLeftHand.transform.rotation;

                // ����̃R���g���[���[�̌����ڃI�u�W�F�N�g��Transform���L�^����
                _lockingVisualLeftControllerPosition = _handVisualInformation.VisualLeftController.transform.position;
                _lockingVisualLeftControllerRotation = _handVisualInformation.VisualLeftController.transform.rotation;

                // ����̃R���g���[���[�������Ă���Ƃ��̌����ڃI�u�W�F�N�g��Transform���L�^����
                _lockingVisualLeftControllerHandPosition = _handVisualInformation.VisualLeftControllerHand.transform.position;
                _lockingVisualLeftControllerHandRotation = _handVisualInformation.VisualLeftControllerHand.transform.rotation;

                break;
        }
    }

    /// <summary>
    /// ���݋L�^����Ă��郍�b�N�pTransform�����������郁�\�b�h
    /// </summary>
    private void ResetViewParameters()
    {
        // ���b�N���錩���ڃI�u�W�F�N�g�̃��b�N�pTransform������������
        _lockingvisualObjectPosition = default;
        _lockingVisualObjectRotation = default;

        // �E��̌����ڃI�u�W�F�N�g�̃��b�N�pTransform������������
        _lockingVisualRightHandPosition = default;
        _lockingVisualRightHandRotation = default;

        // ����̌����ڃI�u�W�F�N�g�̃��b�N�pTransform������������
        _lockingVisualLeftHandPosition = default;
        _lockingVisualLeftHandRotation = default;

        // �E��̃R���g���[���[�̌����ڃI�u�W�F�N�g�̃��b�N�pTransform������������
        _lockingVisualRightControllerPosition = default;
        _lockingVisualRightControllerRotation = default;

        // ����̃R���g���[���[�̌����ڃI�u�W�F�N�g�̃��b�N�pTransform������������
        _lockingVisualLeftControllerPosition = default;
        _lockingVisualLeftControllerRotation = default;

        // �E��̃R���g���[���[�������Ă���Ƃ��̌����ڃI�u�W�F�N�g�̃��b�N�pTransform������������
        _lockingVisualRightControllerHandPosition = default;
        _lockingVisualRightControllerHandRotation = default;

        // ����̃R���g���[���[�������Ă���Ƃ��̌����ڃI�u�W�F�N�g�̃��b�N�pTransform������������
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
    /// �����ڂ��Œ肷�邽�߂̋��E����艺�Ɉʒu������W���Ȃ������肷��v���p�e�B
    /// </summary>
    /// <returns>���E����艺�̍��W�����邩�Ȃ���<br/>true�Ȃ炠��@false�Ȃ�Ȃ�</returns>
    private bool CheckInLockPosition()
    {
        // �e���W�Ŕ�����s��
        foreach (Transform lockingPositionChecker in _lockPositionCheckerTransforms)
        {
            // ���E����艺�ɂ��邩�ǂ���
            if (lockingPositionChecker.position.y <= _lockPositionHeight)
            {
                // ���E����艺�̍��W���������ꍇ
                return true;
            }
        }

        // ���ׂĂ̍��W�����E�����ゾ�����ꍇ
        return false;
    }

    public void Inject(PlayerVisualHandDependencyInformation information)
    {
        // ���View�����擾���Ă���
        _handVisualInformation = information;
    }
}

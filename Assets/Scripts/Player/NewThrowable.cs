using System.Collections;
using UniRx;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Oculus.Interaction;
using HCSMeta.Function.Initialize;

namespace HCSMeta.Player.Object
{
    public class NewThrowable : MonoBehaviour, IDependencyInjector<PlayerHandDependencyInfomation>
    {
        [SerializeField, Tooltip("���g������Rigidbody")]
        public Rigidbody _thisRigidbody = default;

        [SerializeField, Tooltip("���g������Transform")]
        public Transform _thisTransform = default;

        [SerializeField, Tooltip("���x�W��")]
        private float _velocityCoefficient = 1f;

        // ���ݒ͂�ł�����Transform
        private Transform _grabbingHandTransform = default;

        // �E���Transform
        private Transform _rightHandTransform = default;

        // �����Transform
        private Transform _leftHandTransform = default;

        // �͂񂾎��ɓn������̕������i�[����ϐ�
        private HandType _detailEventsHandType = default;

        // �g�p����ThrowData���i�[���邽�߂̕ϐ�
        public ThrowData _throwData = default;

        // ���݉E��Œ͂�ł��邩�ǂ���
        private bool _isGrabbingRightHand = false;

        // ���ݍ���Œ͂�ł��邩�ǂ���
        private bool _isGrabbingLeftHand = false;

        // ���񂾏u�Ԃ̏����擾���邽�߂̃N���X
        private InteractorDetailEventIssuer _interactorDetailEventIssuer;

        // �͂񂾎��◣�������ɃC�x���g�����s����N���X
        private PointableUnityEventWrapper pointableUnityEventWrapper;

        // 
        private bool _isSelected = false;

        private void Awake()
        {
            // ThrowData�𐶐�����
            _throwData = new ThrowData(_thisTransform.position);

            pointableUnityEventWrapper = this.GetComponent<PointableUnityEventWrapper>();
            pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });
            pointableUnityEventWrapper.WhenUnselect.AddListener((action) => { UnSelect(); });
            PlayerInitialize.ConsignmentInject_static(this);
        }

        private void Start()
        {
            _interactorDetailEventIssuer = GameObject.FindObjectOfType<InteractorDetailEventIssuer>();
            // �͂񂾎��̏����u�ǂł���悤�ɂ���
            _interactorDetailEventIssuer.OnInteractor += (handler) => {
                if (_isSelected)
                {
                    // �͂񂾎�̕��������ƂɃt���O�𗧂Ă�
                    SetGrabbingHandFlag(handler.HandType, true);

                    // �͂񂾎���Transform�����ݒ͂�ł����Ƃ��ēo�^����
                    _grabbingHandTransform = GetDetailHandsTransform(_detailEventsHandType);

                    // ���̏��������s��
                    _throwData.ReSetThrowData(_grabbingHandTransform.position);

                    // 
                    _isSelected = false;
                }
                Debug.LogError("Yooooo!");
            };
        }

        private void FixedUpdate()
        {
            // �ǂ���̎�ł��͂�ł��Ȃ��ꍇ
            if (!_isGrabbingRightHand && !_isGrabbingLeftHand)
            {
                // �������Ȃ�
                return;
            }

            // ���݂̍��W��ۑ�����
            _throwData.SetOrbitPosition(_grabbingHandTransform.position);
        }

        /// <summary>
        /// ���܂ꂽ�Ƃ��Ɏ��s���郁�\�b�h
        /// </summary>
        public void Select()
        {
            // 
            _isSelected = true;
        }

        /// <summary>
        /// �����ꂽ�Ƃ��Ɏ��s���郁�\�b�h
        /// </summary>
        public void UnSelect()
        {
            // �͂񂾎�̕��������ƂɃt���O������
            SetGrabbingHandFlag(_detailEventsHandType, false);

            // �܂��ǂ��炩�̎�Œ͂ݑ����Ă����ꍇ
            if (_isGrabbingRightHand || _isGrabbingLeftHand)
            {
                // �܂�����ł�����̎��Transform
                Transform nowGrabbingHand = default;

                // �܂��͂�ł�����̎�̏��𔻕ʂ���
                if (_isGrabbingRightHand)
                {
                    // �E���Transform����
                    nowGrabbingHand = GetDetailHandsTransform(HandType.Right);
                }
                else
                {
                    // �����Transform����
                    nowGrabbingHand = GetDetailHandsTransform(HandType.Left);
                }

                // �܂��͂�ł����̏��ŏ��������s��
                _throwData.ReSetThrowData(nowGrabbingHand.position);

                // �����Ȃ��ŏI������
                return;
            }

            // Kinematic�𖳌��ɂ���
            _thisRigidbody.isKinematic = false;

            // �����x�N�g�����擾����
            Vector3 throwVector = _throwData.GetThrowVector() * _velocityCoefficient;

            // 1�t���[����Ƀx�N�g�����㏑������
            StartCoroutine(OverwriteVelocity(throwVector));
        }

        /// <summary>
        /// �͂�ł����̃t���O��ύX���邽�߂̃��\�b�h
        /// </summary>
        /// <param name="handType">��̕���</param>
        /// <param name="setState">�ύX����l</param>
        private void SetGrabbingHandFlag(HandType handType, bool setState)
        {
            // �͂񂾎�̕��������ƂɃt���O��ύX����
            switch (handType)
            {
                // �E��̏ꍇ
                case HandType.Right:
                    // �E��̒͂�ł��邩�ǂ����̃t���O��ύX����
                    _isGrabbingRightHand = setState;
                    break;

                // ����̏ꍇ
                case HandType.Left:
                    // ����̒͂�ł��邩�ǂ����̃t���O��ύX����
                    _isGrabbingLeftHand = setState;
                    break;

                // ��O����
                default:
                    // �������Ȃ�
                    Debug.LogError($"SetGrabbingHandFlag�ɂĎ�̕����ɗ�O�������@��̕����F{handType}");
                    return;
            }
        }

        /// <summary>
        /// �͂񂾎��Transform��Ԃ��v���p�e�B
        /// </summary>
        /// <param name="handType">��̕���</param>
        /// <returns>�͂񂾎��Transform</returns>
        private Transform GetDetailHandsTransform(HandType handType)
        {
            // ��̕��������Ƃɕ���
            switch (handType)
            {
                // �E��̏ꍇ
                case HandType.Right:
                    // �E���Transform��Ԃ�
                    return _rightHandTransform;

                // ����̏ꍇ
                case HandType.Left:
                    // �����Transform��Ԃ�
                    return _leftHandTransform;

                // ��O����
                default:
                    // �����Ԃ��Ȃ�
                    Debug.LogError($"GetDetailHandsTransform�ɂĎ�̕����ɗ�O�������@��̕����F{handType}");
                    return null;
            }
        }

        /// <summary>
        /// �����␳���s�����ǂ����𔻒肷��v���p�e�B
        /// </summary>
        /// <returns></returns>
        private bool DoAimedThrow()
        {
            return false;
        }

        /// <summary>
        /// �������x���㏑�����邽�߂̃R���[�`��
        /// </summary>
        /// <param name="throwVector">�������x</param>
        /// <returns></returns>
        private IEnumerator OverwriteVelocity(Vector3 throwVector)
        {
            // 1�t���[���ҋ@����@1�t���[���ҋ@���Ȃ���OVR�ɏ������
            yield return new WaitForEndOfFrame();

            // �����x�N�g���𑬓x�ɏ㏑������
            _thisRigidbody.velocity = throwVector;
        }

        public void Inject(PlayerHandDependencyInfomation information)
        {
            // ���Transform��o�^����
            _rightHandTransform = information.RightHand;
            _leftHandTransform = information.LeftHand;
        }
    }
}
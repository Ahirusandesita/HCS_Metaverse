using System.Collections;
using UniRx;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Throwable : MonoBehaviour, IDependencyInjector<PlayerHandDependencyInfomation>
{
    [SerializeField, Tooltip("���g������Rigidbody")]
    public Rigidbody _thisRigidbody = default;

    [SerializeField, Tooltip("���g������Transform")]
    public Transform _thisTransform = default;

    // �͂�ł�����Transform
    private Transform _rightHandTransform = default;

    // �͂�ł����̎��
    private HandType _handType = default;

    // �g�p����ThrowData���i�[���邽�߂̕ϐ�
    public ThrowData _throwData = default;

    InteractorDetailEventIssuer _interactorDetailEventIssuer ;

    private void Awake()
    {
        // ThrowData�𐶐�����
        _throwData = new ThrowData(_thisTransform.position);
    }

    private void Start()
    {
        PlayerInitialize.ConsignmentInject_static(this);

        // 
        _interactorDetailEventIssuer.OnInteractor += (handler) => { 
            _handType = handler.HandType; 
            
        };

        // 

    }

    private void FixedUpdate()
    {
        // ����ł��鎞�̂ݎ��s����
        if (_throwData is null)
        {
            // ����ł��Ȃ������牽�����Ȃ�
            return;
        }

        // ���݂̍��W��ۑ�����
        _throwData.SetOrbitPosition(_thisTransform.position);
    }

    /// <summary>
    /// ���܂ꂽ�Ƃ��Ɏ��s���郁�\�b�h
    /// </summary>
    public void Select()
    {
        // 
        Transform grabbingHandTransform = default;

        // �͂񂾎�̎�ނ𔻒f����
        if (_handType == HandType.Right)
        {
            // �E���ݒ肷��
            grabbingHandTransform = _rightHandTransform;
        }
        else if (_handType == HandType.Left)
        {

        }
        else
        {

        }

        // ���̏��������s��
        _throwData.ReSetThrowData(_thisTransform.position);
    }

    /// <summary>
    /// �����ꂽ�Ƃ��Ɏ��s���郁�\�b�h
    /// </summary>
    public void UnSelect()
    {
        // Kinematic�𖳌��ɂ���
        _thisRigidbody.isKinematic = false;

        // �����x�N�g�����擾����
        Vector3 throwVector = _throwData.GetThrowVector();

        // 1�t���[����Ƀx�N�g�����㏑������
        StartCoroutine(OverwriteVelocity(throwVector));
    }

    private IEnumerator OverwriteVelocity(Vector3 throwVector)
    {
        // 1�t���[���ҋ@����
        yield return new WaitForEndOfFrame();

        // �����x�N�g���𑬓x�ɏ㏑������
        _thisRigidbody.velocity = throwVector;
    }

    public void Inject(PlayerHandDependencyInfomation information)
    {
        // 
        _rightHandTransform = information.RightHand.transform;
    }
}

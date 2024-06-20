using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField, Tooltip("���g������Rigidbody")]
    public Rigidbody _thisRigidbody = default;

    [SerializeField, Tooltip("���g������Transform")]
    public Transform _thisTransform = default;

    // �g�p����ThrowData���i�[���邽�߂̕ϐ�
    public ThrowData _throwData = default;

    private void Awake()
    {
        // ThrowData�𐶐�����
        _throwData = new ThrowData(_thisTransform.position);
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
    /// ���܂ꂽ�Ƃ��Ɏ��s���鏈��
    /// </summary>
    public void Select()
    {
        // Kinematic��L���ɂ���
        _thisRigidbody.isKinematic = true;

        // ThrowData�𐶐�
        _throwData = new ThrowData(_thisTransform.position);
    }

    /// <summary>
    /// �����ꂽ�Ƃ��Ɏ��s���鏈��
    /// </summary>
    public void UnSelect()
    {
        // Kinematic�𖳌��ɂ���
        _thisRigidbody.isKinematic = false;

        // �����x�N�g�����擾����
        Vector3 throwVector = _throwData.GetThrowVector();

        // �����x�N�g���𑬓x�ɑ������
        _thisRigidbody.velocity = throwVector;

        Debug.Log($"�ۂ��ׂ낵���[{_thisRigidbody.velocity.ToString("F6")}");

        // �g���I�����ThrowData������
        _throwData = null;
    }
}

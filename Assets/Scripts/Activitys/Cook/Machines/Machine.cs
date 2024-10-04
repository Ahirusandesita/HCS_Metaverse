using System.Collections.Generic;
using UnityEngine;
using Fusion;

public abstract class Machine : NetworkBehaviour
{
    [Tooltip("���H����Ingrodient"), HideInInspector]
    public Ingrodients _processingIngrodient = default;

    [Tooltip("���H���s���Ă���ʒu")]
    public Transform _machineTransform = default;

    [Tooltip("���g��NetworkObject")]
    protected NetworkObject _networkObject = default;
    
    protected virtual void Start()
    {
        // ���g��NetworkObject���擾����
        _networkObject = gameObject.GetComponent<NetworkObject>();
    }

    /// <summary>
    /// ProcessingIngrodient�̉��H���s�����\�b�h
    /// </summary>
    /// <param name="processingType">���H�̎��</param>
    /// <param name="processingValue">���H�̐i�s��</param>
    /// <returns>���H�̊�������</returns>
    public bool ProcessingAction(ProcessingType processingType, float processingValue)
    {
        // processingIngrodient���Ȃ������ꍇ
        if (_processingIngrodient == default)
        {
            // �������I������false��Ԃ�
            return false;
        }

        // ProcessingIngrodient�̉��H��i�߂�
        bool isEndProcessing = _processingIngrodient.SubToIngrodientsDetailInformationsTimeItTakes(processingType, processingValue);

        // ���H���������Ă��邩�ǂ���
        if (isEndProcessing)
        {
            // �I�u�W�F�N�g�̑��쌠��������ꍇ
            if (!_processingIngrodient.GetComponent<NetworkObject>().HasStateAuthority)
            {
                // �I�u�W�F�N�g��ω�������
                _processingIngrodient.ProcessingStart(processingType, _machineTransform);
            }

            // processingIngrodient������������
            _processingIngrodient = default;
        }

        // ���H�̊��������Ԃ�
        return isEndProcessing;
    }

    /// <summary>
    /// ProcessingIngrodient�̉��H���s�����\�b�h out�t��
    /// </summary>
    /// <param name="processingType">���H�̎��</param>
    /// <param name="processingValue">���H�̐i�s��</param>
    /// <param name="createdCommodity">��������Commodity/param>
    /// <returns>���H�̊�������</returns>
    public bool ProcessingAction(ProcessingType processingType, float processingValue, out Commodity createdCommodity)
    {
        // createdCommodity�̏�����
        createdCommodity = default;

        // processingIngrodient���Ȃ������ꍇ
        if (_processingIngrodient == default)
        {
            // �������I������false��Ԃ�
            return false;
        }

        // ProcessingIngrodient�̉��H��i�߂�
        bool isEndProcessing = _processingIngrodient.SubToIngrodientsDetailInformationsTimeItTakes(processingType, processingValue);

        // ���H���������Ă��邩�ǂ���
        if (isEndProcessing)
        {
            // �I�u�W�F�N�g�̑��쌠��������ꍇ
            if (!_processingIngrodient.GetComponent<NetworkObject>().HasStateAuthority)
            {
                // �I�u�W�F�N�g��ω�������
                _processingIngrodient.ProcessingStart(processingType, _machineTransform);
            }
            // processingIngrodient������������
            _processingIngrodient = default;
        }

        // ���H�̊��������Ԃ�
        return isEndProcessing;
    }
}
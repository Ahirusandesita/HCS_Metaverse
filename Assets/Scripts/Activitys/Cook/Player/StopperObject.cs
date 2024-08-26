using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopperObject : MonoBehaviour
{
    [SerializeField, Tooltip("�ڐG������s��Collider")]
    private Collider _stopperColliter = default;

    // �ڐG������s��Collider�̒��S���W
    private Vector3 _hitBoxCenter = default;

    // �ڐG������s��Collider�̑傫��
    private Vector3 _hitBoxSize = default;

    // �ڐG������s��Collider�̊p�x
    private Quaternion _hitBoxRotation = default;

    private void Start()
    {
        // �ڐG������s��Collider�̊e�l���擾����
        _hitBoxCenter = _stopperColliter.bounds.center;
        _hitBoxSize = _stopperColliter.bounds.size / 2;
        _hitBoxRotation = this.transform.rotation;
    }

    private void Update()
    {
        // �ڐG����Collider�𔻒肵�Ċi�[����
        Collider[] hitColliders = Physics.OverlapBox(_hitBoxCenter, _hitBoxSize, _hitBoxRotation);

        // �ڐG����Collider���Ȃ������ꍇ
        if (hitColliders is null)
        {
            // �Ȃɂ����Ȃ�
            Debug.Log($"�Ȃɂ��������ĂȂ����");
            return;
        }

        // �ڐG����Collider���ׂĂɔ�����s��
        foreach (Collider hitCollider in hitColliders)
        {
            // Stoppable�������Ă��Ȃ��ꍇ
            if (!hitCollider.transform.root.TryGetComponent<Stoppable>(out var stoppable))
            {
                // ����Collider��
                continue;
            }

            // StopData�������Ă���ꍇ
            if (hitCollider.transform.root.TryGetComponent<StopData>(out var stopData))
            {
                // StopData�̒�~�t���O�𗧂Ă�
                stopData.SetIsHitStopper(true);
            }
            // StopData�������Ă��Ȃ��ꍇ
            else
            {
                // �ڐG���Ă���I�u�W�F�N�g��StopData��������
                hitCollider.transform.root.gameObject.AddComponent<StopData>();

                // ��~���̏��������s����
                stoppable.StoppingEvent();
            }
        }
    }
}

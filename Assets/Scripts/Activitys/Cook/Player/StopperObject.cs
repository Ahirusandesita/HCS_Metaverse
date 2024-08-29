using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopperObject : MonoBehaviour
{
    [SerializeField, Tooltip("�ڐG������s��Collider")]
    private Collider _knifeCollider = default;

    private void Update()
    {
        // �ڐG����Collider�𔻒肵�Ċi�[����
        Collider[] hitColliders = Physics.OverlapBox(_knifeCollider.bounds.center, _knifeCollider.bounds.size, this.transform.rotation);

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
                stopData.StopEnd(true);
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

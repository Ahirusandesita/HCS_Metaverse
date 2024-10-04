using UnityEngine;
using Fusion;

public class IngrodientCatcher
{
    public bool SearchIngrodient(Vector3 hitBoxCenter, Vector3 hitBoxSize, Quaternion hitBoxRotation, out NetworkObject processingObject)
    {
        // processingObject�̏�����
        processingObject = default;

        // �I�u�W�F�N�g�̎擾�͈͂��`�����ĐڐG���Ă���Collider���擾����
        Collider[] hitColliders = Physics.OverlapBox(hitBoxCenter, hitBoxSize, hitBoxRotation);

        // �����������Ă��Ȃ������ꍇ
        if (hitColliders is null)
        {
            // False��Ԃ��ďI��
            Debug.Log($"�Ȃɂ��������ĂȂ����");
            return false;
        }

        // �͈͓��̃I�u�W�F�N�g�����ׂĒT������
        foreach (Collider hitCollider in hitColliders)
        {
            // NetworkObject�������Ȃ��ꍇ �܂��� �ړ������������Ȃ��ꍇ
            if (!hitCollider.transform.root.TryGetComponent<NetworkObject>(out var network) || !network.HasStateAuthority)
            {
                // ���̃I�u�W�F�N�g�Ɉڂ�
                continue;
            }

            // Ingrodients�����Ă����ꍇ
            if (hitCollider.transform.root.TryGetComponent<Ingrodients>(out var _))
            {
                // Rigidbody��Kinematic�����Ă���ꍇ
                if (hitCollider.transform.root.GetComponent<Rigidbody>().isKinematic)
                {
                    // ���̃I�u�W�F�N�g�Ɉڂ�
                    continue;
                }

                // �Œ肷��I�u�W�F�N�g���擾����
                processingObject = network;

                // Ingrodient�Ɠ�����������True��Ԃ��ďI������
                return true;
            }
        }

        // Ingrodient�Ɠ�����Ȃ���������false��Ԃ��ďI������
        return false;
    }
}

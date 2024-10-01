using UnityEngine;
using Oculus.Interaction;

public class Puttable : MonoBehaviour
{
    // 
    private IObjectLocker _parentLockedCuttingObject = default;

    // �͂񂾎��◣�������ɃC�x���g�����s����N���X
    private PointableUnityEventWrapper _pointableUnityEventWrapper;

    public void SetLockedCuttingObject(IObjectLocker lockedCuttingObject)
    {
        // 
        _parentLockedCuttingObject = lockedCuttingObject;

        // 
        ISwitchableGrabbableActive grabbableActiveSwicher = GetComponent<ISwitchableGrabbableActive>();

        // �Œ肷��I�u�W�F�N�g��Grabbable��false�ɂ���
        grabbableActiveSwicher.Inactive();

        // �Œ肷��I�u�W�F�N�g�̍��W���}�V���̍��W�Ɉړ�������
        transform.position = lockedCuttingObject.GetObjectLockTransform.position;
        transform.rotation = lockedCuttingObject.GetObjectLockTransform.rotation;

        // �Œ肷��I�u�W�F�N�g��Grabbable��true�ɂ���
        grabbableActiveSwicher.Active();
    }

    public void DestroyThis()
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        _parentLockedCuttingObject.CanselLock();
    }
}
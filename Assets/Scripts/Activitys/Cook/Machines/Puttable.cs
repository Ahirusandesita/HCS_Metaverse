using UnityEngine;
using Oculus.Interaction;

public class Puttable : MonoBehaviour,IGrabbableActiveChangeRequester
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

        grabbableActiveSwicher.Regist(this);
        // �Œ肷��I�u�W�F�N�g��Grabbable��false�ɂ���
        grabbableActiveSwicher.Inactive(this);

        // �Œ肷��I�u�W�F�N�g�̍��W���}�V���̍��W�Ɉړ�������
        transform.position = lockedCuttingObject.GetObjectLockTransform.position;
        transform.rotation = lockedCuttingObject.GetObjectLockTransform.rotation;

        // �Œ肷��I�u�W�F�N�g��Grabbable��true�ɂ���
        grabbableActiveSwicher.Active(this);
        grabbableActiveSwicher.Cancellation(this);
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
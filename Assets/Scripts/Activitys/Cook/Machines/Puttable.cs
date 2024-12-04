using UnityEngine;
using Oculus.Interaction;

public class Puttable : MonoBehaviour,IGrabbableActiveChangeRequester
{
    // 
    private IObjectLocker _parentLockedCuttingObject = default;

    // 掴んだ時や離した時にイベントを実行するクラス
    private PointableUnityEventWrapper _pointableUnityEventWrapper;

    public void SetLockedCuttingObject(IObjectLocker lockedCuttingObject)
    {
        // 
        _parentLockedCuttingObject = lockedCuttingObject;

        // 
        ISwitchableGrabbableActive grabbableActiveSwicher = GetComponent<ISwitchableGrabbableActive>();

        grabbableActiveSwicher.Regist(this);
        // 固定するオブジェクトのGrabbableをfalseにする
        grabbableActiveSwicher.Inactive(this);

        // 固定するオブジェクトの座標をマシンの座標に移動させる
        transform.position = lockedCuttingObject.GetObjectLockTransform.position;
        transform.rotation = lockedCuttingObject.GetObjectLockTransform.rotation;

        // 固定するオブジェクトのGrabbableをtrueにする
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
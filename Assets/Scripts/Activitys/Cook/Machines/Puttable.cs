using UnityEngine;
using Oculus.Interaction;

public class Puttable : MonoBehaviour
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

        // 固定するオブジェクトのGrabbableをfalseにする
        grabbableActiveSwicher.Inactive();

        // 固定するオブジェクトの座標をマシンの座標に移動させる
        transform.position = lockedCuttingObject.GetObjectLockTransform.position;
        transform.rotation = lockedCuttingObject.GetObjectLockTransform.rotation;

        // 固定するオブジェクトのGrabbableをtrueにする
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
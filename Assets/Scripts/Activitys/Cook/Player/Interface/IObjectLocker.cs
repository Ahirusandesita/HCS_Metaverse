using UnityEngine;

public interface IObjectLocker
{
    public Transform GetObjectLockTransform {get;}

    public void CanselLock();
}

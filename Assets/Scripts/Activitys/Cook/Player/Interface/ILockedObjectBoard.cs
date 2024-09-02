using UnityEngine;

public interface ILockedObjectBoard
{
    public Transform GetObjectLockTransform {get;}

    public void CanselCutting();
}

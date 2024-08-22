using UnityEngine;
using Oculus.Interaction;

public class Puttable : MonoBehaviour
{
    private LockedCuttingBoard _parentLockedCuttingObject = default;

    // �͂񂾎��◣�������ɃC�x���g�����s����N���X
    private PointableUnityEventWrapper _pointableUnityEventWrapper;

    private void Awake()
    {
        _pointableUnityEventWrapper = this.GetComponentInChildren<PointableUnityEventWrapper>();
        _pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });

    }

    public void SetLockedCuttingObject(LockedCuttingBoard lockedCuttingObject)
    {
        // 
        _parentLockedCuttingObject = lockedCuttingObject;
    }

    public void Select()
    {
        DestroyThis();
    }

    public void DestroyThis()
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        _pointableUnityEventWrapper.WhenSelect.RemoveListener((action) => { Select(); });
        _parentLockedCuttingObject.CanselCutting();
    }
}
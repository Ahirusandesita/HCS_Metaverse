using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using System.Threading.Tasks;

public class Puttable : MonoBehaviour
{
    private LockedCuttingObject _parentLockedCuttingObject = default;

    // 掴んだ時や離した時にイベントを実行するクラス
    private PointableUnityEventWrapper _pointableUnityEventWrapper;

    private void Awake()
    {
        _pointableUnityEventWrapper = this.GetComponentInChildren<PointableUnityEventWrapper>();
        _pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });
       
    }

    public void SetLockedCuttingObject(LockedCuttingObject lockedCuttingObject)
    {
        // 
        _parentLockedCuttingObject = lockedCuttingObject;
    }

    public void Select()
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        _pointableUnityEventWrapper.WhenSelect.RemoveListener((action) => { Select(); });
        _parentLockedCuttingObject.CanselCutting();
    }
}

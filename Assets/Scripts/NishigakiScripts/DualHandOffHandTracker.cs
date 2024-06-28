using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualHandOffHandTracker : MonoBehaviour
{
    [SerializeField, Tooltip("掴まれていないときのTransform")]
    private Transform _stayPointTransform = default;

    // 
    private Transform _offHandTrackerTransform = default;

    // 
    private bool _isGrabbing = false;

    private void Awake()
    {
        // 自身のTransformをキャッシュしておく
        _offHandTrackerTransform = this.transform;
    }

    private void Update()
    {
        // 掴まれていない場合
        if (!_isGrabbing)
        {
            // 掴まれていないときの位置に移動
            _offHandTrackerTransform.position = _stayPointTransform.position;
        }
    }

    public void Select()
    {
        // 
        _isGrabbing = true;
    }

    public void UnSelect()
    {
        // 
        _isGrabbing = false;
    }
}

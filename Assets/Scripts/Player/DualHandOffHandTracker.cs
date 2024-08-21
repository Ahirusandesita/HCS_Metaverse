using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualHandOffHandTracker : MonoBehaviour
{
    [SerializeField, Tooltip("掴まれていないときのTransform")]
    private Transform _stayPointTransform = default;

    // OffHandTrackerのTransformをキャッシュしておく変数　自身のTransformを代入
    private Transform _offHandTrackerTransform = default;

    // 掴んでいるかどうかを判定するbool
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
        // 掴んでいる状態にする
        _isGrabbing = true;
    }

    public void UnSelect()
    {
        // 掴んでいない状態にする
        _isGrabbing = false;
    }
}

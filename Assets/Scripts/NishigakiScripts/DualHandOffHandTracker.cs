using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualHandOffHandTracker : MonoBehaviour
{
    [SerializeField, Tooltip("�͂܂�Ă��Ȃ��Ƃ���Transform")]
    private Transform _stayPointTransform = default;

    // 
    private Transform _offHandTrackerTransform = default;

    // 
    private bool _isGrabbing = false;

    private void Awake()
    {
        // ���g��Transform���L���b�V�����Ă���
        _offHandTrackerTransform = this.transform;
    }

    private void Update()
    {
        // �͂܂�Ă��Ȃ��ꍇ
        if (!_isGrabbing)
        {
            // �͂܂�Ă��Ȃ��Ƃ��̈ʒu�Ɉړ�
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

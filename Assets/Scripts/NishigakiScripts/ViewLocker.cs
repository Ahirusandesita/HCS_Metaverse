using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewLocker : MonoBehaviour
{
    [SerializeField, Tooltip("�����ڗp�I�u�W�F�N�g")]
    private GameObject _viewObject = default;

    [SerializeField, Tooltip("�����ڂ��Œ肷�邽�߂̋��E���Ƃ̔�����s�����W�Q")]
    private Transform[] _lockPositionCheckerTransforms = new Transform[4];

    [SerializeField, Tooltip("�����ڂ��Œ肷�邽�߂̋��E���̍���")]
    private float _lockPositionHeight = default;

    // 
    private bool _isGrabbing = false;

    // 
    private Vector3 _lockingPosition = default;

    void Update()
    {
        // 
        if (_isGrabbing)
        {
            // 
            if (CheckInLockPosition())
            {
                // 
                if (_lockingPosition == default)
                {
                    // 
                    _lockingPosition = _viewObject.transform.position;

                    // 
                    _lockingPosition.y = _lockPositionHeight;
                }

                // 
                _viewObject.transform.position = _lockingPosition;
            }
            // 
            else if(_lockingPosition != default)
            {
                // 
                _lockingPosition = default;
            }
        }
        // 
        else if(_lockingPosition != default)
        {
            // 
            _lockingPosition = default;
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

    /// <summary>
    /// �����ڂ��Œ肷�邽�߂̋��E����艺�Ɉʒu������W���Ȃ������肷��v���p�e�B
    /// </summary>
    /// <returns>���E����艺�̍��W�����邩�Ȃ���<br/>true�Ȃ炠��@false�Ȃ�Ȃ�</returns>
    private bool CheckInLockPosition()
    {
        // �e���W�Ŕ�����s��
        foreach (Transform lockingPositionChecker in _lockPositionCheckerTransforms)
        {
            // ���E����艺�ɂ��邩�ǂ���
            if (lockingPositionChecker.position.y <= _lockPositionHeight)
            {
                // ���E����艺�̍��W���������ꍇ
                return true;
            }
        }

        // ���ׂĂ̍��W�����E�����ゾ�����ꍇ
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewLocker : MonoBehaviour
{
    [SerializeField, Tooltip("見た目用オブジェクト")]
    private GameObject _viewObject = default;

    [SerializeField, Tooltip("見た目を固定するための境界線との判定を行う座標群")]
    private Transform[] _lockPositionCheckerTransforms = new Transform[4];

    [SerializeField, Tooltip("見た目を固定するための境界線の高さ")]
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
    /// 見た目を固定するための境界線より下に位置する座標がないか判定するプロパティ
    /// </summary>
    /// <returns>境界線より下の座標があるかないか<br/>trueならある　falseならない</returns>
    private bool CheckInLockPosition()
    {
        // 各座標で判定を行う
        foreach (Transform lockingPositionChecker in _lockPositionCheckerTransforms)
        {
            // 境界線より下にあるかどうか
            if (lockingPositionChecker.position.y <= _lockPositionHeight)
            {
                // 境界線より下の座標があった場合
                return true;
            }
        }

        // すべての座標が境界線より上だった場合
        return false;
    }
}

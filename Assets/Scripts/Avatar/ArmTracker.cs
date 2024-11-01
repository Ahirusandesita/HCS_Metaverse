using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmTracker : MonoBehaviour
{
    [SerializeField]
    private Transform _armTransform = default;

    [SerializeField]
    private Transform _shoulderTransform = default;

    [SerializeField]
    private Transform _targetTransform = default;

    private Quaternion _armDefaultRotation = default;

    private Quaternion _correctionRotate = default;

    //private void Start()
    //{
    //    _armDefaultRotation = _armTransform.localRotation;
    //}

    //private void Update()
    //{
    //    Vector3 targetVector = (_targetTransform.position - _shoulderTransform.position).normalized;

    //    float angle = Mathf.Acos(Vector3.Dot(_shoulderTransform.right, targetVector)) * Mathf.Rad2Deg;
    //    Vector3 axis = Vector3.Cross(targetVector, _shoulderTransform.right).normalized;

    //    Quaternion rotation = Quaternion.AngleAxis(angle, axis);

    //    _armTransform.localRotation = rotation * _armDefaultRotation;
    //}

    private void Start()
    {
        _armDefaultRotation = _armTransform.localRotation;

        float angle = Mathf.Acos(Vector3.Dot(_shoulderTransform.right, _armTransform.right)) * Mathf.Rad2Deg;
        Vector3 axis = Vector3.Cross(_shoulderTransform.right, _armTransform.right).normalized;

        _correctionRotate = Quaternion.AngleAxis(angle, axis);
    }

    private void Update()
    {
        Vector3 armForwordVector = _correctionRotate * _shoulderTransform.right;

        Vector3 targetVector = (_targetTransform.position - _shoulderTransform.position).normalized;

        float angle = Mathf.Acos(Vector3.Dot(armForwordVector, targetVector)) * Mathf.Rad2Deg;
        Vector3 axis = Vector3.Cross(targetVector, armForwordVector).normalized;

        Quaternion rotation = Quaternion.AngleAxis(angle, axis);

        _armTransform.localRotation = rotation * _armDefaultRotation;
    }
}

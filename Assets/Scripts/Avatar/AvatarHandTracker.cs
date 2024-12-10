using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class AvatarHandTracker
{
    public AvatarHandTracker(NetworkObject rightShoulder, NetworkObject rightHand, NetworkObject leftShoulder, NetworkObject leftHand)
    {
        // 
        _rightShoulder = rightShoulder;
        _rightHand = rightHand;
        _leftShoulder = leftShoulder;
        _leftHand = leftHand;

        // 
        _rightShoulderOriginRotatioin = _rightShoulder.transform.localRotation;
        _leftShoulderOriginRotatioin = _leftShoulder.transform.localRotation;
    }

    // 
    private NetworkObject _rightShoulder = default;

    // 
    private NetworkObject _leftShoulder = default;

    // 
    private NetworkObject _rightHand = default;

    // 
    private NetworkObject _leftHand = default;

    // 
    private Quaternion _rightShoulderOriginRotatioin = default;

    // 
    private Quaternion _leftShoulderOriginRotatioin = default;

    // 
    private float _handComplementAngle = -90;

    public void RightHandTracking(Transform conrtoller)
    {
        // 
        if (!_rightShoulder.HasStateAuthority)
        {
            return;
        }

        // 
        _rightShoulder.transform.localRotation = _rightShoulderOriginRotatioin;

        Debug.Log($"beforeRotation:{_rightShoulder.transform.localRotation.eulerAngles}");

        // 
        Vector3 shoulderForword = _rightShoulder.transform.right;

        // 
        Vector3 shoulderToHandVector = (conrtoller.position - _rightShoulder.transform.position).normalized;

        // 
        Vector3 shoulderAxis = Vector3.Cross(shoulderForword, shoulderToHandVector).normalized;

        // 
        if (Vector3.Dot(shoulderAxis, Vector3.up) < 0)
        {
            // 
            shoulderAxis *= -1;
        }

        // 
        float shoulderAngle = Vector3.SignedAngle(shoulderForword, shoulderToHandVector, shoulderAxis);

        // 
        _rightShoulder.transform.rotation = Quaternion.AngleAxis(shoulderAngle, shoulderAxis) * _rightShoulder.transform.rotation;

        Debug.Log($"afterRotation:{_rightShoulder.transform.localRotation.eulerAngles}");

        // 
        Vector3 controllerTwist = new Vector3(conrtoller.localEulerAngles.z + _handComplementAngle, 0, 0);

        // 
        _rightHand.transform.localRotation = Quaternion.Euler(controllerTwist);
    }

    public void LeftHandTracking(Transform conrtoller)
    {
        // 
        if (!_leftShoulder.HasStateAuthority)
        {
            return;
        }

        // 
        _leftShoulder.transform.localRotation = _leftShoulderOriginRotatioin;

        // 
        Vector3 shoulderForword = -_leftShoulder.transform.right;

        // 
        Vector3 shoulderToHandVector = (conrtoller.position - _leftShoulder.transform.position).normalized;

        // 
        Vector3 shoulderAxis = Vector3.Cross(shoulderForword, shoulderToHandVector).normalized;

        // 
        if (Vector3.Dot(shoulderAxis, Vector3.up) > 0)
        {
            // 
            shoulderAxis *= -1;
        }

        // 
        float shoulderAngle = Vector3.SignedAngle(shoulderForword, shoulderToHandVector, shoulderAxis);

        // 
        _leftShoulder.transform.rotation = Quaternion.AngleAxis(shoulderAngle, shoulderAxis) * _leftShoulder.transform.rotation;

        // 
        Vector3 controllerTwist = new Vector3(-conrtoller.localEulerAngles.z + _handComplementAngle, 0, 0);

        // 
        _leftHand.transform.localRotation = Quaternion.Euler(controllerTwist);
    }

}

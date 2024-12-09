using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarHandTracker
{
    public AvatarHandTracker(Transform rightShoulder, Transform rightHand, Transform leftShoulder, Transform leftHand)
    {
        // 
        _rightShoulderTransform = rightShoulder;
        _rightHandTransform = rightHand;
        _leftShoulderTransform = leftShoulder;
        _leftHandTransform = leftHand;

        // 
        _rightShoulderOriginRotatioin = _rightShoulderTransform.localRotation;
        _leftShoulderOriginRotatioin = _leftShoulderTransform.localRotation;
    }

    // 
    private Transform _rightShoulderTransform = default;

    // 
    private Transform _leftShoulderTransform = default;

    // 
    private Transform _rightHandTransform = default;

    // 
    private Transform _leftHandTransform = default;

    // 
    private Quaternion _rightShoulderOriginRotatioin = default;

    // 
    private Quaternion _leftShoulderOriginRotatioin = default;

    public void RightHandTracking(Transform conrtoller)
    {
        // 
        _rightShoulderTransform.localRotation = _rightShoulderOriginRotatioin;

        // 
        Vector3 shoulderForword = _rightShoulderTransform.right;

        // 
        Vector3 shoulderToHandVector = (conrtoller.position - _rightShoulderTransform.position).normalized;

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
        _rightShoulderTransform.rotation = Quaternion.AngleAxis(shoulderAngle, shoulderAxis);

        // 
        Vector3 controllerTwist = new Vector3(conrtoller.localEulerAngles.z, 0, 0);

        // 
        _rightHandTransform.localRotation = Quaternion.Euler(controllerTwist);
    }

    public void LeftHandTracking(Transform conrtoller)
    {
        // 
        _leftShoulderTransform.localRotation = _leftShoulderOriginRotatioin;

        Debug.Log($"beforeRotation:{_leftShoulderTransform.localRotation.eulerAngles}");

        // 
        Vector3 shoulderForword = -_leftShoulderTransform.right;

        // 
        Vector3 shoulderToHandVector = (conrtoller.position - _leftShoulderTransform.position).normalized;

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
        _leftShoulderTransform.rotation = Quaternion.AngleAxis(shoulderAngle, shoulderAxis);

        Debug.Log($"afterRotation:{_leftShoulderTransform.localRotation.eulerAngles}");

        // 
        Vector3 controllerTwist = new Vector3(-conrtoller.localEulerAngles.z, 0, 0);

        // 
        _leftHandTransform.localRotation = Quaternion.Euler(controllerTwist);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRotater : MonoBehaviour
{
    [SerializeField]
    private Transform _handTransform = default;

    [SerializeField]
    private Transform _armTransform = default;

    [SerializeField]
    private Transform _targetTransform = default;

    [SerializeField]
    private HandType _handType = HandType.right;

    private enum HandType { right, left}

    void Update()
    {
        Vector3 forwardVector = default;

        Vector3 upVector = default;

        Vector3 targetVector = _targetTransform.forward;

        Vector3 targetProjectionVector = default;

        float angleDiference = default;

        float nowHandAngle = _handTransform.localRotation.x;

        float nextHandAngle = default;

        float nowArmAngle = _armTransform.localRotation.x;

        float nextArmAngle = default;

        float roundAngle = 360;

        switch (_handType)
        {
            case HandType.right:

                forwardVector = _handTransform.right;

                upVector = -_handTransform.up;

                break;

            case HandType.left:

                break;

            default:
                return;
        }

        targetProjectionVector = Vector3.Project(targetVector, forwardVector).normalized;

        angleDiference = Vector3.SignedAngle(upVector, targetProjectionVector, forwardVector);

        nextHandAngle = nowHandAngle + angleDiference / 2;

        if (Mathf.Abs(nextHandAngle) > roundAngle)
        {
            nextHandAngle -= Mathf.Sign(nextHandAngle) * roundAngle;
        }

        nextArmAngle = nowArmAngle + angleDiference / 2;

        if (Mathf.Abs(nextArmAngle) > roundAngle)
        {
            nextArmAngle -= Mathf.Sign(nextArmAngle) * roundAngle;
        }

        _handTransform.localRotation = Quaternion.Euler(nextHandAngle, 0, 0);

        _armTransform.localRotation = Quaternion.Euler(nextArmAngle, 0, 0);
    }
}

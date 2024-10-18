using UnityEngine;

public class JointCalculator
{
    public JointCalculator(Transform startTarget, Transform endTarget, Transform hintTarget)
    {
        _startTarget = startTarget;

        _endTarget = endTarget;

        _hintTarget = hintTarget;
    }

    private Transform _startTarget = default;

    private Transform _endTarget = default;

    private Transform _hintTarget = default;

    public Vector3 GetJointPosition(float startHalf, float endHalf)
    {
        float distanceStartToEnd = Vector3.Distance(_startTarget.position , _endTarget.position);

        float cosStartTarget = (Mathf.Pow(endHalf, 2) + Mathf.Pow(distanceStartToEnd, 2) - Mathf.Pow(startHalf, 2)) / (2 * endHalf * distanceStartToEnd);

        float startAngle = Mathf.Acos(cosStartTarget) * Mathf.Rad2Deg;

        Vector3 startToEndVector = _endTarget.position - _startTarget.position;

        Vector3 crossAxis = Vector3.up;

        if (startToEndVector.normalized == crossAxis)
        {
            crossAxis = Vector3.right;
        }

        Debug.Log($"{startToEndVector} , {crossAxis}");

        crossAxis = Vector3.Cross(startToEndVector, crossAxis);

        Debug.Log($"{startToEndVector} , {crossAxis}");

        Quaternion angleAxis = Quaternion.AngleAxis(startAngle, crossAxis);

        Vector3 startToCirVector = (angleAxis * startToEndVector).normalized;

        Debug.Log($"{angleAxis} , {startToCirVector}");

        Vector3 cirPositionA = _startTarget.position + startToCirVector * startHalf;

        angleAxis = Quaternion.AngleAxis(-startAngle, crossAxis);

        startToCirVector = (angleAxis * startToEndVector).normalized.normalized;

        Vector3 cirPositionB = _startTarget.position + startToCirVector * startHalf;

        Vector3 cirToCrossNormal = (cirPositionB - cirPositionA).normalized;

        Vector3 cirToStartVector = _startTarget.position - cirPositionA;

        float jointHalf = Vector3.Dot(cirToStartVector, cirToCrossNormal) / Vector3.Dot(cirToCrossNormal, cirToCrossNormal);

        Vector3 crossPosition = cirPositionA + cirToCrossNormal * jointHalf;

        Vector3 hintProjectionNormal = (_startTarget.position - crossPosition).normalized;

        Vector3 crossToHintVector = _hintTarget.position - crossPosition;

        float distanceProjectedToHint = Vector3.Dot(hintProjectionNormal, crossToHintVector);

        Vector3 projectedPosition = _hintTarget.position - hintProjectionNormal * distanceProjectedToHint;

        Vector3 jointNormal = (projectedPosition - crossPosition).normalized;

        Vector3 jointPosition = crossPosition + (jointNormal * jointHalf);

        Debug.Log($"{startAngle} , {crossAxis}");

        return jointPosition;
    }
}

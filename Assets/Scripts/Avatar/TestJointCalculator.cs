using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJointCalculator : MonoBehaviour
{
    [SerializeField]
    private Transform _startTarget = default;
    [SerializeField]
    private Transform _endTarget = default;
    [SerializeField]
    private Transform _hintTarget = default;
    [SerializeField]
    private Transform _jointTarget = default;
    [SerializeField]
    private Transform _upperArm = default;
    [SerializeField]
    private Transform _lowerArm = default;
    [SerializeField]
    private float _startHalf = 2f;
    [SerializeField]
    private float _endHalf = 2f;

    private JointCalculator _jointCalculator = default;

    // Start is called before the first frame update
    void Start()
    {
        _jointCalculator = new JointCalculator(_startTarget, _endTarget, _hintTarget);
    }

    // Update is called once per frame
    void Update()
    {
        _jointTarget.position = _jointCalculator.GetJointPosition(_startHalf, _endHalf);

        Vector3 lowerPos = Vector3.Lerp(_endTarget.position,_jointTarget.position,0.5f);

        Vector3 upperPos = Vector3.Lerp(_jointTarget.position, _startTarget.position,0.5f);

        _upperArm.position = upperPos;

        _lowerArm.position = lowerPos;

        _upperArm.LookAt(_startTarget, Vector3.forward);

        _lowerArm.LookAt(_jointTarget, Vector3.forward);
    }
}

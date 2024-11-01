using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotate : MonoBehaviour
{
    private enum type { world, self, byself};

    [SerializeField]
    private type _type;

    [SerializeField]
    private float _angle = 40;

    [SerializeField]
    private float _addValue = 30;

    [SerializeField]
    private Vector3 _axis = new Vector3(1, 0 ,0);

    private Quaternion _defaultQuaternion = default;

    // Start is called before the first frame update
    void Start()
    {
        /*
        switch (_type)
        {
            case type.world:
                transform.Rotate(_axis.normalized, _angle, Space.World);
                break;

            case type.self:
                transform.Rotate(_axis.normalized, _angle, Space.Self);
                break;
        }
        */

        _defaultQuaternion = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            switch (_type)
            {
                case type.world:
                    _angle += _addValue;
                    transform.rotation = Quaternion.AngleAxis(_angle, _axis.normalized);
                    break;

                case type.self:
                    _angle += _addValue;
                    Vector3 localAxis = transform.worldToLocalMatrix * _axis;
                    Debug.Log($"{transform.root.name}: <color=red>localAxis = {localAxis}</color>");
                    transform.rotation = Quaternion.AngleAxis(_angle, localAxis.normalized);
                    break;

                case type.byself:
                    _angle += _addValue;
                    Quaternion addQuaternion = Quaternion.AngleAxis(_angle, _axis.normalized);
                    transform.rotation = _defaultQuaternion * addQuaternion;
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            switch (_type)
            {
                case type.world:
                    _angle -= _addValue;
                    transform.rotation = Quaternion.AngleAxis(_angle, _axis.normalized);
                    break;

                case type.self:
                    _angle -= _addValue;
                    Vector3 localAxis = transform.worldToLocalMatrix * _axis;
                    Debug.Log($"{transform.root.name}: <color=red>localAxis = {localAxis}</color>");
                    transform.rotation = Quaternion.AngleAxis(_angle, localAxis.normalized);
                    break;

                case type.byself:
                    _angle -= _addValue;
                    Quaternion addQuaternion = Quaternion.AngleAxis(_angle, _axis.normalized);
                    transform.rotation = _defaultQuaternion * addQuaternion;
                    break;
            }
        }
    }
}

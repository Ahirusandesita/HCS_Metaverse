using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectChenger : MonoBehaviour
{
    [SerializeField]
    private Vector3 _photographPosition = Vector3.zero;

    [SerializeField]
    private GameObject _parentObject = default;

    [SerializeField]
    private int _childIndex = 0;

    [SerializeField]
    private Camera _photoCamera = default;

    private Vector3 _beforePositoin = default;

    private GameObject _targetObject = default;

    private float _defaultCameraSize = default;


    // Start is called before the first frame update
    private void Start()
    {
        _targetObject = GetNextTarget();

        _defaultCameraSize = _photoCamera.orthographicSize;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ReleaseTarget();

            _childIndex++;

            _targetObject = GetNextTarget();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ReleaseTarget();

            _childIndex--;

            _targetObject = GetNextTarget();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _targetObject.transform.rotation *= Quaternion.Euler(0, 15, 0);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _targetObject.transform.rotation *= Quaternion.Euler(0, -15, 0);
        }

        if (Input.GetKey(KeyCode.W))
        {
            _targetObject.transform.position += new Vector3(0, 0.5f, 0) * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            _targetObject.transform.position -= new Vector3(0, 0.5f, 0) * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            _photoCamera.orthographicSize += 0.5f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            _photoCamera.orthographicSize -= 0.5f * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _photoCamera.orthographicSize = _defaultCameraSize;
        }
    }

    private GameObject GetNextTarget()
    {
        try
        {
            Transform target = _parentObject.transform.GetChild(_childIndex);

            _beforePositoin = target.position;

            target.position = _photographPosition;

            return target.gameObject;
        }
        catch
        {
            Debug.LogError("childIndex‚ª”ÍˆÍŠO");
            return null;
        }
    }

    private void ReleaseTarget()
    {
        if (_targetObject != default)
        {
            _targetObject.transform.position = _beforePositoin;
        }
    }
}

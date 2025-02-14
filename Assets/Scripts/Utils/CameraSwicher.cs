using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwicher : MonoBehaviour
{
    private Camera[] _allCamera = default;
	private int _index = 0;
	private void Start()
	{
		_allCamera = FindObjectsByType<Camera>(FindObjectsSortMode.None);
		_index = -1;
		CameraSwitch();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.RightShift))
		{
			CameraSwitch();
		}
	}

	private void CameraSwitch()
	{
		_index++;
		foreach(Camera camera in _allCamera)
		{
			camera.enabled = false;
		}
		if(_index >= _allCamera.Length)
		{
			_index = 0;
		}
		_allCamera[_index].enabled = true;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityStartUIController : MonoBehaviour
{
	[SerializeField]
	private float _offsetZ = default;
	[SerializeField]
	private bool _isFollow = true;
	private Transform _myTransform = default;
	private Transform _playerHeadTransform;
	private Transform MyTransform { get => _myTransform ??= transform; }

	private void Start()
	{
		_playerHeadTransform = GameObject.Find("CenterEyeAnchor").transform;
	}

	private void Update()
	{
		if (!_isFollow) { return; }
		MyTransform.position = _playerHeadTransform.forward * _offsetZ 
			+ _playerHeadTransform.position;
		MyTransform.rotation = _playerHeadTransform.rotation;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityStartUIController : MonoBehaviour
{
	[SerializeField]
	private Vector3 _offset = default;
	private Transform _myTransform = default;
	private Transform _playerHeadTransform;
	private Transform MyTransform { get => _myTransform ??= transform; }

	private void Start()
	{
		_playerHeadTransform = GameObject.Find("CenterEyeAnchor").transform;
	}

	private void Update()
	{
		MyTransform.rotation = _playerHeadTransform.rotation;
		MyTransform.position = _playerHeadTransform.position + _offset;
	}
}

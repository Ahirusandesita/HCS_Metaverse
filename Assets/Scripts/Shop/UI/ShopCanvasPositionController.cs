using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KumaDebug;

public class ShopCanvasPositionController : MonoBehaviour
{
	[SerializeField]
	private Vector3 _positionOffset = Vector3.forward;
	private Transform _myTransform = default;
	private Transform _playerTransform = default;


	private void OnEnable()
	{
		_playerTransform = FindObjectOfType<VRPlayerController>().transform;
		_myTransform = transform;
	}
	private void Update()
	{
		_myTransform.rotation = _playerTransform.rotation;
		_myTransform.position = _playerTransform.position 
			+ (_myTransform.forward * _positionOffset.x) 
			+ (_myTransform.forward * _positionOffset.y) 
			+ (_myTransform.forward * _positionOffset.z);
	}
}
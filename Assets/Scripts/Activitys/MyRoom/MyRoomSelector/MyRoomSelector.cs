using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRoomSelector : MonoBehaviour
{
	[SerializeField]
	private float _offsetZ = 0.5f;
	[SerializeField]
	private MyRoomSelecterUIManager _myRoomSelecterUIManager;
	private Transform _myTransform = default;
	private Transform _playerHeadTransform;
	private Transform MyTransform { get => _myTransform ??= transform; }

	private void Start()
	{
		_playerHeadTransform = GameObject.Find("CenterEyeAnchor").transform;
		Init();
	}

	private void Update()
	{
		MyTransform.position = _playerHeadTransform.forward * _offsetZ
			+ _playerHeadTransform.position;
		MyTransform.rotation = _playerHeadTransform.rotation;
	}
	[ContextMenu("init")]
	public void Init()
	{
		_myRoomSelecterUIManager.Init();
	}
}

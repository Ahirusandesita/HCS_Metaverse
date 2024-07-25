using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Fusion;

public class RemoteView : NetworkBehaviour
{
	private Transform _playerTransform;
	private Transform _myTransform;

	public override void Spawned()
	{
		base.Spawned();
		_playerTransform = FindObjectOfType<VRPlayerController>().transform;
		_myTransform = transform;
	}

	public void SetVector3(Vector3 vector)
	{
		//Debug.LogWarning(vector);

	}

	public override void FixedUpdateNetwork()
	{
		base.FixedUpdateNetwork();
		_myTransform.position = _playerTransform.position;
	}
}

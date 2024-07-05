using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Fusion;

public class RemoteView : NetworkBehaviour
{
	private Transform myTransform = default;
	[Networked]
	public Vector3 LinkPosition { get; set; }

	private void Start()
	{
		myTransform = transform;
	}

	

	public void SetVector3(Vector3 vector)
	{
		//Debug.LogWarning(vector);

		myTransform.position = vector;
	}
}

using UnityEngine;
using System.Collections.Generic;
using Fusion;
using HCSMeta.Player;

namespace HCSMeta.Network
{
	public class RemoteView : NetworkBehaviour
	{
		private Transform _playerTransform;
		private Transform _viewTransform;

		public override void Spawned()
		{
			base.Spawned();
			_playerTransform = FindObjectOfType<VRPlayerController>().transform;
			_viewTransform = transform;
		}

		public void SetVector3(Vector3 vector)
		{
			//Debug.LogWarning(vector);
		}

		public override void FixedUpdateNetwork()
		{
			base.FixedUpdateNetwork();
			_viewTransform.position = _playerTransform.position;
		}
	}
}
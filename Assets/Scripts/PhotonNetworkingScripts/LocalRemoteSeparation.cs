using UnityEngine;
using Fusion;

namespace HCSMeta.Network
{
	public class LocalRemoteSeparation : MonoBehaviour
	{
		[SerializeField]
		private SeparationLifetimeScope separationLifetimeScope;

		[SerializeField]
		private GameObject localGameObject;

		[SerializeField]
		private NetworkPrefabRef remoteViewObjectPrefab;

		public void RemoteViewCreate(NetworkRunner networkRunner, PlayerRef playerRef)
		{
			NetworkObject remoteViewObject
				= networkRunner.Spawn(remoteViewObjectPrefab, Vector3.zero, Quaternion.identity, playerRef);
			RemoteView remoteView = remoteViewObject.GetComponent<RemoteView>();

			Instantiate(separationLifetimeScope).SeparationSetup(localGameObject, remoteView).Build();
		}
	}
}
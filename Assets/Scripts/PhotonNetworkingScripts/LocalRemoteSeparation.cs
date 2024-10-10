using UnityEngine;
using Fusion;
using Layer_lab._3D_Casual_Character;

public class LocalRemoteSeparation : MonoBehaviour
{
	[SerializeField]
	private SeparationLifetimeScope separationLifetimeScope;

	[SerializeField]
	private GameObject localGameObject;

	[SerializeField]
	private NetworkPrefabRef remoteViewObjectPrefab;

	public async void RemoteViewCreate(NetworkRunner networkRunner, PlayerRef playerRef)
	{
		NetworkObject remoteViewObject
			= networkRunner.Spawn(remoteViewObjectPrefab, Vector3.zero, Quaternion.identity);
		RemoteView remoteView = remoteViewObject.GetComponent<RemoteView>();

		if (playerRef == networkRunner.LocalPlayer)
		{
			CharacterControl characterController = remoteView.GetComponentInChildren<CharacterControl>();
			foreach (Renderer renderer in characterController.GetComponentsInChildren<Renderer>())
			{
				renderer.gameObject.SetActive(false);
			}
		}

		Instantiate(separationLifetimeScope).SeparationSetup(localGameObject, remoteView).Build();
	}
}
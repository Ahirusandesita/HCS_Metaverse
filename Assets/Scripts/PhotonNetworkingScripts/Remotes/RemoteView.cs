using UnityEngine;
using Fusion;

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
	}

	public override void FixedUpdateNetwork()
	{
		base.FixedUpdateNetwork();
		_viewTransform.position = _playerTransform.position;
	}

	[ContextMenu("test")]
	private void Test()
	{
		GetComponentInChildren<CharacterRPCManager>()
			.Rpc_ChangeWear(Layer_lab._3D_Casual_Character.PartsType.Top,0,GetComponent<NetworkObject>());
	}
}

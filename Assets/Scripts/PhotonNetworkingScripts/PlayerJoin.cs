using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerJoin : SimulationBehaviour, IPlayerJoined
{
	private ActivityZone _activityZone = default;
	private MasterServerConect _masterServer = default;
	[SerializeField]
	private LocalRemoteSeparation localRemoteReparation;
	[SerializeField]
	private NetworkPrefabRef _roomCounterPrefab;
	[SerializeField]
	private NetworkPrefabRef _rpcManagerPrefab;
	public void PlayerJoined(PlayerRef player)
	{
		_masterServer = FindObjectOfType<MasterServerConect>();
		_activityZone = FindObjectOfType<ActivityZone>();
		//InitRegisterNetwork();

		Transform masterTransform = _masterServer.transform;
		Runner.Spawn(_roomCounterPrefab).transform.parent = masterTransform;

		Debug.LogWarning(Runner);
		if(FindObjectOfType<RPCManager>() is null)
		{

			NetworkObject networkObject = Runner.Spawn(_rpcManagerPrefab, Vector3.zero, Quaternion.identity);
			RPCManager rpcManager = networkObject.GetComponent<RPCManager>();

			if (_activityZone is not null)
			{
				rpcManager.SessionNameChangedHandler += _masterServer.JoinOrCreateSession;
			}
			rpcManager.transform.parent = masterTransform;
		}


		localRemoteReparation.RemoteViewCreate(Runner, Runner.LocalPlayer);
		RPCManager.Instance.Rpc_RequestRoomData(player);
	}

	private void InitRegisterNetwork()
	{
		NetworkObject[] networkObjects = FindObjectsOfType<NetworkObject>();
		Runner.RegisterSceneObjects(Runner.GetSceneRef(_masterServer.gameObject), networkObjects);
	}
}

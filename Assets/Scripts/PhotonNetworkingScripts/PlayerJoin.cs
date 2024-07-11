using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerJoin : SimulationBehaviour, IPlayerJoined
{
	private ActivityZone _activityZone = default;
	
	[SerializeField]
	private NetworkPrefabRef _roomCounterPrefab;
	[SerializeField]
	private NetworkPrefabRef _rpcManagerPrefab;
	public void PlayerJoined(PlayerRef player)
	{
		if (!Runner.IsSharedModeMasterClient) { return; }
		MasterServerConect masterServer = FindObjectOfType<MasterServerConect>();
		if (RPCManager.Instance != null && RoomManager.Instance != null)
		{
			RPCManager.Instance.SessionNameChangedHandler += masterServer.JoinOrCreateSession;
			RPCManager.Instance.Rpc_Init(player);
			return;
		}
		_activityZone = FindObjectOfType<ActivityZone>();

		Transform masterTransform = masterServer.transform;
		if(FindObjectOfType<RoomManager>() == null)
		{
			NetworkObject networkObject = Runner.Spawn(_roomCounterPrefab);
			RoomManager roomManager = networkObject.GetComponent<RoomManager>();
			roomManager.transform.parent = masterTransform;
		}

		
		if(FindObjectOfType<RPCManager>() == null)
		{
			NetworkObject networkObject = Runner.Spawn(_rpcManagerPrefab);
			RPCManager rpcManager = networkObject.GetComponent<RPCManager>();
			rpcManager.transform.parent = masterTransform;
		}
		RPCManager.Instance.SessionNameChangedHandler += masterServer.JoinOrCreateSession;
		RPCManager.Instance.Rpc_Init(player);
	}
}

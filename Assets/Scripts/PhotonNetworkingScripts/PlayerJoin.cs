using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerJoin : SimulationBehaviour, IPlayerJoined
{
	[SerializeField]
	private NetworkPrefabRef _rpcManagerPrefab;
	public void PlayerJoined(PlayerRef player)
	{
		if (!Runner.IsSharedModeMasterClient) { return; }
		RPCManager rpcManager = FindObjectOfType<RPCManager>();
		if(rpcManager != null) { return; }
		MasterServerConect masterServer = FindObjectOfType<MasterServerConect>();
		Transform masterTransform = masterServer.transform;
		NetworkObject networkObject = Runner.Spawn(_rpcManagerPrefab);
		rpcManager = networkObject.GetComponent<RPCManager>();
		rpcManager.transform.parent = masterTransform;

		RPCManager.Instance.Rpc_Init(player);

	}
}

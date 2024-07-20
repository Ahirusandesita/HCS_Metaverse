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
		if (player != Runner.LocalPlayer) { return; }

		MasterServerConect masterServer = FindObjectOfType<MasterServerConect>();
		Transform masterTransform = masterServer.transform;
		NetworkObject networkObject = Runner.Spawn(_rpcManagerPrefab);
		RPCManager rpcManager = networkObject.GetComponent<RPCManager>();
		rpcManager.transform.parent = masterTransform;

		RPCManager.Instance.Rpc_Init(player);

	}
}

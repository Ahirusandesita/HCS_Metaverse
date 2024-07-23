using Fusion;
using UnityEngine;
using System.Collections.Generic;

public delegate void SessionNameChanged(string name);

public class RPCManager : NetworkBehaviour
{
	public event SessionNameChanged SessionNameChangedHandler;

	[SerializeField]
	private GameObject _leaderObjectPrefab;

	private GameObject _leaderObject;

	private static RPCManager _instance;
	public static RPCManager Instance { get => _instance; }

	public override void Spawned()
	{
		Debug.LogWarning("Spawned");
		MasterServerConect masterServer = FindObjectOfType<MasterServerConect>();
		SessionNameChangedHandler += masterServer.JoinOrCreateSession;
		_instance = this;
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	public RpcInvokeInfo Rpc_Test([RpcTarget] PlayerRef playerRef)
	{
		Debug.LogWarning($"<color=White>Test:{playerRef}</color>");
		return default;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_ChangeRoomSessionName(PlayerRef chengeTarget, string nextSessionName)
	{
		Debug.LogError(
			$"ChangeSessionName:{nextSessionName}" +
			$"\nPlayerName:{chengeTarget}");
		RoomManager.Instance.ChengeSessionName(chengeTarget, nextSessionName);
	}


	/// <summary>
	/// セッションに参加または作成する
	/// </summary>
	/// <param name="sessionName">セッション名</param>
	/// <param name="rpcTarget">RPCの対象プレイヤー</param>
	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	public void Rpc_JoinSession(string sessionName, [RpcTarget] PlayerRef rpcTarget = new())
	{
		Debug.LogError("RpcJoin");
		//実行
		SessionNameChangedHandler?.Invoke(sessionName);
	}

	/// <summary>
	/// 状態変更権限を手放させる
	/// </summary>
	/// <param name="networkObject">手放すオブジェクト</param>
	/// <param name="rpcTarget">手放すプレイヤー</param>
	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public void Rpc_ReleaseStateAuthority(NetworkObject networkObject, [RpcTarget] PlayerRef rpcTarget = new())
	{
		networkObject.ReleaseStateAuthority();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_RoomJoinOrCreate(WorldType worldType, PlayerRef playerRef, int roomNumber = -1)
	{
		RoomManager.Instance.JoinOrCreate(worldType, playerRef, Runner.SessionInfo.Name, roomNumber);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_RoomLeftOrClose(PlayerRef playerRef)
	{
		RoomManager.Instance.LeftOrClose(playerRef);
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	public void Rpc_RequestRoomData(PlayerRef requestPlayer)
	{
		Room roomTemp = RoomManager.Instance.GetCurrentRoom(Runner.LocalPlayer);
		if (roomTemp is null) { return; }
		bool isLeader = roomTemp.LeaderIndex == roomTemp[Runner.LocalPlayer];
		Rpc_SendRoomData(requestPlayer, roomTemp.WorldType, Runner.LocalPlayer
			, isLeader, Runner.SessionInfo.Name, roomTemp.Number);
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	private void Rpc_SendRoomData([RpcTarget] PlayerRef rpcTarget
		, WorldType worldType, PlayerRef playerRef, bool isLeader, string sessionName, int roomNumber = -1)
	{
		RoomManager.Instance.JoinOrCreate(worldType, playerRef, sessionName, roomNumber);
		if (isLeader)
		{
			RoomManager.Instance.LeaderChange(playerRef);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_InstanceLeaderObject([RpcTarget] PlayerRef rpcTarget)
	{
		Debug.LogWarning("Rpc_Instance:" + _leaderObject);
		if (_leaderObject) { return; }
		_leaderObject = Instantiate(_leaderObjectPrefab);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_DestroyLeaderObject([RpcTarget] PlayerRef rpcTarget)
	{
		Debug.LogWarning("Rpc_Destroy:" + _leaderObject);
		if (!_leaderObject) { return; }
		Destroy(_leaderObject);
	}
}

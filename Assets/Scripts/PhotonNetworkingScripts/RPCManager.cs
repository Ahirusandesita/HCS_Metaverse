using Fusion;
using UnityEngine;
using System.Collections.Generic;

public delegate void SessionNameChanged(string name);

public class RPCManager : NetworkBehaviour
{
	public event SessionNameChanged SessionNameChangedHandler;

	private static RPCManager _instance;
	public static RPCManager Instance { get => _instance; }


	public override void Spawned()
	{
		base.Spawned();
		if (_instance is null)
		{
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			Runner.Despawn(this.Object);
			return;
		}
		Debug.LogWarning(Runner);
		RPCManager._instance.Rpc_RequestRoomData(Runner.LocalPlayer);
	}

	/// <summary>
	/// セッションに参加または作成する
	/// </summary>
	/// <param name="sessionName">セッション名</param>
	/// <param name="rpcTarget">RPCの対象プレイヤー</param>
	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_JoinSession(string sessionName, [RpcTarget] PlayerRef rpcTarget = new())
	{
		Debug.LogWarning("RPC_SessionName:" + sessionName);
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
		RoomManager.Instance.JoinOrCreate(worldType, playerRef, roomNumber);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_RoomLeftOrClose(PlayerRef playerRef)
	{
		RoomManager.Instance.LeftOrClose(playerRef);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_RequestRoomData(PlayerRef requestPlayer)
	{
		Debug.LogWarning(Runner.LocalPlayer + ":" + requestPlayer);
		Room roomTemp = RoomManager.Instance.GetCurrentRoom(Runner.LocalPlayer);
		if (roomTemp is null) { return; }
		bool isLeader = roomTemp.LeaderIndex == roomTemp[Runner.LocalPlayer];
		Rpc_SendRoomData(requestPlayer, roomTemp.WorldType, Runner.LocalPlayer, isLeader, roomTemp.Number);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	private void Rpc_SendRoomData([RpcTarget] PlayerRef rpcTarget
		, WorldType worldType, PlayerRef playerRef, bool isLeader, int roomNumber = -1)
	{
		RoomManager.Instance.JoinOrCreate(worldType, playerRef, roomNumber);
		if (isLeader)
		{
			RoomManager.Instance.LeaderChenge(playerRef);
		}
	}

}

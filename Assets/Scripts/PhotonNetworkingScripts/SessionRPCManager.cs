using Fusion;

public class SessionRPCManager : NetworkBehaviour
{
	public override void Spawned()
	{
		XDebug.LogWarning($"RPCManager_Spawned", KumaDebugColor.SuccessColor);
		DontDestroyOnLoad(this.gameObject);
		if (!Runner.IsSharedModeMasterClient)
		{
			Rpc_RequestRoomData(GateOfFusion.Instance.NetworkRunner.LocalPlayer);
		}
	}

	private void OnDisable()
	{
		XDebug.LogWarning($"RpcManager_Destory", KumaDebugColor.ErrorColor);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_ChangeRoomSessionName(PlayerRef chengeTarget, string nextSessionName)
	{
		XDebug.LogWarning(
			$"ChangeSessionName:{nextSessionName}" +
			$"\nPlayerName:{chengeTarget}", KumaDebugColor.RpcColor);
		RoomManager.Instance.ChangeSessionName(chengeTarget, nextSessionName);
	}


	/// <summary>
	/// セッションに参加または作成する
	/// </summary>
	/// <param name="sessionName">セッション名</param>
	/// <param name="rpcTarget">RPCの対象プレイヤー</param>
	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	public async void Rpc_JoinSession(string sessionName,string sceneName, [RpcTarget] PlayerRef rpcTarget = new())
	{
		XDebug.LogWarning("RpcJoin", KumaDebugColor.SuccessColor);
		MasterServerConect masterServer = FindObjectOfType<MasterServerConect>();
		await masterServer.Disconnect();
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
		//実行
		await masterServer.JoinOrCreateSession(sessionName);
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	public void Rpc_GrabStateAuthorityChanged(NetworkObject networkObject)
	{
		StateAuthorityData stateAuthorityData = networkObject.GetComponent<StateAuthorityData>();
		if (!networkObject.HasStateAuthority)
		{
			stateAuthorityData.IsGrabbable = false;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	public void Rpc_ReleseStateAuthorityChanged(NetworkObject networkObject)
	{
		StateAuthorityData stateAuthorityData = networkObject.GetComponent<StateAuthorityData>();
		stateAuthorityData.IsGrabbable = true;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_JoinOrCreateRoom(SceneNameType worldType, PlayerRef playerRef, int roomNumber = -1)
	{
		RoomManager.Instance.JoinOrCreate(worldType, playerRef, Runner.SessionInfo.Name, roomNumber);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_LeftOrCloseRoom(PlayerRef playerRef)
	{
		RoomManager.Instance.LeftOrClose(playerRef);
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	public void Rpc_RequestRoomData(PlayerRef requestPlayer)
	{
		XDebug.LogWarning($"Rpc_RequestRoomData:{requestPlayer}", KumaDebugColor.RpcColor);
		Room roomTemp = RoomManager.Instance.GetCurrentRoom(Runner.LocalPlayer);
		int roomKey = RoomManager.Instance.GetCurrentRoomKey(roomTemp);
		if (roomTemp is null) { return; }
		bool isLeader = roomTemp.LeaderIndex == roomTemp.GetPlayerIndex(Runner.LocalPlayer);
		Rpc_SendRoomData(requestPlayer, roomTemp.SceneNameType, Runner.LocalPlayer
			, isLeader, Runner.SessionInfo.Name, roomKey);
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	private void Rpc_SendRoomData([RpcTarget] PlayerRef rpcTarget
		, SceneNameType worldType, PlayerRef playerRef, bool isLeader, string sessionName, int roomNumber = -1)
	{
		XDebug.LogWarning($"Rpc_SendRoomData:{playerRef}", KumaDebugColor.RpcColor);
		RoomManager.Instance.JoinOrCreate(worldType, playerRef, sessionName, roomNumber);
		if (isLeader)
		{
			RoomManager.Instance.LeaderChange(playerRef);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	public void Rpc_DestroyLeaderObject([RpcTarget] PlayerRef rpcTarget)
	{
		XDebug.LogWarning("Rpc_DestroyLeaderObject:" + rpcTarget, KumaDebugColor.RpcColor);
		RoomManager.Instance.DestroyLeaderObject();
	}
}
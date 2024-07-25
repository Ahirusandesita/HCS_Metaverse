using Fusion;
using UnityEngine;
using Cysharp.Threading.Tasks;

public delegate void SessionNameChanged(string name);

public class RPCManager : NetworkBehaviour
{
	public event SessionNameChanged SessionNameChangedHandler;

	[SerializeField]
	private GameObject _leaderObjectPrefab;

	private GameObject _leaderObject;

	private static RPCManager _instance;
	public static RPCManager Instance { get => _instance; }

	public async override void Spawned()
	{
		Debug.LogWarning($"<color=yellow>RPCManager_Spawned</color>");
		RoomManager.Instance.Test();
		MasterServerConect masterServer = FindObjectOfType<MasterServerConect>();
		SessionNameChangedHandler += masterServer.JoinOrCreateSession;
		_instance = this;

		if (Runner.SessionInfo.PlayerCount == 2)
		{
			await UniTask.WaitForSeconds(1f);
			if (Runner.SessionInfo.PlayerCount > 1)
			{
				_instance.Rpc_RequestRoomData(Runner.LocalPlayer);
			}
		}
		//if(RoomManager.Instance.GetCurrentRoom(Runner.LocalPlayer) == null)
		//{
		//	FindObjectOfType<TestGameZone>().Open();
		//}
		masterServer.TestTestTestTest();
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

	[Rpc(RpcSources.All, RpcTargets.All,InvokeLocal = false)]
	public void Rpc_GrabStateAuthorityChanged(NetworkObject networkObject)
	{
		StateAuthorityData stateAuthorityData = networkObject.GetComponent<StateAuthorityData>();
		if (!networkObject.HasStateAuthority)
		{
			stateAuthorityData.IsGrabbable = false;
		}
	}

	[Rpc(RpcSources.All,RpcTargets.All,InvokeLocal = false)]
	public void Rpc_ReleseStateAuthorityChanged(NetworkObject networkObject)
	{
		StateAuthorityData stateAuthorityData = networkObject.GetComponent<StateAuthorityData>();
		stateAuthorityData.IsGrabbable = true;
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
		Debug.LogWarning($"<color=orange>Rpc_RequestRoomData:{requestPlayer}</color>");
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
		Debug.LogWarning($"<color=orange>Rpc_SendRoomData</color>:{playerRef}");
		RoomManager.Instance.JoinOrCreate(worldType, playerRef, sessionName, roomNumber);
		if (isLeader)
		{
			RoomManager.Instance.LeaderChange(playerRef);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_InstanceLeaderObject([RpcTarget] PlayerRef rpcTarget)
	{
		Debug.LogWarning("<color=orange>Rpc_InstanceLeaderObject</color>:" + _leaderObject);
		if (_leaderObject) { return; }
		_leaderObject = Instantiate(_leaderObjectPrefab);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_DestroyLeaderObject([RpcTarget] PlayerRef rpcTarget)
	{
		Debug.LogWarning("<color=orange>Rpc_DestroyLeaderObject</color>:" + _leaderObject);
		if (!_leaderObject) { return; }
		Destroy(_leaderObject);
	}

	
}

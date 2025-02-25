using Fusion;
using KumaDebug;
using UnityEngine.SceneManagement;
public class SessionRPCManager : NetworkBehaviour
{
	public override void Spawned()
	{
		XKumaDebugSystem.LogWarning($"RPCManager_Spawned", KumaDebugColor.SuccessColor);
		DontDestroyOnLoad(this.gameObject);
		if (!Runner.IsSharedModeMasterClient)
		{
			Rpc_RequestRoomData(GateOfFusion.Instance.NetworkRunner.LocalPlayer);
		}
		Room currentRoom = RoomManager.Instance.FindCurrentRoom(Runner.LocalPlayer);
		if (currentRoom == null)
		{
			string firstScene = SceneManager.GetActiveScene().name;

			if (FindObjectOfType<MasterServerConect>().IsSolo)
			{
				_ = RoomManager.Instance.JoinOrCreate(firstScene, Runner.LocalPlayer);
			}
			else
			{
				Rpc_JoinOrCreateRoom(firstScene, Runner.LocalPlayer);
			}
		}
	}

	private void OnDisable()
	{
		XKumaDebugSystem.LogWarning($"RpcManager_Destory", KumaDebugColor.ErrorColor);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_ChangeRoomSessionName(PlayerRef chengeTarget, string nextSessionName)
	{
		XKumaDebugSystem.LogWarning(
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
	public async void Rpc_JoinSession(string sessionName, string sceneName, [RpcTarget] PlayerRef rpcTarget = new())
	{
		XKumaDebugSystem.LogWarning("RpcJoin", KumaDebugColor.SuccessColor);
		MasterServerConect masterServer = FindObjectOfType<MasterServerConect>();
		await masterServer.Disconnect();
		SceneManager.LoadScene(sceneName);
		//実行
		await masterServer.JoinOrCreateSession(sceneName,sessionName, rpcTarget);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public async void Rpc_JoinOrCreateRoom(string joinWorldType, PlayerRef joinPlayer)
	{
		JoinOrCreateResult result = await RoomManager.Instance.JoinOrCreate(joinWorldType, joinPlayer);
		string temp = result switch
		{
			JoinOrCreateResult.Create => "を作成",
			JoinOrCreateResult.Join => "に参加",
			_ => "の作成、参加に失敗",
		};
		XKumaDebugSystem.LogWarning($"{joinPlayer}が{joinWorldType}のルーム{temp}しました", KumaDebugColor.RpcColor);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public async void Rpc_LeftOrCloseRoom(PlayerRef leftPlayer)
	{
		await RoomManager.Instance.LeftOrClose(leftPlayer);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_RoomStandbyOn()
	{
		FindObjectOfType<MasterServerConect>().IsRoomStandbyOn();
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	public void Rpc_RequestRoomData(PlayerRef requestPlayer)
	{
		XKumaDebugSystem.LogWarning($"Rpc_RequestRoomData:{requestPlayer}", KumaDebugColor.RpcColor);
		Room roomTemp = RoomManager.Instance.FindCurrentRoom(Runner.LocalPlayer);
		if (roomTemp is null) { return; }
		XKumaDebugSystem.LogWarning($"{roomTemp.SceneNameType}", KumaDebugColor.ErrorColor);
		bool isLeader = roomTemp.LeaderPlayerRef == Runner.LocalPlayer;
		Rpc_SendRoomData(requestPlayer, roomTemp.SceneNameType, Runner.LocalPlayer
			, isLeader);
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	private async void Rpc_SendRoomData([RpcTarget] PlayerRef rpcTarget
		, string worldType, PlayerRef playerRef, bool isLeader)
	{
		XKumaDebugSystem.LogWarning($"Rpc_SendRoomData:{worldType}:{playerRef}:{isLeader}", KumaDebugColor.RpcColor);
		await RoomManager.Instance.JoinOrCreate(worldType, playerRef);
		//自分がリーダーの場合リーダーを自分に変える
		if (isLeader)
		{
			Rpc_ChangeLeader(playerRef);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	public void Rpc_ChangeLeader(PlayerRef nextLeader)
	{
		RoomManager.Instance.LeaderChange(nextLeader);
	}

	/// <summary>
	/// StateAuthorityを離させる
	/// </summary>
	/// <param name="rpcTarget"></param>
	/// <param name="networkObject"></param>
	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
	public void Rpc_ReleaseStateAuthority([RpcTarget] PlayerRef rpcTarget, NetworkObject networkObject)
	{
		XKumaDebugSystem.LogWarning($"{rpcTarget}:{networkObject.name}");
		networkObject.ReleaseStateAuthority();

	}
	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_ExecuteOnActivityConnedted()
	{
		GateOfFusion.Instance.ExecuteOnActivityConnected();
	}
}
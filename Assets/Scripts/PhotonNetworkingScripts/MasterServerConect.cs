using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using Photon.Voice.Unity;
using Photon.Voice.Fusion;
using Cysharp.Threading.Tasks;

public class MasterServerConect : NetworkBehaviour, INetworkRunnerCallbacks, IMasterServerConectable
{
	[SerializeField]
	private NetworkPrefabRef _rpcManagerPrefab;
	[SerializeField]
	private Recorder _recorder;
	[SerializeField]
	private NetworkRunner _networkRunnerPrefab;

	[SerializeField]
	private LocalRemoteSeparation localRemoteReparation;

	private NetworkRunner _networkRunner;

	/// <summary>
	/// このクラスはランナーとの紐づけはしないためラップする
	/// </summary>
	public new NetworkRunner Runner { get => _networkRunner; }

	private async void Awake()
	{
		XDebug.LogWarning("MasterServerConnectAwake", KumaDebugColor.MessageColor);
		if (FindObjectsOfType<MasterServerConect>().Length > 1)
		{
			Destroy(this.gameObject);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
		await InstanceNetworkRunner();
		await Connect("Room");

	}

	/// <summary>
	/// ルームに入る。ない場合は作る
	/// </summary>
	public async UniTask JoinOrCreateSession(string sessionName)
	{
		RPCManager.Instance.Rpc_ChangeRoomSessionName(Runner.LocalPlayer, sessionName);
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(Runner.LocalPlayer);
		int leaderIndex = currentRoom.LeaderIndex;
		await UniTask.WaitUntil(() => currentRoom.JoinRoomPlayer[leaderIndex].SessionName == sessionName);
		RoomManager.Instance.Initialize(Runner.LocalPlayer);
		NetworkRunner oldRunner = _networkRunner;
		await InstanceNetworkRunner();
		await Connect(sessionName);
		await oldRunner.Shutdown(true, ShutdownReason.HostMigration);
	}

	/// <summary>
	/// ネットワークランナーを生成してコールバック対象にする
	/// </summary>
	private async UniTask InstanceNetworkRunner()
	{
		// NetworkRunnerを生成する
		AsyncInstantiateOperation<NetworkRunner> objectTemp = InstantiateAsync(_networkRunnerPrefab);
		await objectTemp;
		_networkRunner = objectTemp.Result[0];
		// NetworkRunnerのコールバック対象に、このスクリプト（GameLauncher）を登録する
		_networkRunner.AddCallbacks(this);
		GateOfFusion.Instance.NetworkRunner = _networkRunner;
		XDebug.LogWarning("UpdateRunner", KumaDebugColor.SuccessColor);
	}

	public async UniTask Connect(string SessionName)
	{
		// "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
		StartGameResult result = await _networkRunner.StartGame(new StartGameArgs
		{
			//StartGameCancellationToken = destroyCancellationToken,
			GameMode = GameMode.Shared,
			SessionName = SessionName,
			SceneManager = _networkRunner.GetComponent<NetworkSceneManagerDefault>()
		}
		);
		_networkRunner.GetComponent<FusionVoiceClient>().PrimaryRecorder = _recorder;

		GateOfFusion.Instance.IsCanUsePhoton = result.Ok;

		XDebug.LogWarning("Connect:" + (result.Ok ? "Success" : "Fail"), KumaDebugColor.InformationColor);
	}

	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		XDebug.LogWarning($"JoinSession:{player}", KumaDebugColor.InformationColor);
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(Runner.LocalPlayer);
		RPCManager rpcManager = FindObjectOfType<RPCManager>();
		if (Runner.LocalPlayer != player && !Runner.IsSharedModeMasterClient) { return; }

		//ここから下はマスターのみ実行
		XDebug.LogWarning($"MasterJoin", KumaDebugColor.SuccessColor);
		MasterServerConect masterServer = FindObjectOfType<MasterServerConect>();
		Transform masterTransform = masterServer.transform;
		NetworkObject networkObject = Runner.Spawn(_rpcManagerPrefab);
		rpcManager = networkObject.GetComponent<RPCManager>();
		rpcManager.transform.parent = masterTransform;

		localRemoteReparation.RemoteViewCreate(Runner, Runner.LocalPlayer);
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		if (runner.TryGetPlayerObject(player, out NetworkObject avater))
		{
			runner.Despawn(avater);
		}
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	public void OnConnectedToServer(NetworkRunner runner)
	{
	}

	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
		XDebug.LogError("DisconnectedFromServer:", KumaDebugColor.ErrorColor);
	}

	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
	}

	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data)
	{
	}

	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}
}
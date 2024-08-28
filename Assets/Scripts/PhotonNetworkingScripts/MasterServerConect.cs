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
	[SerializeField, HideAtPlaying]
	private bool _isUsePhoton = true;

	private NetworkRunner _networkRunner;

	private RPCManager _rpcManager;

	public bool IsUsePhoton => _isUsePhoton;

	/// <summary>
	/// このクラスはランナーとの紐づけはしないためラップする
	/// </summary>
	public new NetworkRunner Runner
	{
		get
		{
			return _networkRunner;
		}
	}

	public RPCManager RPCManager => _rpcManager ??= FindObjectOfType<RPCManager>();

	public async UniTask<NetworkRunner> GetRunner()
	{
		if (_networkRunner == null)
		{
			_networkRunner = await InstanceNetworkRunner();
		}
		return _networkRunner;
	}

	public async UniTask<RPCManager> GetRPCManager()
	{
		if (_rpcManager != null) { return _rpcManager; }

		_rpcManager = FindObjectOfType<RPCManager>();
		if (_rpcManager == null)
		{
			XDebug.LogWarning($"rpcInstance", KumaDebugColor.SuccessColor);
			_networkRunner = await GetRunner();
			NetworkObject networkObjectTemp = await _networkRunner.SpawnAsync(_rpcManagerPrefab);
			_rpcManager = networkObjectTemp.GetComponent<RPCManager>();
		}
		return _rpcManager;
	}

	private async void Awake()
	{
		if (!_isUsePhoton)
		{
			Destroy(FindObjectOfType<RoomManager>().gameObject);
			return;
		}

		if (FindObjectsOfType<MasterServerConect>().Length > 1)
		{
			Destroy(this.gameObject);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
		_networkRunner = await InstanceNetworkRunner();
		await Connect("Room");
	}

	[ContextMenu("dadada")]
	private void AAA()
	{
		Debug.LogWarning(_networkRunner.IsSharedModeMasterClient);
	}

	/// <summary>
	/// ルームに入る。ない場合は作る
	/// </summary>
	public async UniTask JoinOrCreateSession(string sessionName)
	{
		if (!_isUsePhoton) { return; }
		(await GetRPCManager()).Rpc_ChangeRoomSessionName(Runner.LocalPlayer, sessionName);
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(Runner.LocalPlayer);
		int leaderIndex = currentRoom.LeaderIndex;
		XDebug.LogWarning($"{Runner.SessionInfo.PlayerCount}:{currentRoom.JoinRoomPlayer.Count}", KumaDebugColor.MessageColor);
		await UniTask.WaitUntil(() => currentRoom.WithNextSessionCount >= currentRoom.JoinRoomPlayer.Count - 1);
		RoomManager.Instance.Initialize(Runner.LocalPlayer);
		NetworkRunner oldRunner = _networkRunner;
		_networkRunner = await InstanceNetworkRunner();
		XDebug.LogWarning("JoinOrCreate", KumaDebugColor.MessageColor);
		await Connect(sessionName);
		XDebug.LogWarning("JoinOrCreate", KumaDebugColor.MessageColor);
		XDebug.LogWarning("JoinOrCreate", KumaDebugColor.MessageColor);
		await oldRunner.Shutdown(true, ShutdownReason.HostMigration);
		oldRunner = null;
		await UniTask.WaitUntil(() => oldRunner == null);
		XDebug.LogWarning("JoinOrCreate", KumaDebugColor.MessageColor);
	}

	/// <summary>
	/// ネットワークランナーを生成してコールバック対象にする
	/// </summary>
	private async UniTask<NetworkRunner> InstanceNetworkRunner()
	{
		// NetworkRunnerを生成する
		AsyncInstantiateOperation<NetworkRunner> objectTemp = InstantiateAsync(_networkRunnerPrefab);
		await objectTemp;
		NetworkRunner networkRunner = objectTemp.Result[0];
		// NetworkRunnerのコールバック対象に、このスクリプト（GameLauncher）を登録する
		networkRunner.AddCallbacks(this);
		GateOfFusion.Instance.NetworkRunner = networkRunner;
		XDebug.LogWarning("UpdateRunner", KumaDebugColor.SuccessColor);
		return networkRunner;
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
	public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		XDebug.LogWarning($"JoinSession:{player}", KumaDebugColor.InformationColor);

		if (Runner.LocalPlayer == player)
		{
			localRemoteReparation.RemoteViewCreate(Runner, Runner.LocalPlayer);
		}
		if (Runner.IsSharedModeMasterClient)
		{
			await GetRPCManager();
		}

	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		XDebug.LogWarning($"LeftSession:{player}", KumaDebugColor.InformationColor);
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
		XDebug.LogError($"DisconnectedFromServer:{reason}", KumaDebugColor.ErrorColor);
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

#if UNITY_EDITOR

#endif
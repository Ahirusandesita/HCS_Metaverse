using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using Photon.Voice.Unity;
using Photon.Voice.Fusion;

public interface IMasterServerConectable
{
	 void Connect();
}

public class MasterServerConect : NetworkBehaviour, INetworkRunnerCallbacks, IMasterServerConectable
{
	[SerializeField]
	private ActivityZone _activityZone = default;
	[SerializeField]
	private Recorder _recorder;
	[SerializeField]
	private NetworkRunner _networkRunnerPrefab;

	[SerializeField]
	private LocalRemoteSeparation localRemoteReparation;

	[SerializeField]
	private NetworkPrefabRef _rpcManagerPrefab;

	private NetworkRunner _networkRunner;

	private async void Awake()
	{
		_networkRunner = Instantiate(_networkRunnerPrefab);

		_networkRunner.GetComponent<FusionVoiceClient>().PrimaryRecorder = _recorder;
		_networkRunner.AddCallbacks(this);

		await _networkRunner.StartGame(new StartGameArgs
		{
			GameMode = GameMode.AutoHostOrClient,
			SessionName = "Room",
			SceneManager = _networkRunner.GetComponent<NetworkSceneManagerDefault>()
		}
		);

		RPCManager rpcManager = _networkRunner.Spawn(_rpcManagerPrefab, Vector3.zero, Quaternion.identity).GetComponent<RPCManager>();
		rpcManager.SessionNameChangedHandler += _activityZone.SetSessionName;

	}

	/// <summary>
	/// ネットワークランナーを更新する
	/// </summary>
	public async void UpdateNetworkRunner()
	{
		await _networkRunner.Shutdown(true,ShutdownReason.HostMigration);
		// NetworkRunnerを生成する
		_networkRunner = Instantiate(_networkRunnerPrefab);
		// NetworkRunnerのコールバック対象に、このスクリプト（GameLauncher）を登録する
		_networkRunner.AddCallbacks(this);

	}

	async void IMasterServerConectable.Connect()
	{
		// "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
		await _networkRunner.StartGame(new StartGameArgs
		{
			GameMode = GameMode.AutoHostOrClient,
			SessionName = "Room",
			SceneManager = _networkRunner.GetComponent<NetworkSceneManagerDefault>()
		}
		);
	}

	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		if (!_networkRunner.IsServer) { return; }
		localRemoteReparation.RemoteViewCreate(runner, player);
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
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
		runner.Shutdown(true, ShutdownReason.HostMigration);
		//新Runnerを生成する
		runner = Instantiate(_networkRunnerPrefab);
		runner.AddCallbacks(this);

		//新セッションに接続する
		runner.StartGame(new StartGameArgs
		{
			SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
			HostMigrationToken = hostMigrationToken,
		});
	}

	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,  System.ArraySegment<byte> data)
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

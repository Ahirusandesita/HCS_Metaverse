using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using Photon.Voice.Unity;
using Photon.Voice.Fusion;
using Cysharp.Threading.Tasks;

public interface IMasterServerConectable
{
	UniTask Connect(string SessionName);
}

public class MasterServerConect : NetworkBehaviour, INetworkRunnerCallbacks, IMasterServerConectable
{
	private bool _debugBool = false;

	[SerializeField]
	private NetworkPrefabRef _rpcManagerPrefab;
	[SerializeField]
	private Recorder _recorder;
	[SerializeField]
	private NetworkRunner _networkRunnerPrefab;

	private NetworkRunner _networkRunner;
	[SerializeField]
	private RegisterSceneInInspector activityName;

	[SerializeField]
	private NetworkObject _testNetworkObject;

	[SerializeField]
	private LocalRemoteSeparation localRemoteReparation;


	/// <summary>
	/// このクラスはランナーとの紐づけはしないためラップする
	/// </summary>
	public new NetworkRunner Runner { get => _networkRunner; }

	private async void Awake()
	{
		_networkRunner = Instantiate(_networkRunnerPrefab);
		_networkRunner.AddCallbacks(this);

		await Connect("Room");
	}

	[ContextMenu("ActivityStart")]
	private void Acaca()
	{
		ActivityStart();
	}
	public async void ActivityStart()
	{
		//アクティビティスタート
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(_networkRunner.LocalPlayer);
		if (currentRoom is null)
		{
			Debug.LogWarning("どのルームにも入っていません");
			return;
		}
		string sessionName = currentRoom.NextSessionName;
		foreach (Room.RoomPlayer roomPlayer in currentRoom.JoinRoomPlayer)
		{
			if (roomPlayer.PlayerData == _networkRunner.LocalPlayer) { continue; }
			Debug.LogWarning(roomPlayer.PlayerData);
			RPCManager.Instance.Rpc_JoinSession(sessionName, roomPlayer.PlayerData);
		}
		await UniTask.WaitUntil(() => currentRoom.WithLeaderSessionCount <= 0);
		JoinOrCreateSession(sessionName);
	}

	[ContextMenu("Grab")]
	private void TestTest()
	{
		GateOfFusion.Instance.Grab(_testNetworkObject);
	}

	[ContextMenu("")]
	private void TestTestTest()
	{
	}

	[ContextMenu("Test")]
	public void TestTestTestTest()
	{
		NetworkObject[] networkObjects = new NetworkObject[1];
		networkObjects[0] = _testNetworkObject;
		Runner.RegisterSceneObjects(Runner.GetSceneRef(_testNetworkObject.gameObject), networkObjects);
	}


	/// <summary>
	/// ルームに入る。ない場合は作る
	/// </summary>
	public async void JoinOrCreateSession(string sessionName)
	{
		RPCManager.Instance.Rpc_ChangeRoomSessionName(Runner.LocalPlayer, sessionName);
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(Runner.LocalPlayer);
		int leaderIndex = currentRoom.LeaderIndex;
		await UniTask.WaitUntil(() => currentRoom.JoinRoomPlayer[leaderIndex].SessionName == sessionName);
		RoomManager.Instance.Initialize(Runner.LocalPlayer);
		NetworkRunner oldRunner = _networkRunner;
		InstanceNetworkRunner();
		await Connect(sessionName);
		await oldRunner.Shutdown(true, ShutdownReason.HostMigration);
	}

	/// <summary>
	/// ネットワークランナーを生成してコールバック対象にする
	/// </summary>
	private void InstanceNetworkRunner()
	{
		// NetworkRunnerを生成する
		_networkRunner = Instantiate(_networkRunnerPrefab);
		// NetworkRunnerのコールバック対象に、このスクリプト（GameLauncher）を登録する
		_networkRunner.AddCallbacks(this);
		Debug.LogWarning("UpdateRunner");
	}

	public async UniTask Connect(string SessionName)
	{
		// "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
		await _networkRunner.StartGame(new StartGameArgs
		{
			//StartGameCancellationToken = destroyCancellationToken,
			GameMode = GameMode.Shared,
			SessionName = SessionName,
			SceneManager = _networkRunner.GetComponent<NetworkSceneManagerDefault>()
		}
		);
		_networkRunner.GetComponent<FusionVoiceClient>().PrimaryRecorder = _recorder;
		Debug.LogWarning("Connect");
	}

	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		

		Room currentRoom = RoomManager.Instance.GetCurrentRoom(Runner.LocalPlayer);
		if (currentRoom != null)
		{
			if (currentRoom.LeaderPlayerRef == player && Runner.IsSharedModeMasterClient)
			{
				Runner.SetMasterClient(currentRoom.LeaderPlayerRef);
			}
		}
		RPCManager rpcManager = FindObjectOfType<RPCManager>();
		if (Runner.LocalPlayer != player) { return; }

		if (Runner.IsSharedModeMasterClient)
		{
			//ここから下はマスターのみ実行
			Debug.LogWarning($"<color=yellow>MasterJoin</color>");
			if (rpcManager == null)
			{
				MasterServerConect masterServer = FindObjectOfType<MasterServerConect>();
				Transform masterTransform = masterServer.transform;
				NetworkObject networkObject = Runner.Spawn(_rpcManagerPrefab);
				rpcManager = networkObject.GetComponent<RPCManager>();
				rpcManager.transform.parent = masterTransform;
			}
		}
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

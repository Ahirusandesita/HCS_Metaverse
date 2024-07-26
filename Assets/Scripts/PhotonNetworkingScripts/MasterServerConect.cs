using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using Photon.Voice.Unity;
using Photon.Voice.Fusion;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public interface IMasterServerConectable
{
	UniTask Connect(string SessionName);
}

public class MasterServerConect : NetworkBehaviour, INetworkRunnerCallbacks, IMasterServerConectable
{
	[SerializeField]
	private NetworkPrefabRef _rpcManagerPrefab;
	[SerializeField]
	private Recorder _recorder;
	[SerializeField]
	private NetworkRunner _networkRunnerPrefab;

	private NetworkRunner _networkRunner;

	[SerializeField]
	private NetworkObject _testNetworkObject;

	[SerializeField]
	private LocalRemoteSeparation localRemoteReparation;
	[SerializeField]
	private RegisterSceneInInspector _sceneName;

	[SerializeField]
	private Text _text;
	/// <summary>
	/// このクラスはランナーとの紐づけはしないためラップする
	/// </summary>
	public new NetworkRunner Runner { get => _networkRunner; }

	private async void Awake()
	{
		_networkRunner = Instantiate(_networkRunnerPrefab);
		_networkRunner.AddCallbacks(this);
		if (FindObjectsOfType<MasterServerConect>().Length > 1)
		{
			Destroy(this.gameObject);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
		await Connect("Room");
	}

	[ContextMenu("Left")]
	private void TestTest()
	{
		RPCManager.Instance.Rpc_RoomLeftOrClose(Runner.LocalPlayer);
	}
	[ContextMenu("ActivityStart")]
	private void Test()
	{
		GateOfFusion.Instance.ActivityStart(_sceneName);
	}

	[ContextMenu("klkl")]
	private void TestTestTest()
	{
		Runner.LoadScene(_sceneName, LoadSceneMode.Single);
	}

	[ContextMenu("Test")]
	public void TestTestTestTest()
	{
		NetworkObject[] networkObjects = new NetworkObject[1];
		if (_testNetworkObject == null) { return; }
		networkObjects[0] = _testNetworkObject;
		Runner.RegisterSceneObjects(Runner.GetSceneRef(_testNetworkObject.gameObject), networkObjects);
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
		XDebug.LogWarning("UpdateRunner",KumaDebugColor.NotificationColor);
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
		_text.text = result.Ok ? "Success" : "fail" + "\n" + result.ShutdownReason + "\n" + result.ErrorMessage + "\n" + result.StackTrace;
		_networkRunner.GetComponent<FusionVoiceClient>().PrimaryRecorder = _recorder;
		XDebug.LogWarning("Connect",KumaDebugColor.NotificationColor);
	}

	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		XDebug.LogWarning($"JoinSession:{player}", KumaDebugColor.NotificationColor);
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(Runner.LocalPlayer);
		RPCManager rpcManager = FindObjectOfType<RPCManager>();
		if (Runner.LocalPlayer != player) { return; }
		if (Runner.IsSharedModeMasterClient)
		{
			//ここから下はマスターのみ実行
			Debug.LogWarning($"<color=yellow>MasterJoin</color>");
			MasterServerConect masterServer = FindObjectOfType<MasterServerConect>();
			Transform masterTransform = masterServer.transform;
			NetworkObject networkObject = Runner.Spawn(_rpcManagerPrefab);
			rpcManager = networkObject.GetComponent<RPCManager>();
			rpcManager.transform.parent = masterTransform;
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
		XDebug.LogError("DisconnectedFromServer:", Color.red);
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

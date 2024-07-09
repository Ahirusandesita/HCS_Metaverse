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
	[SerializeField]
	private ActivityZone _activityZone = default;
	[SerializeField]
	private Recorder _recorder;
	[SerializeField]
	private NetworkRunner _networkRunnerPrefab;

	[SerializeField]
	private LocalRemoteSeparation localRemoteReparation;
	[SerializeField]
	private NetworkPrefabRef _roomCounterPrefab;

	[SerializeField]
	private NetworkPrefabRef _rpcManagerPrefab;

	private NetworkRunner _networkRunner;
	[SerializeField]
	private RegisterSceneInInspector activityName;

	[SerializeField]
	private NetworkObject _testNetworkObject;

	public void CookActivityJoin(int roomNumber = -1)
	{
		RoomManager.Instance.JoinOrCreate(WorldType.UnderCook, _networkRunner.LocalPlayer,roomNumber);
	}



	private async void Awake()
	{
		_networkRunner = Instantiate(_networkRunnerPrefab);
		_networkRunner.AddCallbacks(this);

		await Connect("Room");
		InitRegisterNetwork();

		RPCManager rpcManager = _networkRunner.
			Spawn(_rpcManagerPrefab, Vector3.zero, Quaternion.identity).
			GetComponent<RPCManager>();
		if (_activityZone is not null)
		{
			rpcManager.SessionNameChangedHandler += JoinOrCreateSession;
		}

		Transform myTransform = transform;
		rpcManager.transform.parent = myTransform;
		_networkRunner.Spawn(_roomCounterPrefab).transform.parent = myTransform;

		localRemoteReparation.RemoteViewCreate(_networkRunner, _networkRunner.LocalPlayer);
	}

	private void InitRegisterNetwork()
	{
		NetworkObject[] networkObjects = FindObjectsOfType<NetworkObject>();
		_networkRunner.RegisterSceneObjects(_networkRunner.GetSceneRef(gameObject), networkObjects);
	}

	[ContextMenu("ActivityTrigger")]
	private void ActivityStartTrigger()
	{
		//アクティビティスタート
		string sessionName = "TestRoom";

		RPCManager.Instance.Rpc_JoinSession(sessionName);
		//ActivityStart(activityName);
	}

	[ContextMenu("Test")]
	private void TestTest()
	{
		int roomNumber = -1;
		RoomManager.Instance.JoinOrCreate(WorldType.UnderCook, _networkRunner.LocalPlayer, roomNumber);
	}

	[ContextMenu("Test2")]
	private void TestTestTest()
	{

	}

	/// <summary>
	/// 状態変更権限を自分のにする
	/// </summary>
	/// <param name="networkObject">取得したいオブジェクト</param>
	/// <returns>成功したか</returns>
	public async UniTask GetStateAuthority(NetworkObject networkObject)
	{
		if (networkObject.HasStateAuthority)
		{
			Debug.LogWarning("has");
			return;
		}
		if (networkObject.GetComponent<ReleaseStateAuthorityData>().IsNotReleaseStateAuthority)
		{
			return;
		}
		var token = destroyCancellationToken;
		RPCManager.Instance.Rpc_ReleaseStateAuthority(networkObject, networkObject.StateAuthority);
		await UniTask.WaitUntil(() => networkObject.StateAuthority == PlayerRef.None, cancellationToken: token);
		networkObject.RequestStateAuthority();
	}


	/// <summary>
	/// アクティビティを開始する
	/// </summary>
	/// <param name="sceneName">開始するアクティビティのシーン名</param>
	public void ActivityStart(string sceneName)
	{

		Debug.LogWarning("ActivityStart");
		_networkRunner.LoadScene(sceneName);
	}

	/// <summary>
	/// ルームに入る。ない場合は作る
	/// </summary>
	private async void JoinOrCreateSession(string sessionName)
	{
		await UpdateNetworkRunner();
		await Connect(sessionName);
	}

	/// <summary>
	/// ネットワークランナーを更新する
	/// </summary>
	private async UniTask UpdateNetworkRunner()
	{
		Debug.LogWarning("UpdateRunner");
		await _networkRunner.Shutdown(true, ShutdownReason.HostMigration);
		// NetworkRunnerを生成する
		_networkRunner = Instantiate(_networkRunnerPrefab);
		// NetworkRunnerのコールバック対象に、このスクリプト（GameLauncher）を登録する
		_networkRunner.AddCallbacks(this);

	}

	public async UniTask Connect(string SessionName)
	{
		// "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
		await _networkRunner.StartGame(new StartGameArgs
		{
			StartGameCancellationToken = destroyCancellationToken,
			GameMode = GameMode.Shared,
			SessionName = SessionName,
			SceneManager = _networkRunner.GetComponent<NetworkSceneManagerDefault>()
		}
		);
		_networkRunner.GetComponent<FusionVoiceClient>().PrimaryRecorder = _recorder;
	}

	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
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

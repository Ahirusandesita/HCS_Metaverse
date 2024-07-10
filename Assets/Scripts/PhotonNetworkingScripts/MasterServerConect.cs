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



	private async void Awake()
	{
		_networkRunner = Instantiate(_networkRunnerPrefab);
		_networkRunner.AddCallbacks(this);

		await Connect("Room");
		InitRegisterNetwork();

		Transform myTransform = transform;
		_networkRunner.Spawn(_roomCounterPrefab).transform.parent = myTransform;

		RPCManager rpcManager = _networkRunner.
			Spawn(_rpcManagerPrefab, Vector3.zero, Quaternion.identity).
			GetComponent<RPCManager>();
		if (_activityZone is not null)
		{
			rpcManager.SessionNameChangedHandler += JoinOrCreateSession;
		}

		rpcManager.transform.parent = myTransform;
		localRemoteReparation.RemoteViewCreate(_networkRunner, _networkRunner.LocalPlayer);
	}

	private void InitRegisterNetwork()
	{
		NetworkObject[] networkObjects = FindObjectsOfType<NetworkObject>();
		_networkRunner.RegisterSceneObjects(_networkRunner.GetSceneRef(gameObject), networkObjects);
	}

	[ContextMenu("ActivityStart")]
	public void ActivityStart()
	{
		//�A�N�e�B�r�e�B�X�^�[�g
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(_networkRunner.LocalPlayer);

		string sessionName = currentRoom.SessionName;

		foreach (PlayerRef playerRef in currentRoom.JoinPlayer)
		{
			RPCManager.Instance.Rpc_JoinSession(sessionName, playerRef);
		}
		_networkRunner.LoadScene(activityName);
	}

	[ContextMenu("Join")]
	private void TestTest()
	{
		Debug.LogWarning("join");
		int roomNumber = -1;
		RPCManager.Instance.Rpc_RoomJoinOrCreate(WorldType.UnderCook, _networkRunner.LocalPlayer, roomNumber);
	}

	[ContextMenu("Left")]
	private void TestTestTest()
	{
		Debug.LogWarning("left");
		RPCManager.Instance.Rpc_RoomLeftOrClose(_networkRunner.LocalPlayer);
	}

	[ContextMenu("Request")]
	private void TestTestTestTest()
	{
		Debug.LogWarning("Request");
		RPCManager.Instance.Rpc_RequestRoomData(_networkRunner.LocalPlayer);
	}

	/// <summary>
	/// ��ԕύX�����������̂ɂ���
	/// </summary>
	/// <param name="networkObject">�擾�������I�u�W�F�N�g</param>
	/// <returns>����������</returns>
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
	/// ���[���ɓ���B�Ȃ��ꍇ�͍��
	/// </summary>
	private async void JoinOrCreateSession(string sessionName)
	{
		await UpdateNetworkRunner();
		await Connect(sessionName);
	}

	/// <summary>
	/// �l�b�g���[�N�����i�[���X�V����
	/// </summary>
	private async UniTask UpdateNetworkRunner()
	{
		Debug.LogWarning("UpdateRunner");
		await _networkRunner.Shutdown(true, ShutdownReason.HostMigration);
		// NetworkRunner�𐶐�����
		_networkRunner = Instantiate(_networkRunnerPrefab);
		// NetworkRunner�̃R�[���o�b�N�ΏۂɁA���̃X�N���v�g�iGameLauncher�j��o�^����
		_networkRunner.AddCallbacks(this);

	}

	public async UniTask Connect(string SessionName)
	{
		// "Room"�Ƃ������O�̃��[���ɎQ������i���[�������݂��Ȃ���΍쐬���ĎQ������j
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
		//�VRunner�𐶐�����
		runner = Instantiate(_networkRunnerPrefab);
		runner.AddCallbacks(this);

		//�V�Z�b�V�����ɐڑ�����
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

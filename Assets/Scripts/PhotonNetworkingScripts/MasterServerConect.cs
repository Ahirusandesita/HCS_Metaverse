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


	/// <summary>
	/// ���̃N���X�̓����i�[�Ƃ̕R�Â��͂��Ȃ����߃��b�v����
	/// </summary>
	public new NetworkRunner Runner { get => _networkRunner; }

	private async void Awake()
	{
		_networkRunner = Instantiate(_networkRunnerPrefab);
		_networkRunner.AddCallbacks(this);

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
		XDebug.LogWarning(_sceneName + ":awaw", KumaDebugColor.TempColor);
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
	/// ���[���ɓ���B�Ȃ��ꍇ�͍��
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
	/// �l�b�g���[�N�����i�[�𐶐����ăR�[���o�b�N�Ώۂɂ���
	/// </summary>
	private void InstanceNetworkRunner()
	{
		// NetworkRunner�𐶐�����
		_networkRunner = Instantiate(_networkRunnerPrefab);
		// NetworkRunner�̃R�[���o�b�N�ΏۂɁA���̃X�N���v�g�iGameLauncher�j��o�^����
		_networkRunner.AddCallbacks(this);
		Debug.LogWarning("UpdateRunner");
	}

	public async UniTask Connect(string SessionName)
	{
		// "Room"�Ƃ������O�̃��[���ɎQ������i���[�������݂��Ȃ���΍쐬���ĎQ������j
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
		XDebug.LogWarning($"JoinSession:{player}", KumaDebugColor.NotificationColor);
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
			//�������牺�̓}�X�^�[�̂ݎ��s
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

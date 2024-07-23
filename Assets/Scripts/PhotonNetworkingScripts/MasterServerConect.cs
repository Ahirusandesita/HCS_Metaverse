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
	/// ���̃N���X�̓����i�[�Ƃ̕R�Â��͂��Ȃ����߃��b�v����
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
		//�A�N�e�B�r�e�B�X�^�[�g
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(_networkRunner.LocalPlayer);
		if (currentRoom is null)
		{
			Debug.LogWarning("�ǂ̃��[���ɂ������Ă��܂���");
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

	[ContextMenu("SendRpc")]
	private void TestTest()
	{
		Debug.LogWarning($"SendRpc:" +
			$"{RoomManager.Instance.GetCurrentRoom(Runner.LocalPlayer).JoinRoomPlayer[1].PlayerData}");

		RpcInvokeInfo rpcInvokeInfo = RPCManager.Instance
			.Rpc_Test(RoomManager.Instance
			.GetCurrentRoom(Runner.LocalPlayer)
			.JoinRoomPlayer[1].PlayerData);

		Debug.LogWarning($"{rpcInvokeInfo.LocalInvokeResult}:{rpcInvokeInfo.SendCullResult}:{rpcInvokeInfo.SendResult.Result}");
	}

	[ContextMenu("Left")]
	private void TestTestTest()
	{
		Debug.LogWarning("left");
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
		if (Runner.IsSharedModeMasterClient)
		{
			Runner.SetMasterClient(currentRoom.LeaderPlayerRef);
		}
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
	public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		if (runner.SessionInfo.PlayerCount == 2 && !_debugBool)
		{
			_debugBool = true;
			await UniTask.WaitForSeconds(1f);
			FindObjectOfType<TestGameZone>().Open();
		}

		if (Runner.LocalPlayer != player) { return; }
		//�������牺�͖{�l�̂ݎ��s
		localRemoteReparation.RemoteViewCreate(Runner, Runner.LocalPlayer);
		if(Runner.SessionInfo.PlayerCount  > 1)
		{
			RPCManager.Instance.Rpc_RequestRoomData(Runner.LocalPlayer);
		}

		if (!Runner.IsSharedModeMasterClient) { return; }
		//�������牺�̓}�X�^�[�̂ݎ��s
		Debug.LogWarning($"<color=yellow>MasterJoin</color>");
		RPCManager rpcManager = FindObjectOfType<RPCManager>();
		if (rpcManager != null) { return; }
		MasterServerConect masterServer = FindObjectOfType<MasterServerConect>();
		Transform masterTransform = masterServer.transform;
		NetworkObject networkObject = Runner.Spawn(_rpcManagerPrefab);
		rpcManager = networkObject.GetComponent<RPCManager>();
		rpcManager.transform.parent = masterTransform;

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

using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using Photon.Voice.Unity;
using Photon.Voice.Fusion;

public interface IMasterServerConectable
{
    void Connect();
}

public class MasterServerConect : NetworkBehaviour,INetworkRunnerCallbacks, IMasterServerConectable
{

	[SerializeField]
	private Recorder _recorder;
	[SerializeField]
	private NetworkRunner _networkRunnerPrefab;
	[SerializeField]
    private LocalRemoteSeparation localRemoteReparation;
	private NetworkRunner _networkRunner;

	[ContextMenu("awake")]
	private void Awake()
	{
		_networkRunner = (NetworkRunner)FindObjectOfType(typeof(NetworkRunner));
		if (_networkRunner is null)
		{
			_networkRunner = Instantiate(_networkRunnerPrefab);
		}
		else
		{
			Destroy(this.gameObject);
			return;
		}


		_networkRunner.GetComponent<FusionVoiceClient>().PrimaryRecorder = _recorder;
		_networkRunner.AddCallbacks(this);
		var a = this as IMasterServerConectable;
		a.Connect();
	}

	void IMasterServerConectable.Connect()
    {
		// "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
		_networkRunner.StartGame(new StartGameArgs
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
		localRemoteReparation.RemoteViewCreate(runner,player);
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
	}

	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
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

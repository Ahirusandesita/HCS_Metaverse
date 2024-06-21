using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;

public interface IMasterServerConectable
{
	void Connect();
}

public class MasterServerConect : MonoBehaviour, INetworkRunnerCallbacks, IMasterServerConectable
{
	[SerializeField]
	private LocalRemoteSeparation localRemoteReparation;

	[SerializeField]
	private NetworkRunner networkRunnerPrefab;

	private NetworkRunner _networkRunner;

	public void OnConnectedToServer(NetworkRunner runner)
	{
		throw new NotImplementedException();
	}

	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		throw new NotImplementedException();
	}

	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
		throw new NotImplementedException();
	}

	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
		throw new NotImplementedException();
	}

	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
		throw new NotImplementedException();
	}

	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		throw new NotImplementedException();
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		throw new NotImplementedException();
	}

	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
		throw new NotImplementedException();
	}

	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
		throw new NotImplementedException();
	}

	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
		throw new NotImplementedException();
	}

	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		localRemoteReparation.RemoteViewCreate(runner,player);
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		throw new NotImplementedException();
	}

	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
		throw new NotImplementedException();
	}

	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
		throw new NotImplementedException();
	}

	public void OnSceneLoadDone(NetworkRunner runner)
	{
		throw new NotImplementedException();
	}

	public void OnSceneLoadStart(NetworkRunner runner)
	{
		throw new NotImplementedException();
	}

	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
		throw new NotImplementedException();
	}

	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		throw new NotImplementedException();
	}

	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
		throw new NotImplementedException();
	}

	private void Awake()
	{
		
	}

	//public override void OnConnectedToMaster()
	//{
	//    // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
	//    PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
	//}

	//public override void OnJoinedRoom()
	//{
	//    localRemoteReparation.RemoteViewCreate();
	//}

	void IMasterServerConectable.Connect()
	{
		_networkRunner = Instantiate(networkRunnerPrefab);
		_networkRunner.AddCallbacks(this);

		// StartGameArgsに渡した設定で、セッションに参加する
		_networkRunner.StartGame(new StartGameArgs
		{
			GameMode = GameMode.AutoHostOrClient,
			SessionName = "TestRoom",
			SceneManager = _networkRunner.GetComponent<NetworkSceneManagerDefault>()
		}
	   );
		//PhotonNetwork.ConnectUsingSettings();
	}



}

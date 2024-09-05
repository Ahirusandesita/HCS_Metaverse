using UnityEngine;
using Fusion;
using Fusion.Sockets;
using Photon.Voice.Unity;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;

public class MasterServerConect : NetworkBehaviour, IMasterServerConectable
{
	[SerializeField]
	private NetworkPrefabRef _sessionRPCManagerPrefab;
	[SerializeField]
	private Recorder _recorder;
	[SerializeField]
	private NetworkRunner _networkRunnerPrefab;
	[SerializeField]
	private LocalRemoteSeparation localRemoteReparation;
	[SerializeField, HideAtPlaying]
	private bool _isUsePhoton = false;
	private NetworkRunner _networkRunner;
	private SessionRPCManager _sessionRPCManager;
	private bool _isConnected = default;
	public event Action OnConnect;
	public bool IsUsePhoton => _isUsePhoton;
	public bool IsConnected => _isConnected;
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
	public SessionRPCManager SessionRPCManager => _sessionRPCManager ??= FindObjectOfType<SessionRPCManager>();
	public async UniTask<NetworkRunner> GetRunnerAsync()
	{
		if (_networkRunner == null)
		{
			_networkRunner = await InstanceNetworkRunnerAsync();
		}
		return _networkRunner;
	}

	public async UniTask<SessionRPCManager> GetSessionRPCManagerAsync()
	{
		if (_sessionRPCManager != null) { return _sessionRPCManager; }
		_sessionRPCManager = FindObjectOfType<SessionRPCManager>();
		await UniTask.WaitUntil(() => _sessionRPCManager != null);
		return _sessionRPCManager;
	}

	public async UniTask<SessionRPCManager> InstanceSessionRPCManagerAsync()
	{
		XDebug.LogWarning($"InstanceRpcManager{_networkRunner.IsShutdown}", KumaDebugColor.ErrorColor);
		NetworkObject networkObjectTemp = await _networkRunner.SpawnAsync(_sessionRPCManagerPrefab);
		_sessionRPCManager = networkObjectTemp.GetComponent<SessionRPCManager>();
		return _sessionRPCManager;
	}

	private async void Awake()
	{
		if (FindObjectsOfType<MasterServerConect>().Length > 1)
		{
			Destroy(this.gameObject);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
		if (!_isUsePhoton)
		{
			return;
		}
		
		SceneNameType firstWorldType = SceneNameType.TestPhotonScene;
		_networkRunner = await InstanceNetworkRunnerAsync();
		await Connect(firstWorldType.ToString());
		RoomManager.Instance.JoinOrCreate(firstWorldType, Runner.LocalPlayer, Runner.SessionInfo.Name);
	}

	public async UniTask Disconnect()
	{
		XDebug.LogWarning($"Disconnect", KumaDebugColor.SuccessColor);
		if (!_isUsePhoton) { return; }
		if (_networkRunner == null)
		{
			XDebug.LogWarning($"Runnerがnullです", KumaDebugColor.ErrorColor);
			return;
		}
		await _networkRunner.Shutdown(true, ShutdownReason.Ok);
		await UniTask.WaitUntil(() => _networkRunner == null);
	}

	/// <summary>
	/// セッションに入る。ない場合は作る
	/// </summary>
	public async UniTask JoinOrCreateSession(string sessionName)
	{
		if (!_isUsePhoton) { return; }
		if (_networkRunner != null)
		{
			XDebug.LogWarning($"Runnerが破棄されていません", KumaDebugColor.ErrorColor);
			await Disconnect();
		}
		RoomManager.Instance.Initialize(Runner.LocalPlayer);
		_networkRunner = await InstanceNetworkRunnerAsync();
		await Connect(sessionName);

		if (_networkRunner.SessionInfo.PlayerCount > 1)
		{
			(await GetSessionRPCManagerAsync()).Rpc_ChangeRoomSessionName(Runner.LocalPlayer, sessionName);
		}
		else
		{
			RoomManager.Instance.ChangeSessionName(Runner.LocalPlayer, sessionName);
		}
	}

	/// <summary>
	/// ネットワークランナーを生成する
	/// </summary>
	private async UniTask<NetworkRunner> InstanceNetworkRunnerAsync()
	{
		// NetworkRunnerを生成する
		AsyncInstantiateOperation<NetworkRunner> objectTemp = InstantiateAsync(_networkRunnerPrefab);
		await objectTemp;
		NetworkRunner networkRunner = objectTemp.Result[0];
		GateOfFusion.Instance.NetworkRunner = networkRunner;
		NetworkEvents events = networkRunner.GetComponent<NetworkEvents>();
		events.PlayerJoined.AddListener(OnPlayerJoined);
		events.PlayerLeft.AddListener(OnPlayerLeft);
		events.OnShutdown.AddListener(OnShutdown);
		events.OnConnectedToServer.AddListener(OnConnectedToServer);
		XDebug.LogWarning("UpdateRunner", KumaDebugColor.SuccessColor);
		return networkRunner;
	}

	public async UniTask Connect(string sessionName)
	{
		NetworkSceneInfo networkSceneInfo = default;

		networkSceneInfo.AddSceneRef(SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex));

		StartGameArgs startGameArgs = new StartGameArgs
		{
			GameMode = GameMode.Shared,
			SessionName = sessionName,
			SceneManager = _networkRunner.GetComponent<NetworkSceneManagerDefault>(),
			Scene = networkSceneInfo,
		};
		//ルームに参加する（ルームが存在しなければ作成して参加する）
		StartGameResult result = await _networkRunner.StartGame(startGameArgs);

		XDebug.LogWarning("Connect:" + (result.Ok ? "Success" : "Fail"), KumaDebugColor.InformationColor);

	}

	private async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		XDebug.LogWarning($"JoinSession:{player}", KumaDebugColor.InformationColor);

		if (Runner.LocalPlayer == player)
		{
			localRemoteReparation.RemoteViewCreate(Runner, Runner.LocalPlayer);
			if (Runner.IsSharedModeMasterClient)
			{
				await InstanceSessionRPCManagerAsync();
			}
		}
	}
	private void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		XDebug.LogWarning($"LeftSession:{player}", KumaDebugColor.InformationColor);
		if (runner.TryGetPlayerObject(player, out NetworkObject avater))
		{
			runner.Despawn(avater);
		}
	}


	private void OnConnectedToServer(NetworkRunner runner)
	{
		XDebug.LogWarning("OnConnectedToServer",KumaDebugColor.MessageColor);
		OnConnect?.Invoke();
		_isConnected = true;
	}

	private void OnShutdown(NetworkRunner runner, ShutdownReason reason)
	{
		XDebug.LogWarning("OnDisconnectedFromServer", KumaDebugColor.MessageColor);
		_isConnected = false;
	}
}
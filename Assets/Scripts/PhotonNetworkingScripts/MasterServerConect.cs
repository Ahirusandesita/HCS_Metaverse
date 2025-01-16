using UnityEngine;
using Fusion;
using Fusion.Sockets;
using Photon.Voice.Unity;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;
using KumaDebug;

public class MasterServerConect : NetworkBehaviour, IMasterServerConectable
{
	[SerializeField]
	private NetworkPrefabRef _sessionRPCManagerPrefab;
	[SerializeField]
	private CharacterRPCManager _characterRPCManagerPrefab;
	[SerializeField]
	private Recorder _recorder;
	[SerializeField]
	private NetworkRunner _networkRunnerPrefab;
	[SerializeField]
	private LocalRemoteSeparation localRemoteReparation;
	[SerializeField, HideAtPlaying]
	private bool _isUsePhoton = true;
	private NetworkRunner _networkRunner;
	private SessionRPCManager _sessionRPCManager;
	private bool _isConnected = default;	
	private bool _isRoomStandby = false;
	private bool _isActivityConnected = false;
	public event Action OnConnect;
	public bool IsActivityConnected => _isActivityConnected;
	public bool IsUsePhoton => _isUsePhoton;
	public bool IsConnected => _isConnected;
	public bool IsSolo => Runner.SessionInfo.PlayerCount <= 1;
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
	public bool IsRoomStandBy => _isRoomStandby;
	public SessionRPCManager SessionRPCManager => _sessionRPCManager ??= FindObjectOfType<SessionRPCManager>();
	#region EditorOnly
#if UNITY_EDITOR
	[SerializeField, HideAtPlaying]
	private bool _isKumaDebug = false;
	public bool IsKumaDebug => _isKumaDebug;
#endif
	#endregion
	
	[ContextMenu("start")]
	private void ActivityS()
	{
		//testObj.ReleaseStateAuthority();
		GateOfFusion.Instance.ActivityStart();
	}

	public void IsRoomStandbyOn()
	{
		XKumaDebugSystem.LogError("roomOn");
		_isRoomStandby = true;
	}
	public void IsActivityConnectedON()
	{
		_isActivityConnected = true;
	}

	/// <summary>
	/// SessionRPCManagerを取得する取得できない場合はできるまでまつ
	/// </summary>
	/// <returns>取得したオブジェクト</returns>
	public async UniTask<SessionRPCManager> GetSessionRPCManagerAsync()
	{
		if (_sessionRPCManager != null) { return _sessionRPCManager; }
		//キャッシュされていない場合
		await UniTask.WaitUntil(() => (_sessionRPCManager = FindObjectOfType<SessionRPCManager>()) != null);
		return _sessionRPCManager;
	}

	/// <summary>
	/// SessionRPCManagerを生成する
	/// </summary>
	/// <returns>生成したオブジェクト</returns>
	public async UniTask<SessionRPCManager> InstanceSessionRPCManagerAsync()
	{
		XKumaDebugSystem.LogWarning($"InstanceRpcManager{_networkRunner.IsShutdown}", KumaDebugColor.ErrorColor);
		NetworkObject networkObjectTemp = await _networkRunner.SpawnAsync(_sessionRPCManagerPrefab);
		_sessionRPCManager = networkObjectTemp.GetComponent<SessionRPCManager>();
		return _sessionRPCManager;
	}

	/// <summary>
	/// SessionRPCManagerを生成する
	/// </summary>
	/// <returns>生成したオブジェクト</returns>
	public async UniTask<CharacterRPCManager> InstanceCharacterRPCManagerAsync()
	{
		return await GateOfFusion.Instance.SpawnAsync(_characterRPCManagerPrefab);
	}

	private async void Awake()
	{
		SceneNameType firstScene = SceneNameType.TestPhotonScene;

		if (FindObjectsOfType<MasterServerConect>().Length > 1)
		{
			Destroy(this.gameObject);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
		_networkRunner = await InstanceNetworkRunnerAsync();
		if (!_isUsePhoton)
		{
			await RoomManager.Instance.JoinOrCreate(firstScene, Runner.LocalPlayer);
			return;
		}
		await Connect(firstScene.ToString());
		_isRoomStandby = true;
	}

	public async UniTask Disconnect()
	{
		XKumaDebugSystem.LogError("room:disc");
		_isRoomStandby = false;
		XKumaDebugSystem.LogWarning($"Disconnect", KumaDebugColor.SuccessColor);
		if (!_isUsePhoton) { return; }
		if (_networkRunner == null)
		{
			XKumaDebugSystem.LogWarning($"Runnerがnullです", KumaDebugColor.ErrorColor);
			return;
		}
		await _networkRunner.Shutdown(true, ShutdownReason.Ok);
		_isActivityConnected = false;
		await UniTask.WaitUntil(() => _networkRunner == null);
	}

	/// <summary>
	/// セッションに入る。ない場合は作る
	/// </summary>
	public async UniTask JoinOrCreateSession(string sessionName, PlayerRef executePlayer)
	{
		if (!_isUsePhoton) { return; }
		RoomManager.Instance.Initialize(executePlayer);
		if (_networkRunner != null)
		{
			XKumaDebugSystem.LogWarning($"Runnerが破棄されていません", KumaDebugColor.ErrorColor);
			await Disconnect();
		}
		_networkRunner = await InstanceNetworkRunnerAsync();
		await Connect(sessionName);

		if (_networkRunner.SessionInfo.PlayerCount > 1)
		{
			(await GetSessionRPCManagerAsync()).Rpc_ChangeRoomSessionName(executePlayer, sessionName);
		}
		else
		{
			RoomManager.Instance.ChangeSessionName(executePlayer, sessionName);
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
		events.OnDisconnectedFromServer.AddListener(OnDisconnectedFromMasterServer);
		events.OnShutdown.AddListener(OnShutdown);
		events.OnConnectedToServer.AddListener(OnConnectedToServer);
		XKumaDebugSystem.LogWarning("UpdateRunner", KumaDebugColor.SuccessColor);
		return networkRunner;
	}

	public async UniTask Connect(string sessionName)
	{
		NetworkSceneInfo networkSceneInfo = default;
		XKumaDebugSystem.LogWarning($"セッション名：{sessionName}", KumaDebugColor.ErrorColor);
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

		XKumaDebugSystem.LogWarning("Connect:" + (result.Ok ? "Success" : "Fail"), KumaDebugColor.InformationColor);
	}

	private async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		XKumaDebugSystem.LogWarning($"JoinSession:{player}", KumaDebugColor.InformationColor);

		if (Runner.LocalPlayer == player)
		{
			localRemoteReparation.RemoteViewCreate(Runner, Runner.LocalPlayer);
			if (Runner.IsSharedModeMasterClient)
			{
				await InstanceSessionRPCManagerAsync();
				await InstanceCharacterRPCManagerAsync();
			}
		}
	}
	private void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		XKumaDebugSystem.LogWarning($"LeftSession:{player}", KumaDebugColor.InformationColor);
		if (runner.TryGetPlayerObject(player, out NetworkObject avater))
		{
			runner.Despawn(avater);
		}
	}

	private void OnConnectedToServer(NetworkRunner runner)
	{
		XKumaDebugSystem.LogWarning("OnConnectedToServer", KumaDebugColor.MessageColor);
		OnConnect?.Invoke();
		_isConnected = true;
	}

	private void OnDisconnectedFromMasterServer(NetworkRunner runner, NetDisconnectReason reason)
	{
		_ = RoomManager.Instance.LeftOrClose(runner.LocalPlayer);
		XKumaDebugSystem.LogWarning($"OnDisconnectedFromMasterServer:{reason}", KumaDebugColor.MessageColor);
	}

	private void OnShutdown(NetworkRunner runner, ShutdownReason reason)
	{
		XKumaDebugSystem.LogWarning($"OnShutdown:{reason}", KumaDebugColor.MessageColor);
		_isConnected = false;
	}
}
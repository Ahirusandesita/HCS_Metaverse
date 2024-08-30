using UnityEngine;
using Fusion;
using Photon.Voice.Unity;
using Cysharp.Threading.Tasks;

public class MasterServerConect : NetworkBehaviour, IMasterServerConectable
{

	[SerializeField]
	private NetworkPrefabRef _rpcManagerPrefab;
	[SerializeField]
	private Recorder _recorder;
	[SerializeField]
	private NetworkRunner _networkRunnerPrefab;
	[SerializeField]
	private LocalRemoteSeparation localRemoteReparation;
	[SerializeField, HideAtPlaying]
	private bool _isUsePhoton = true;

	private NetworkRunner _networkRunner;

	private SessionRPCManager _sessionRpcManager;

	public bool IsUsePhoton => _isUsePhoton;

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

	public SessionRPCManager SessionRPCManager => _sessionRpcManager ??= FindObjectOfType<SessionRPCManager>();

	public async UniTask<NetworkRunner> GetRunnerAsync()
	{
		if (_networkRunner == null)
		{
			_networkRunner = await InstanceNetworkRunnerAsync();
		}
		return _networkRunner;
	}

	public async UniTask<SessionRPCManager> GetRPCManagerAsync()
	{
		if (_sessionRpcManager != null) { return _sessionRpcManager; }

		_sessionRpcManager = FindObjectOfType<SessionRPCManager>();
		if (_sessionRpcManager == null)
		{
			_networkRunner = await GetRunnerAsync();
			_sessionRpcManager = await InstanceRpcManagerAsync();
		}
		return _sessionRpcManager;
	}

	public async UniTask<SessionRPCManager> InstanceRpcManagerAsync()
	{
		SessionRPCManager rpcManagerTemp;
		NetworkObject networkObjectTemp = await _networkRunner.SpawnAsync(_rpcManagerPrefab);
		rpcManagerTemp = networkObjectTemp.GetComponent<SessionRPCManager>();
		return rpcManagerTemp;
	}

	private async void Awake()
	{
		if (!_isUsePhoton)
		{
			Destroy(FindObjectOfType<RoomManager>().gameObject);
			return;
		}

		if (FindObjectsOfType<MasterServerConect>().Length > 1)
		{
			Destroy(this.gameObject);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
		_networkRunner = await InstanceNetworkRunnerAsync();
		
		await Connect("Room");

	}

	/// <summary>
	/// セッションに入る。ない場合は作る
	/// </summary>
	public async UniTask JoinOrCreateSession(string sessionName)
	{
		if (!_isUsePhoton) { return; }
		(await GetRPCManagerAsync()).Rpc_ChangeRoomSessionName(Runner.LocalPlayer, sessionName);
		RoomManager.Instance.Initialize(Runner.LocalPlayer);
		NetworkRunner oldRunner = _networkRunner;
		_networkRunner = await InstanceNetworkRunnerAsync();
		await Connect(sessionName);
		await oldRunner.Shutdown(true, ShutdownReason.HostMigration);
	}

	/// <summary>
	/// ネットワークランナーを生成してコールバック対象にする
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
		XDebug.LogWarning("UpdateRunner", KumaDebugColor.SuccessColor);
		return networkRunner;
	}

	public async UniTask Connect(string SessionName)
	{
		//ルームに参加する（ルームが存在しなければ作成して参加する）
		StartGameResult result = await _networkRunner.StartGame(new StartGameArgs
		{
			GameMode = GameMode.Shared,
			SessionName = SessionName,
			SceneManager = _networkRunner.GetComponent<NetworkSceneManagerDefault>()
		}
		);

		XDebug.LogWarning("Connect:" + (result.Ok ? "Success" : "Fail"), KumaDebugColor.InformationColor);
	}

	public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		XDebug.LogWarning($"JoinSession:{player}", KumaDebugColor.InformationColor);

		if (Runner.LocalPlayer == player)
		{
			localRemoteReparation.RemoteViewCreate(Runner, Runner.LocalPlayer);
		}
		if (Runner.IsSharedModeMasterClient)
		{
			await GetRPCManagerAsync();
		}

	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		XDebug.LogWarning($"LeftSession:{player}", KumaDebugColor.InformationColor);
		if (runner.TryGetPlayerObject(player, out NetworkObject avater))
		{
			runner.Despawn(avater);
		}
	}
}
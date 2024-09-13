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
	public bool IsSolo => Runner.SessionInfo.PlayerCount <= 1;
	/// <summary>
	/// ���̃N���X�̓����i�[�Ƃ̕R�Â��͂��Ȃ����߃��b�v����
	/// </summary>
	public new NetworkRunner Runner
	{
		get
		{
			return _networkRunner;
		}
	}
	public SessionRPCManager SessionRPCManager => _sessionRPCManager ??= FindObjectOfType<SessionRPCManager>();
#if UNITY_EDITOR
	[SerializeField, HideAtPlaying]
	private bool _isKumaDebug = false;
	public bool IsKumaDebug => _isKumaDebug;
#endif


	public async UniTask<SessionRPCManager> GetSessionRPCManagerAsync()
	{
		if (_sessionRPCManager != null) { return _sessionRPCManager; }
		_sessionRPCManager = FindObjectOfType<SessionRPCManager>();
		await UniTask.WaitUntil(() => _sessionRPCManager != null);
		return _sessionRPCManager;
	}

	public async UniTask<SessionRPCManager> InstanceSessionRPCManagerAsync()
	{
		XKumaDebugSystem.LogWarning($"InstanceRpcManager{_networkRunner.IsShutdown}", KumaDebugColor.ErrorColor);
		NetworkObject networkObjectTemp = await _networkRunner.SpawnAsync(_sessionRPCManagerPrefab);
		_sessionRPCManager = networkObjectTemp.GetComponent<SessionRPCManager>();
		return _sessionRPCManager;
	}

	private async void Awake()
	{
		SceneNameType firstScene = SceneNameType.KumaKumaTest;

		if (FindObjectsOfType<MasterServerConect>().Length > 1)
		{
			Destroy(this.gameObject);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
		_networkRunner = await InstanceNetworkRunnerAsync();
		if (!_isUsePhoton)
		{
			await RoomManager.Instance.JoinOrCreate(firstScene, Runner.LocalPlayer, Runner.SessionInfo.Name);
			return;
		}

		await Connect(firstScene.ToString());
		
	}

	public async UniTask Disconnect()
	{
		XKumaDebugSystem.LogWarning($"Disconnect", KumaDebugColor.SuccessColor);
		if (!_isUsePhoton) { return; }
		if (_networkRunner == null)
		{
			XKumaDebugSystem.LogWarning($"Runner��null�ł�", KumaDebugColor.ErrorColor);
			return;
		}
		await _networkRunner.Shutdown(true, ShutdownReason.Ok);
		await UniTask.WaitUntil(() => _networkRunner == null);
	}

	/// <summary>
	/// �Z�b�V�����ɓ���B�Ȃ��ꍇ�͍��
	/// </summary>
	public async UniTask JoinOrCreateSession(string sessionName, PlayerRef executePlayer)
	{
		if (!_isUsePhoton) { return; }
		RoomManager.Instance.Initialize(executePlayer);
		if (_networkRunner != null)
		{
			XKumaDebugSystem.LogWarning($"Runner���j������Ă��܂���", KumaDebugColor.ErrorColor);
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
	/// �l�b�g���[�N�����i�[�𐶐�����
	/// </summary>
	private async UniTask<NetworkRunner> InstanceNetworkRunnerAsync()
	{
		// NetworkRunner�𐶐�����
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

		networkSceneInfo.AddSceneRef(SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex));

		StartGameArgs startGameArgs = new StartGameArgs
		{
			GameMode = GameMode.Shared,
			SessionName = sessionName,
			SceneManager = _networkRunner.GetComponent<NetworkSceneManagerDefault>(),
			Scene = networkSceneInfo,
		};
		//���[���ɎQ������i���[�������݂��Ȃ���΍쐬���ĎQ������j
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
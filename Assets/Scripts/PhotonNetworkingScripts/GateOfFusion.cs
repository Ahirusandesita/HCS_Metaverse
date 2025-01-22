using UnityEngine.SceneManagement;
using UnityEngine;
using Fusion;
using System.Linq;
using Cysharp.Threading.Tasks;
using KumaDebug;

public class GateOfFusion
{
	private NetworkRunner _networkRunner = default;
	private MasterServerConect _masterServer = default;
	private static GateOfFusion _instance = default;
	private SyncResult _syncResult = SyncResult.Complete;
	public static GateOfFusion Instance => _instance ??= new GateOfFusion();
	public bool IsActivityConnected => MasterServer.IsActivityConnected;
	public event System.Action OnConnect;
	public event System.Action OnActivityConnected;
	public event System.Action OnShutdown;


	public GateOfFusion()
	{
		MasterServer.OnConnect += () => OnConnect?.Invoke();
		MasterServer.OnShutdownEvent += () => OnShutdown?.Invoke();
	}
	private MasterServerConect MasterServer
	{
		get
		{
			_masterServer ??= Object.FindObjectOfType<MasterServerConect>();
			_masterServer ??= new GameObject("Master").AddComponent<MasterServerConect>();
			return _masterServer;
		}
	}
	public bool IsUsePhoton { get => MasterServer.IsUsePhoton; }
	public NetworkRunner NetworkRunner
	{
		get
		{
			if (_networkRunner == null)
			{
				_networkRunner = Object.FindObjectOfType<NetworkRunner>();
			}
			return _networkRunner;
		}
		set
		{
			_networkRunner = value;
		}
	}

	#region Spawn_Despawn
	public void Despawn<T>(T despawnObject) where T : Component
	{
		if (!MasterServer.IsUsePhoton)
		{
			Object.Destroy(despawnObject.gameObject);
			return;
		}
		XKumaDebugSystem.LogWarning($"Despawn:{despawnObject.gameObject}", KumaDebugColor.ErrorColor);
		if (despawnObject.TryGetComponent(out NetworkObject networkObject))
		{
			NetworkRunner.Despawn(networkObject);
			return;
		}
		XKumaDebugSystem.LogError("NetworkObjectが取得できませんでした。なのでDestroyします。", KumaDebugColor.ErrorColor);
		Object.Destroy(despawnObject.gameObject);
	}

	public async UniTask<T> SpawnAsync<T>(T prefab, Vector3 position = default, Quaternion quaternion = default, Transform parent = default) where T : Component
	{
		T temp;
		if (!MasterServer.IsUsePhoton)
		{
			temp = Object.Instantiate(prefab, position, quaternion);
		}
		else if (prefab.TryGetComponent(out NetworkObject networkObject))
		{
			await UniTask.WaitUntil(() => MasterServer.IsConnected);
			temp = (await NetworkRunner.SpawnAsync(networkObject, position, quaternion)).GetComponent<T>();
		}
		else
		{
			XKumaDebugSystem.LogError("NetworkObjectが取得できませんでした。なのでInstantiateします。", KumaDebugColor.ErrorColor);
			temp = Object.Instantiate(prefab, position, quaternion);
		}
		temp.transform.SetParent(parent);
		return temp;
	}

	public async UniTask<GameObject> SpawnAsync(GameObject prefab, Vector3 position = default, Quaternion quaternion = default, Transform parent = default)
	{
		GameObject temp;
		if (!MasterServer.IsUsePhoton)
		{
			temp = Object.Instantiate(prefab, position, quaternion);
		}
		else if (prefab.TryGetComponent(out NetworkObject networkObject))
		{
			await UniTask.WaitUntil(() => MasterServer.IsConnected);
			temp = (await NetworkRunner.SpawnAsync(networkObject, position, quaternion)).gameObject;
		}
		else
		{
			XKumaDebugSystem.LogError("NetworkObjectが取得できませんでした。なのでInstantiateします。", KumaDebugColor.ErrorColor);
			temp = Object.Instantiate(prefab, position, quaternion);
		}
		temp.transform.SetParent(parent);
		return temp;
	}
	#endregion

	public async void ActivityStart()
	{
		XKumaDebugSystem.LogWarning("アクティビティスタート", KumaDebugColor.MessageColor);
		if (_syncResult != SyncResult.Complete)
		{
			Debug.LogWarning("移動中です");
			return;
		}
		RoomManager.Instance.DestroyActivityStartUI();
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(NetworkRunner.LocalPlayer);
		if (currentRoom == null)
		{
			XKumaDebugSystem.LogWarning("部屋に所属していません", KumaDebugColor.WarningColor);
			return;
		}
		string sceneName = currentRoom.SceneNameType.ToString();
		if (SceneManager.GetActiveScene().name == sceneName)
		{
			XKumaDebugSystem.LogWarning("現在いるシーンです", KumaDebugColor.WarningColor);
			return;
		}
		_syncResult = SyncResult.Connecting;
		if (!MasterServer.IsUsePhoton)
		{
			PlayerDontDestroyData.Instance.PreviousScene = sceneName;
			SceneManager.LoadScene(sceneName);
			_syncResult = SyncResult.Complete;
			return;
		}
		if (currentRoom == null)
		{
			XKumaDebugSystem.LogWarning("どのルームにも入っていません", KumaDebugColor.ErrorColor);
			_syncResult = SyncResult.Complete;
			return;
		}
		if (currentRoom.LeaderPlayerRef != NetworkRunner.LocalPlayer)
		{
			XKumaDebugSystem.LogWarning("リーダーではありません", KumaDebugColor.ErrorColor);
			_syncResult = SyncResult.Complete;
			return;
		}
		
		string sessionName = currentRoom.NextSessionName;
		PlayerRef localPlayerRef = NetworkRunner.LocalPlayer;
		foreach (PlayerRef roomPlayer in currentRoom.JoinRoomPlayer)
		{
			if (roomPlayer == NetworkRunner.LocalPlayer) { continue; }
			MasterServer.SessionRPCManager.Rpc_JoinSession(sessionName, sceneName, roomPlayer);
			XKumaDebugSystem.LogWarning($"{roomPlayer}を移動させた", KumaDebugColor.MessageColor);
			await UniTask.WaitUntil(() => !NetworkRunner.ActivePlayers.Contains(roomPlayer));
		}

		XKumaDebugSystem.LogWarning($"全員移動させた", KumaDebugColor.MessageColor);
		await MasterServer.Disconnect();
		XKumaDebugSystem.LogWarning($"切断した", KumaDebugColor.MessageColor);
		await SceneManager.LoadSceneAsync(sceneName);
		if (PlayerDontDestroyData.Instance != null)
		{
			PlayerDontDestroyData.Instance.PreviousScene = sceneName;
		}
		XKumaDebugSystem.LogWarning($"シーンを読み込んだ");
		await MasterServer.JoinOrCreateSession(sessionName, localPlayerRef);
		XKumaDebugSystem.LogWarning($"自分がセッション移動した", KumaDebugColor.MessageColor);
		_syncResult = SyncResult.Complete;
		XKumaDebugSystem.LogWarning($"移動終了", KumaDebugColor.MessageColor);
		await UniTask.WaitUntil(() => NetworkRunner != null);

		foreach (PlayerRef roomPlayer in currentRoom.JoinRoomPlayer)
		{
			await UniTask.WaitUntil(() => NetworkRunner.ActivePlayers.Contains(roomPlayer));
		}
		XKumaDebugSystem.LogWarning("全員到着した");


		MasterServer.SessionRPCManager.Rpc_RoomStandbyOn();
		if (currentRoom.SceneNameType is not SceneNameType.KumaKumaTest or SceneNameType.TestPhotonScene)
		{
			_masterServer.SessionRPCManager.Rpc_ExecuteOnActivityConnedted();
		}
	}
	public void ExecuteOnActivityConnected()
	{
		OnActivityConnected?.Invoke();
		MasterServer.IsActivityConnectedON();
	}

	public async void ReturnMainRoom()
	{
		XKumaDebugSystem.LogWarning("アクティビティスタート", KumaDebugColor.MessageColor);
		if (_syncResult != SyncResult.Complete)
		{
			Debug.LogWarning("移動中です");
			return;
		}

		string sceneName = SceneNameType.TestPhotonScene.ToString();
		if (SceneManager.GetActiveScene().name == sceneName)
		{
			XKumaDebugSystem.LogWarning("現在いるシーンです", KumaDebugColor.WarningColor);
			return;
		}
		_syncResult = SyncResult.Connecting;
		if (!MasterServer.IsUsePhoton)
		{
			SceneManager.LoadScene(sceneName);
			_syncResult = SyncResult.Complete;
			return;
		}
		string sessionName = $"{sceneName}:0";
		PlayerRef localPlayerRef = NetworkRunner.LocalPlayer;

		await MasterServer.Disconnect();
		XKumaDebugSystem.LogWarning($"切断した", KumaDebugColor.MessageColor);
		await SceneManager.LoadSceneAsync(sceneName);
		XKumaDebugSystem.LogWarning($"シーンを読み込んだ");
		await MasterServer.JoinOrCreateSession(sessionName, localPlayerRef);
		XKumaDebugSystem.LogWarning($"自分がセッション移動した", KumaDebugColor.MessageColor);
		_syncResult = SyncResult.Complete;
		XKumaDebugSystem.LogWarning($"移動終了", KumaDebugColor.MessageColor);
		await UniTask.WaitUntil(() => NetworkRunner != null);
	}
}


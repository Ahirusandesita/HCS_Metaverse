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
		XKumaDebugSystem.LogError("NetworkObject���擾�ł��܂���ł����B�Ȃ̂�Destroy���܂��B", KumaDebugColor.ErrorColor);
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
			XKumaDebugSystem.LogError("NetworkObject���擾�ł��܂���ł����B�Ȃ̂�Instantiate���܂��B", KumaDebugColor.ErrorColor);
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
			XKumaDebugSystem.LogError("NetworkObject���擾�ł��܂���ł����B�Ȃ̂�Instantiate���܂��B", KumaDebugColor.ErrorColor);
			temp = Object.Instantiate(prefab, position, quaternion);
		}
		temp.transform.SetParent(parent);
		return temp;
	}
	#endregion

	public async void ActivityStart()
	{
		XKumaDebugSystem.LogWarning("�A�N�e�B�r�e�B�X�^�[�g", KumaDebugColor.MessageColor);
		if (_syncResult != SyncResult.Complete)
		{
			Debug.LogWarning("�ړ����ł�");
			return;
		}
		RoomManager.Instance.DestroyActivityStartUI();
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(NetworkRunner.LocalPlayer);
		if (currentRoom == null)
		{
			XKumaDebugSystem.LogWarning("�����ɏ������Ă��܂���", KumaDebugColor.WarningColor);
			return;
		}
		string sceneName = currentRoom.SceneNameType.ToString();
		if (SceneManager.GetActiveScene().name == sceneName)
		{
			XKumaDebugSystem.LogWarning("���݂���V�[���ł�", KumaDebugColor.WarningColor);
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
			XKumaDebugSystem.LogWarning("�ǂ̃��[���ɂ������Ă��܂���", KumaDebugColor.ErrorColor);
			_syncResult = SyncResult.Complete;
			return;
		}
		if (currentRoom.LeaderPlayerRef != NetworkRunner.LocalPlayer)
		{
			XKumaDebugSystem.LogWarning("���[�_�[�ł͂���܂���", KumaDebugColor.ErrorColor);
			_syncResult = SyncResult.Complete;
			return;
		}
		
		string sessionName = currentRoom.NextSessionName;
		PlayerRef localPlayerRef = NetworkRunner.LocalPlayer;
		foreach (PlayerRef roomPlayer in currentRoom.JoinRoomPlayer)
		{
			if (roomPlayer == NetworkRunner.LocalPlayer) { continue; }
			MasterServer.SessionRPCManager.Rpc_JoinSession(sessionName, sceneName, roomPlayer);
			XKumaDebugSystem.LogWarning($"{roomPlayer}���ړ�������", KumaDebugColor.MessageColor);
			await UniTask.WaitUntil(() => !NetworkRunner.ActivePlayers.Contains(roomPlayer));
		}

		XKumaDebugSystem.LogWarning($"�S���ړ�������", KumaDebugColor.MessageColor);
		await MasterServer.Disconnect();
		XKumaDebugSystem.LogWarning($"�ؒf����", KumaDebugColor.MessageColor);
		await SceneManager.LoadSceneAsync(sceneName);
		if (PlayerDontDestroyData.Instance != null)
		{
			PlayerDontDestroyData.Instance.PreviousScene = sceneName;
		}
		XKumaDebugSystem.LogWarning($"�V�[����ǂݍ���");
		await MasterServer.JoinOrCreateSession(sessionName, localPlayerRef);
		XKumaDebugSystem.LogWarning($"�������Z�b�V�����ړ�����", KumaDebugColor.MessageColor);
		_syncResult = SyncResult.Complete;
		XKumaDebugSystem.LogWarning($"�ړ��I��", KumaDebugColor.MessageColor);
		await UniTask.WaitUntil(() => NetworkRunner != null);

		foreach (PlayerRef roomPlayer in currentRoom.JoinRoomPlayer)
		{
			await UniTask.WaitUntil(() => NetworkRunner.ActivePlayers.Contains(roomPlayer));
		}
		XKumaDebugSystem.LogWarning("�S����������");


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
		XKumaDebugSystem.LogWarning("�A�N�e�B�r�e�B�X�^�[�g", KumaDebugColor.MessageColor);
		if (_syncResult != SyncResult.Complete)
		{
			Debug.LogWarning("�ړ����ł�");
			return;
		}

		string sceneName = SceneNameType.TestPhotonScene.ToString();
		if (SceneManager.GetActiveScene().name == sceneName)
		{
			XKumaDebugSystem.LogWarning("���݂���V�[���ł�", KumaDebugColor.WarningColor);
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
		XKumaDebugSystem.LogWarning($"�ؒf����", KumaDebugColor.MessageColor);
		await SceneManager.LoadSceneAsync(sceneName);
		XKumaDebugSystem.LogWarning($"�V�[����ǂݍ���");
		await MasterServer.JoinOrCreateSession(sessionName, localPlayerRef);
		XKumaDebugSystem.LogWarning($"�������Z�b�V�����ړ�����", KumaDebugColor.MessageColor);
		_syncResult = SyncResult.Complete;
		XKumaDebugSystem.LogWarning($"�ړ��I��", KumaDebugColor.MessageColor);
		await UniTask.WaitUntil(() => NetworkRunner != null);
	}
}


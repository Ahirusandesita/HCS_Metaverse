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
	public event System.Action OnConnect;

	public GateOfFusion()
	{
		MasterServer.OnConnect += () => OnConnect?.Invoke();
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

	/// <summary>
	/// �͂ނƂ��ɌĂ�
	/// </summary>
	/// <param name="networkObject">�͂񂾃I�u�W�F�N�g</param>
	public void Grab(NetworkObject networkObject)
	{
		if (!MasterServer.IsUsePhoton) { return; }
		XKumaDebugSystem.LogWarning($"Grab:{networkObject.name}", KumaDebugColor.InformationColor);
		StateAuthorityData stateAuthorityData = networkObject.GetComponent<StateAuthorityData>();
		if (stateAuthorityData.IsNotReleaseStateAuthority)
		{
			XKumaDebugSystem.LogWarning($"����������܂���ł���", KumaDebugColor.WarningColor);
			return;
		}
		if (networkObject.HasStateAuthority)
		{
			XKumaDebugSystem.LogWarning($"�����������������Ă��܂�", KumaDebugColor.WarningColor);
			return;
		}
		MasterServer.SessionRPCManager.Rpc_GrabStateAuthorityChanged(networkObject);
		networkObject.RequestStateAuthority();
		stateAuthorityData.IsNotReleaseStateAuthority = true;
	}

	public void Release(NetworkObject networkObject)
	{
		if (!MasterServer.IsUsePhoton) { return; }
		MasterServer.SessionRPCManager.Rpc_ReleseStateAuthorityChanged(networkObject);
		StateAuthorityData stateAuthorityData = networkObject.GetComponent<StateAuthorityData>();
		stateAuthorityData.IsNotReleaseStateAuthority = false;
	}

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

	public async void ActivityStart()
	{
		XKumaDebugSystem.LogWarning("�A�N�e�B�r�e�B�X�^�[�g", KumaDebugColor.MessageColor);
		if (_syncResult != SyncResult.Complete)
		{
			Debug.LogWarning("�ړ����ł�");
			return;
		}
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(NetworkRunner.LocalPlayer);
		string sceneName = currentRoom.SceneNameType.ToString();
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
		foreach (RoomPlayer roomPlayer in currentRoom.JoinRoomPlayer)
		{
			if (roomPlayer.PlayerData == NetworkRunner.LocalPlayer) { continue; }
			MasterServer.SessionRPCManager.Rpc_JoinSession(sessionName, sceneName, roomPlayer.PlayerData);
			XKumaDebugSystem.LogWarning($"{roomPlayer.PlayerData}���ړ�������", KumaDebugColor.MessageColor);
			await UniTask.WaitUntil(() => !NetworkRunner.ActivePlayers.Contains(roomPlayer.PlayerData));
		}
		XKumaDebugSystem.LogWarning($"�S���ړ�������", KumaDebugColor.MessageColor);
		await MasterServer.Disconnect();
		XKumaDebugSystem.LogWarning($"�ؒf����", KumaDebugColor.MessageColor);
		await SceneManager.LoadSceneAsync(sceneName);
		XKumaDebugSystem.LogWarning($"�V�[����ǂݍ���");
		await MasterServer.JoinOrCreateSession(sessionName,localPlayerRef);
		XKumaDebugSystem.LogWarning($"�������Z�b�V�����ړ�����", KumaDebugColor.MessageColor);
		_syncResult = SyncResult.Complete;
		XKumaDebugSystem.LogWarning($"�ړ��I��", KumaDebugColor.MessageColor);
	}
}


using UnityEngine.SceneManagement;
using UnityEngine;
using Fusion;
using System.Linq;
using Cysharp.Threading.Tasks;

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
		MasterServer.OnConnect += OnConnect;
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
		XDebug.LogWarning($"Grab:{networkObject.name}", KumaDebugColor.InformationColor);
		StateAuthorityData stateAuthorityData = networkObject.GetComponent<StateAuthorityData>();
		if (stateAuthorityData.IsNotReleaseStateAuthority)
		{
			XDebug.LogWarning($"����������܂���ł���", KumaDebugColor.WarningColor);
			return;
		}
		if (networkObject.HasStateAuthority)
		{
			XDebug.LogWarning($"�����������������Ă��܂�", KumaDebugColor.WarningColor);
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
		XDebug.LogWarning($"Despawn:{despawnObject.gameObject}", KumaDebugColor.ErrorColor);
		if (despawnObject.TryGetComponent(out NetworkObject networkObject))
		{
			NetworkRunner.Despawn(networkObject);
			return;
		}
		XDebug.LogError("NetworkObject���擾�ł��܂���ł����B�Ȃ̂�Destroy���܂��B", KumaDebugColor.ErrorColor);
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

			temp = (await NetworkRunner.SpawnAsync(networkObject, position, quaternion)).GetComponent<T>();
		}
		else
		{
			XDebug.LogError("NetworkObject���擾�ł��܂���ł����B�Ȃ̂�Instantiate���܂��B", KumaDebugColor.ErrorColor);
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
			temp = (await NetworkRunner.SpawnAsync(networkObject, position, quaternion)).gameObject;
		}
		else
		{
			XDebug.LogError("NetworkObject���擾�ł��܂���ł����B�Ȃ̂�Instantiate���܂��B", KumaDebugColor.ErrorColor);
			temp = Object.Instantiate(prefab, position, quaternion);
		}
		temp.transform.SetParent(parent);
		return temp;
	}

	/*
	 * �l�b�g���[�N��؂�
	 * �V�[����ς���
	 * �ڑ�����
	 */

	public async void ActivityStart()
	{
		XDebug.LogWarning("�A�N�e�B�r�e�B�X�^�[�g", KumaDebugColor.MessageColor);
		if (_syncResult != SyncResult.Complete)
		{
			Debug.LogError("�ړ����ł�");
			return;
		}
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(NetworkRunner.LocalPlayer);
		string sceneName = currentRoom.SceneNameType.ToString();
		if(SceneManager.GetActiveScene().name == sceneName)
		{
			XDebug.LogWarning("���݂���V�[���ł�", KumaDebugColor.WarningColor);
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
			XDebug.LogWarning("�ǂ̃��[���ɂ������Ă��܂���", KumaDebugColor.ErrorColor);
			_syncResult = SyncResult.Complete;
			return;
		}
		if (currentRoom.LeaderPlayerRef != NetworkRunner.LocalPlayer)
		{
			XDebug.LogWarning("���[�_�[�ł͂���܂���", KumaDebugColor.ErrorColor);
			_syncResult = SyncResult.Complete;
			return;
		}
		string sessionName = currentRoom.NextSessionName;

		foreach (RoomPlayer roomPlayer in currentRoom.JoinRoomPlayer)
		{
			if (roomPlayer.PlayerData == NetworkRunner.LocalPlayer) { continue; }
			MasterServer.SessionRPCManager.Rpc_JoinSession(sessionName, sceneName, roomPlayer.PlayerData);
			XDebug.LogWarning($"{roomPlayer.PlayerData}���ړ�������", KumaDebugColor.MessageColor);
			await UniTask.WaitUntil(() => !NetworkRunner.ActivePlayers.Contains(roomPlayer.PlayerData));
		}

		XDebug.LogWarning($"�S���ړ�������", KumaDebugColor.MessageColor);
		await MasterServer.Disconnect();
		XDebug.LogWarning($"�ؒf����", KumaDebugColor.MessageColor);

		await MasterServer.JoinOrCreateSession(sessionName);
		XDebug.LogWarning($"�������Z�b�V�����ړ�����", KumaDebugColor.MessageColor);
		//await UniTask.WaitUntil(() => MasterServer.SessionRPCManager == null);
		//XDebug.LogWarning($"RpcDelete", KumaDebugColor.MessageColor);

		foreach (RoomPlayer roomPlayer in currentRoom.JoinRoomPlayer)
		{
			await UniTask.WaitUntil(() => NetworkRunner.ActivePlayers.Contains(roomPlayer.PlayerData));
		}

		SessionRPCManager sessionRPCManager = await MasterServer.GetSessionRPCManagerAsync();

		if (!NetworkRunner.IsSharedModeMasterClient)
		{
			//XDebug.LogWarning($"Rpc�擾", KumaDebugColor.MessageColor);
			//await UniTask.WaitUntil(() => Object.FindObjectOfType<SessionRPCManager>() != null);
			//SessionRPCManager sessionRPCManager = Object.FindObjectOfType<SessionRPCManager>();
			XDebug.LogWarning($"{NetworkRunner.SessionInfo.PlayerCount}" +
				$":{currentRoom.JoinRoomPlayer.Count}", KumaDebugColor.MessageColor);
			sessionRPCManager.Rpc_ChangeMasterClient(NetworkRunner.LocalPlayer);
			await UniTask.WaitUntil(() => NetworkRunner.IsSharedModeMasterClient);
			XDebug.LogWarning("�������}�X�^�[�ɂȂ���", KumaDebugColor.MessageColor);
		}
		await NetworkRunner.LoadScene(sceneName, LoadSceneMode.Single);
		_syncResult = SyncResult.Complete;
		XDebug.LogWarning($"�ړ��I��", KumaDebugColor.MessageColor);
	}
}


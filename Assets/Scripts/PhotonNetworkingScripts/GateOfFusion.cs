using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Fusion;
using Cysharp.Threading.Tasks;

public class GateOfFusion
{
	private bool _canUsePhoton = default;
	private NetworkRunner _networkRunner = default;
	private static GateOfFusion _instance = default;
	private MasterServerConect _masterServer = default;
	private SyncResult _syncResult = SyncResult.Complete;
	public static GateOfFusion Instance => _instance ??= new GateOfFusion();
	private MasterServerConect MasterServer
		=> _masterServer ??= Object.FindObjectOfType<MasterServerConect>();

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
	public bool IsCanUsePhoton { get => _canUsePhoton; set => _canUsePhoton = value; }
	public SyncResult SyncResult { get => _syncResult; set => _syncResult = value; }




	/// <summary>
	/// 掴むときに呼ぶ
	/// </summary>
	/// <param name="networkObject">掴んだオブジェクト</param>
	public void Grab(NetworkObject networkObject)
	{
		if (!MasterServer.IsUsePhoton) { return; }
		XDebug.LogWarning($"Grab:{networkObject.name}", KumaDebugColor.InformationColor);
		StateAuthorityData stateAuthorityData = networkObject.GetComponent<StateAuthorityData>();
		if (stateAuthorityData.IsNotReleaseStateAuthority)
		{
			XDebug.LogWarning($"権限がありませんでした", KumaDebugColor.WarningColor);
			return;
		}
		if (networkObject.HasStateAuthority)
		{
			XDebug.LogWarning($"自分が権限を持っています", KumaDebugColor.WarningColor);
			return;
		}
		MasterServer.RPCManager.Rpc_GrabStateAuthorityChanged(networkObject);
		networkObject.RequestStateAuthority();
		stateAuthorityData.IsNotReleaseStateAuthority = true;
	}

	public void Release(NetworkObject networkObject)
	{
		if (!MasterServer.IsUsePhoton) { return; }
		MasterServer.RPCManager.Rpc_ReleseStateAuthorityChanged(networkObject);
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
		XDebug.LogWarning($"Despawn:{despawnObject.gameObject}",KumaDebugColor.ErrorColor);
		if (despawnObject.TryGetComponent(out NetworkObject networkObject))
		{
			NetworkRunner.Despawn(networkObject);
			return;
		}
		XDebug.LogError("NetworkObjectが取得できませんでした。なのでDestroyします。", KumaDebugColor.ErrorColor);
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
			XDebug.LogError("NetworkObjectが取得できませんでした。なのでInstantiateします。", KumaDebugColor.ErrorColor);
			temp = Object.Instantiate(prefab, position, quaternion);
		}
		temp.transform.parent = parent;
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
			XDebug.LogError("NetworkObjectが取得できませんでした。なのでInstantiateします。", KumaDebugColor.ErrorColor);
			temp = Object.Instantiate(prefab, position, quaternion);
		}
		temp.transform.parent = parent;
		return temp;
	}

	

	public async void ActivityStart(string sceneName)
	{
		if (_syncResult != SyncResult.Complete)
		{
			Debug.LogError("移動中です");
			return;
		}
		_syncResult = SyncResult.Connecting;
		if (!MasterServer.IsUsePhoton)
		{
			SceneManager.LoadScene(sceneName);
			_syncResult = SyncResult.Complete;
			return;
		}
		//アクティビティスタート
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(NetworkRunner.LocalPlayer);
		if (currentRoom == null)
		{
			XDebug.LogWarning("どのルームにも入っていません", KumaDebugColor.ErrorColor);
			_syncResult = SyncResult.Complete;
			return;
		}
		if (currentRoom.LeaderPlayerRef != NetworkRunner.LocalPlayer)
		{
			XDebug.LogWarning("リーダーではありません", KumaDebugColor.ErrorColor);
			_syncResult = SyncResult.Complete;
			return;
		}
		string sessionName = currentRoom.NextSessionName;
		foreach (RoomPlayer roomPlayer in currentRoom.JoinRoomPlayer)
		{
			if (roomPlayer.PlayerData == NetworkRunner.LocalPlayer) { continue; }
			MasterServer.RPCManager.Rpc_JoinSession(sessionName, roomPlayer.PlayerData);
			XDebug.LogWarning($"{roomPlayer.PlayerData}を移動させた", KumaDebugColor.MessageColor);
		}
		XDebug.LogWarning($"全員移動させた", KumaDebugColor.MessageColor);
		await MasterServer.JoinOrCreateSession(sessionName);
		XDebug.LogWarning($"自分がセッション移動した", KumaDebugColor.MessageColor);
		await MasterServer.GetRunner();
		await UniTask.WaitUntil(() => Object.FindObjectOfType<RPCManager>() != null);

		RPCManager rpcManager = Object.FindObjectOfType<RPCManager>();

		if (!NetworkRunner.IsSharedModeMasterClient)
		{
			
			XDebug.LogError($"{NetworkRunner.SessionInfo.PlayerCount}" +
				$":{currentRoom.JoinRoomPlayer.Count}", KumaDebugColor.MessageColor);
			rpcManager.Rpc_ChangeMasterClient(NetworkRunner.LocalPlayer);
			XDebug.LogError($"{NetworkRunner}:{NetworkRunner.IsSharedModeMasterClient}", KumaDebugColor.ErrorColor);
			await UniTask.WaitUntil(() => NetworkRunner.IsSharedModeMasterClient);
			XDebug.LogWarning("自分がマスターになった", KumaDebugColor.MessageColor);
		}
		await NetworkRunner.LoadScene(sceneName, LoadSceneMode.Single);
		_syncResult = SyncResult.Complete;
		XDebug.LogWarning($"移動終了",KumaDebugColor.MessageColor);
	}
}


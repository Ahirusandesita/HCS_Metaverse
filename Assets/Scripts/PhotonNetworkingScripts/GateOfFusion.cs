using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cysharp.Threading.Tasks;

public class GateOfFusion
{
	private bool _canUsePhoton = default;
	private NetworkRunner _networkRunner = default;
	private static GateOfFusion _instance = default;
	private MasterServerConect _masterServer = default;
	private SyncResult _syncResult = default;
	public static GateOfFusion Instance => _instance ??= new GateOfFusion();
	private MasterServerConect MasterServer
		=> _masterServer ??= Object.FindObjectOfType<MasterServerConect>();
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
	public SyncResult SyncResult => _syncResult;

	/// <summary>
	/// 掴むときに呼ぶ
	/// </summary>
	/// <param name="networkObject">掴んだオブジェクト</param>
	public void Grab(NetworkObject networkObject)
	{
		XDebug.LogWarning($"Grab:{networkObject.name}", KumaDebugColor.InformationColor);
		StateAuthorityData stateAuthorityData = networkObject.GetComponent<StateAuthorityData>();
		if (stateAuthorityData.IsNotReleaseStateAuthority)
		{
			XDebug.LogWarning($"権限がありませんでした", KumaDebugColor.ErrorColor);
			return;
		}
		if (networkObject.HasStateAuthority)
		{
			XDebug.LogWarning($"自分が権限を持っています", KumaDebugColor.WarningColor);
			return;
		}
		RPCManager.Instance.Rpc_GrabStateAuthorityChanged(networkObject);
		networkObject.RequestStateAuthority();
	}

	public void Despawn<T>(T despawnObject) where T : Component
	{
		if (despawnObject.TryGetComponent(out NetworkObject networkObject))
		{
			NetworkRunner.Despawn(networkObject);
			return;
		}
		XDebug.LogError("NetworkObjectが取得できませんでした。なのでDestroyします。", KumaDebugColor.ErrorColor);
		Object.Destroy(despawnObject.gameObject);
	}

	public void Spawn(GameObject prefab, Vector3 position = default, Quaternion quaternion = default, Transform parent = default)
	{
		GameObject temp;
		if (prefab.TryGetComponent(out NetworkObject networkObject))
		{
			temp = NetworkRunner.Spawn(networkObject, position, quaternion).gameObject;
		}
		else
		{
			XDebug.LogError("NetworkObjectが取得できませんでした。なのでInstantiateします。", KumaDebugColor.ErrorColor);
			temp = Object.Instantiate(prefab,position,quaternion);
		}
		temp.transform.parent = parent;
	}

	public void Release(NetworkObject networkObject)
	{
		RPCManager.Instance.Rpc_ReleseStateAuthorityChanged(networkObject);
	}

	public async void ActivityStart(string sceneName)
	{
		if (_syncResult != SyncResult.Complete)
		{
			Debug.LogError("移動中です");
			return;
		}
		_syncResult = SyncResult.Connecting;
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
			RPCManager.Instance.Rpc_JoinSession(sessionName, roomPlayer.PlayerData);
			XDebug.LogWarning($"{roomPlayer.PlayerData}を移動させた", KumaDebugColor.MessageColor);
		}
		await UniTask.WaitUntil(() => currentRoom.WithLeaderSessionCount <= 0);
		XDebug.LogWarning($"全員移動させた", KumaDebugColor.MessageColor);
		await MasterServer.JoinOrCreateSession(sessionName);
		XDebug.LogWarning($"自分がセッション移動した", KumaDebugColor.MessageColor);
		if (!NetworkRunner.IsSharedModeMasterClient)
		{
			RPCManager.Instance.Rpc_ChangeMasterClient(NetworkRunner.LocalPlayer);
			await UniTask.WaitUntil(() => NetworkRunner.IsSharedModeMasterClient);
			XDebug.LogWarning("自分マスターになった", KumaDebugColor.MessageColor);
		}
		await NetworkRunner.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
		_syncResult = SyncResult.Complete;
	}
}


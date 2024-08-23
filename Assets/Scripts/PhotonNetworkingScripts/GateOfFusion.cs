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
	/// �͂ނƂ��ɌĂ�
	/// </summary>
	/// <param name="networkObject">�͂񂾃I�u�W�F�N�g</param>
	public void Grab(NetworkObject networkObject)
	{
		XDebug.LogWarning($"Grab:{networkObject.name}", KumaDebugColor.InformationColor);
		StateAuthorityData stateAuthorityData = networkObject.GetComponent<StateAuthorityData>();
		if (stateAuthorityData.IsNotReleaseStateAuthority)
		{
			XDebug.LogWarning($"����������܂���ł���", KumaDebugColor.ErrorColor);
			return;
		}
		if (networkObject.HasStateAuthority)
		{
			XDebug.LogWarning($"�����������������Ă��܂�", KumaDebugColor.WarningColor);
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
		XDebug.LogError("NetworkObject���擾�ł��܂���ł����B�Ȃ̂�Destroy���܂��B", KumaDebugColor.ErrorColor);
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
			XDebug.LogError("NetworkObject���擾�ł��܂���ł����B�Ȃ̂�Instantiate���܂��B", KumaDebugColor.ErrorColor);
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
			Debug.LogError("�ړ����ł�");
			return;
		}
		_syncResult = SyncResult.Connecting;
		//�A�N�e�B�r�e�B�X�^�[�g
		Room currentRoom = RoomManager.Instance.GetCurrentRoom(NetworkRunner.LocalPlayer);
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
			RPCManager.Instance.Rpc_JoinSession(sessionName, roomPlayer.PlayerData);
			XDebug.LogWarning($"{roomPlayer.PlayerData}���ړ�������", KumaDebugColor.MessageColor);
		}
		await UniTask.WaitUntil(() => currentRoom.WithLeaderSessionCount <= 0);
		XDebug.LogWarning($"�S���ړ�������", KumaDebugColor.MessageColor);
		await MasterServer.JoinOrCreateSession(sessionName);
		XDebug.LogWarning($"�������Z�b�V�����ړ�����", KumaDebugColor.MessageColor);
		if (!NetworkRunner.IsSharedModeMasterClient)
		{
			RPCManager.Instance.Rpc_ChangeMasterClient(NetworkRunner.LocalPlayer);
			await UniTask.WaitUntil(() => NetworkRunner.IsSharedModeMasterClient);
			XDebug.LogWarning("�����}�X�^�[�ɂȂ���", KumaDebugColor.MessageColor);
		}
		await NetworkRunner.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
		_syncResult = SyncResult.Complete;
	}
}


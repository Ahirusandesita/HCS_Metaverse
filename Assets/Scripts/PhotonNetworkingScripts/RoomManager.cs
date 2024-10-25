using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;
using Cysharp.Threading.Tasks;
using KumaDebug;
using System;

public class RoomManager : MonoBehaviour
{
	[SerializeField]
	private GameObject _leaderObjectPrefab;
	[SerializeField]
	private GameObject _activityStartUIPrefab;
	private ActivityMemberTextController _activityMemberTextController;
	private Dictionary<int, Room> _rooms = new();
	private GameObject _leaderObject;
	private GameObject _activityStartUI;
	private static RoomManager _instance = default;
	private MasterServerConect _masterServer = default;
	public static RoomManager Instance { get => _instance; }
	private MasterServerConect MasterServerConect
	{
		get
		{
			if (_masterServer == null)
			{
				_masterServer = FindObjectOfType<MasterServerConect>();
			}
			return _masterServer;
		}
	}

	private void OnDisable()
	{
		XKumaDebugSystem.LogWarning($"�폜�FRoomManager", KumaDebugColor.ErrorColor);
	}
	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			transform.parent = null;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
			return;
		}
		_instance = this;
	}

	public Room GetCurrentRoom(PlayerRef playerRef)
	{
		if (_rooms.Count < 1) { return null; }
		Room temp = _rooms.Values.FirstOrDefault(room => room.JoinRoomPlayer.Contains(playerRef));
		if (temp == null)
		{
			XKumaDebugSystem.LogWarning("���[���ɎQ�����Ă��܂���", KumaDebugColor.WarningColor);
		}
		return temp;
	}

	public int GetCurrentRoomKey(Room room)
	{
		return _rooms.FirstOrDefault(roomData => roomData.Value == room).Key;
	}

	public int GetCuurentRoomKey(PlayerRef playerRef)
	{
		return _rooms
			.FirstOrDefault(roomData => roomData.Value.JoinRoomPlayer
				.Where(playerData => playerData == playerRef)
			.FirstOrDefault() != null).Key;
	}

	public async UniTask<JoinOrCreateResult> JoinOrCreate(SceneNameType sceneNameType, PlayerRef joinPlayer, int roomNumber = -1)
	{

		Room myRoom = GetCurrentRoom(joinPlayer);
		if (myRoom != null)
		{
			XKumaDebugSystem.LogWarning($"{joinPlayer}�͂��łɃ��[���ɎQ�����Ă��܂�", KumaDebugColor.WarningColor);
			await Instance.LeftOrClose(joinPlayer);
		}

		JoinOrCreateResult result = default;
		Room roomTemp = default;
		//�����ԍ��w��Ȃ��̏ꍇ
		if (roomNumber < 0)
		{
			//����镔�������邩
			roomTemp = _rooms.Values.FirstOrDefault(room => !room.IsEndJoining
			&& room.SceneNameType == sceneNameType);
		}
		else if (_rooms.ContainsKey(roomNumber))
		{
			roomTemp = _rooms[roomNumber];
		}

		//����镔�����Ȃ����ߍ쐬����
		if (roomTemp == null)
		{
			//�����ŃL�[�����ꍇ
			if (roomNumber < 0) { roomNumber = 1; }
			for (; _rooms.ContainsKey(roomNumber); roomNumber++) ;
			if (sceneNameType == SceneNameType.TestPhotonScene)
			{
				roomTemp = Create(sceneNameType, 0);
			}
			else
			{
				roomTemp = Create(sceneNameType, roomNumber);
			}
			if (!roomTemp.IsNonLeader && joinPlayer == GateOfFusion.Instance.NetworkRunner.LocalPlayer)
			{
				InstantiateLeaderObject();
			}
			result = JoinOrCreateResult.Create;
		}
		else
		{
			result = JoinOrCreateResult.Join;
			if (_activityMemberTextController != null)
			{
				_activityMemberTextController.UpdateText();
			}
		}

		roomTemp.Join(joinPlayer);

		if (!roomTemp.IsNonLeader && joinPlayer == GateOfFusion.Instance.NetworkRunner.LocalPlayer)
		{
			InstantiateActivityStartUI();
		}
		XKumaDebugSystem.LogWarning($"�Q��:{sceneNameType}," +
			$"Result:{result}\n," +
			$"Player:{joinPlayer}",
			KumaDebugColor.InformationColor);

		return result;
	}

	public void InstantiateLeaderObject()
	{
		XKumaDebugSystem.LogWarning($"InstanceLeaderObject:{MasterServerConect.Runner.LocalPlayer}", KumaDebugColor.SuccessColor);

		_leaderObject = Instantiate(_leaderObjectPrefab);
	}
	public void InstantiateActivityStartUI(bool isLeader = false)
	{
		_activityStartUI = Instantiate(_activityStartUIPrefab);
		Room room = GetCurrentRoom(GateOfFusion.Instance.NetworkRunner.LocalPlayer);
		XKumaDebugSystem.LogWarning($"{room}:{_activityStartUIPrefab}", KumaDebugColor.RpcColor);
		if (room.LeaderPlayerRef != GateOfFusion.Instance.NetworkRunner.LocalPlayer || isLeader)
		{
			Destroy(FindObjectOfType<ActivityStartButton>().gameObject);
		}
		XKumaDebugSystem.LogWarning($"���[�_�[UI����", KumaDebugColor.InformationColor);
	}

	public void DestroyActivityStartUI()
	{
		XKumaDebugSystem.LogWarning($"���[�_�[UI�폜", KumaDebugColor.InformationColor);
		if (_activityStartUI != null)
		{
			Destroy(_activityStartUI);
		}
		else
		{
			XKumaDebugSystem.LogWarning($"���[�_�[UI��Null�Ȃ̂Ŕj���ł��܂���ł����B", KumaDebugColor.WarningColor);
		}
	}

	public void DestroyLeaderObject()
	{
		XKumaDebugSystem.LogWarning($"DestoryLeaderObject:{MasterServerConect.Runner.LocalPlayer}", KumaDebugColor.SuccessColor);
		if (_leaderObject != null)
		{
			Destroy(_leaderObject);
		}
		else
		{
			XKumaDebugSystem.LogWarning($"���[�_�[�I�u�W�F�N�g��Null�Ȃ̂Ŕj���ł��܂���ł����B", KumaDebugColor.WarningColor);
		}

	}

	/// <summary>
	/// ���[������ޏo����܂��̓��[�������
	/// </summary>
	/// <param name="playerRef">�ޏo����v���C���[���</param>
	public async UniTask LeftOrClose(PlayerRef playerRef)
	{
		Room joinedRoom = GetCurrentRoom(playerRef);
		if (joinedRoom is null) { return; }
		XKumaDebugSystem.LogWarning(
			$"�ޏo:{joinedRoom.SceneNameType}" +
			$"\nRoom:{joinedRoom.NextSessionName}" +
			$"Player:{playerRef}", KumaDebugColor.InformationColor);
		LeftResult result = await joinedRoom.Left(playerRef);
		if (result == LeftResult.Closable)
		{
			int deleteRoomKey = _rooms.FirstOrDefault(room => room.Value == joinedRoom).Key;
			XKumaDebugSystem.LogWarning($"{_rooms[deleteRoomKey].NextSessionName}���폜");
			_rooms.Remove(deleteRoomKey);
		}
		else if (result == LeftResult.Fail)
		{
			XKumaDebugSystem.LogError("���[���ɎQ�����Ă��܂���", KumaDebugColor.ErrorColor);
		}
	}

	private Room Create(SceneNameType activityType, int roomNumber)
	{
		string nextSessionName = activityType + ":" + roomNumber;
		_rooms.Add(roomNumber, new Room(activityType, nextSessionName));
		return _rooms.Values.LastOrDefault();
	}

	public void ChangeSessionName(PlayerRef playerRef, string currentSessionName)
	{
		Room room = GetCurrentRoom(playerRef);
		if (room == null)
		{
			XKumaDebugSystem.LogWarning("���[����������܂���ł���", KumaDebugColor.ErrorColor);
			return;
		}
		room.ChangeSessionName(playerRef, currentSessionName);
	}

	public async void LeaderChange(PlayerRef nextLeaderPlayer)
	{
		XKumaDebugSystem.LogWarning($"�V���[�_�[{nextLeaderPlayer}", KumaDebugColor.SuccessColor);
		Room roomTemp = GetCurrentRoom(nextLeaderPlayer);

		await UniTask.WaitUntil(() => MasterServerConect.Runner.ActivePlayers.Contains(nextLeaderPlayer));


		if (MasterServerConect.Runner.ActivePlayers.Count() > 1)
		{
			await UniTask.WaitUntil(() => MasterServerConect.Runner.ActivePlayers.Contains(roomTemp.LeaderPlayerRef));
			MasterServerConect.SessionRPCManager.Rpc_DestroyLeaderObject(roomTemp.LeaderPlayerRef);
		}
		//�O�̃��[�_�[�̃��[�_�[�I�u�W�F�N�g��j������
		bool isLeader = nextLeaderPlayer == GateOfFusion.Instance.NetworkRunner.LocalPlayer;
		if (isLeader)
		{
			DestroyActivityStartUI();
			InstantiateLeaderObject();
			InstantiateActivityStartUI(true);
		}
		if (roomTemp.IsNonLeader) { return; }
		roomTemp.ChangeLeader(nextLeaderPlayer);

	}

	/// <summary>
	/// �����ȊO�̃f�[�^��j������
	/// </summary>
	public void Initialize(PlayerRef myPlayerRef)
	{
		XKumaDebugSystem.LogWarning($"���[���}�l�[�W���[������", KumaDebugColor.SuccessColor);
		Room myRoom = GetCurrentRoom(myPlayerRef);
		int myRoomKey = GetCurrentRoomKey(myRoom);
		XKumaDebugSystem.LogWarning($"{myRoom}:{myPlayerRef}:", KumaDebugColor.MessageColor);
		bool isLeader = myRoom.LeaderPlayerRef == myPlayerRef;
		_rooms.Clear();
		_rooms.Add(myRoomKey, myRoom);
		if (myRoom.IsNonLeader) { return; }
		if (isLeader) { myRoom.ChangeLeader(myPlayerRef); }
	}

	[ContextMenu("DebugRoomData")]
	public void Test()
	{
		if (_rooms.Count <= 0)
		{
			XKumaDebugSystem.LogWarning("���[��������܂���", KumaDebugColor.MessageColor);
			return;
		}
		foreach (KeyValuePair<int, Room> roomData in _rooms)
		{
			XKumaDebugSystem.LogWarning(
				$"RoomData::,NextSessionName:{roomData.Value.NextSessionName}" +
				$"Leader:{roomData.Value.LeaderPlayerRef}," +
				$"PlayerCount{roomData.Value.JoinRoomPlayer.Count}" +
				$"RoomNumber:{roomData.Key}", KumaDebugColor.InformationColor);
			foreach (PlayerRef player in roomData.Value.JoinRoomPlayer)
			{
				XKumaDebugSystem.LogWarning($"{player}", KumaDebugColor.InformationColor);
			}
		}
	}
}
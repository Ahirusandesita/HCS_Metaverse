using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;
using Cysharp.Threading.Tasks;
using KumaDebug;

public class RoomManager : MonoBehaviour
{
	[SerializeField]
	private GameObject _leaderObjectPrefab;
	private Dictionary<int, Room> _rooms = new();
	private GameObject _leaderObject;
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
		Room temp = _rooms.Values.FirstOrDefault(room => room.JoinRoomPlayer.Contains(new RoomPlayer(playerRef)));
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
				.Where(playerData => playerData.PlayerData == playerRef)
			.FirstOrDefault() != null).Key;
	}

	public async UniTask<JoinOrCreateResult> JoinOrCreate(SceneNameType sceneNameType, PlayerRef joinPlayer, string currentSessionName, int roomNumber = -1)
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
			roomTemp = _rooms.Values.FirstOrDefault(room => !room.IsEndJoining && room.SceneNameType == sceneNameType);
		}
		else
		{
			foreach (int a in _rooms.Keys)
			{
				XKumaDebugSystem.LogWarning(a);
			}
			int key = _rooms.Keys.FirstOrDefault(room => room == roomNumber);
			XKumaDebugSystem.LogWarning(key, KumaDebugColor.MessageColor);
			roomTemp = _rooms[key];
		}

		if (roomTemp == null)
		{
			if (roomNumber < 0) { roomNumber = 1; }
			for (; _rooms.ContainsKey(roomNumber); roomNumber++) ;
			roomTemp = Create(sceneNameType, roomNumber);
			if (sceneNameType != SceneNameType.TestPhotonScene)
			{
				InstantiateLeaderObject();
			}
			result = JoinOrCreateResult.Create;
		}
		else
		{
			result = JoinOrCreateResult.Join;
		}

		roomTemp.Join(joinPlayer, currentSessionName);

		XKumaDebugSystem.LogWarning($"�Q��:{sceneNameType}," +
			$"Result:{result}\n," +
			$"Player:{joinPlayer}",
			KumaDebugColor.InformationColor);

		return result;
	}

	public void InstantiateLeaderObject()
	{
		if (_leaderObject) { return; }
		XKumaDebugSystem.LogWarning($"InstanceLeaderObject:{MasterServerConect.Runner.LocalPlayer}", KumaDebugColor.SuccessColor);
		_leaderObject = Instantiate(_leaderObjectPrefab);
	}

	public void DestroyLeaderObject()
	{
		if (!_leaderObject) { return; }
		XKumaDebugSystem.LogWarning($"DestoryLeaderObject:{MasterServerConect.Runner.LocalPlayer}", KumaDebugColor.SuccessColor);
		Destroy(_leaderObject);
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
			$"\nRoom:{joinedRoom}" +
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

	public void LeaderChange(PlayerRef nextLeaderPlayer)
	{
		XKumaDebugSystem.LogWarning($"�V���[�_�[{nextLeaderPlayer}", KumaDebugColor.SuccessColor);
		Room roomTemp = GetCurrentRoom(nextLeaderPlayer);
		XKumaDebugSystem.LogWarning($"leaderIndex:{roomTemp.LeaderIndex}", KumaDebugColor.SuccessColor);
		foreach (PlayerRef playerRef in MasterServerConect.Runner.ActivePlayers)
		{
			XKumaDebugSystem.LogWarning($"{playerRef}", KumaDebugColor.InformationColor);
		}

		//�O�̃��[�_�[�̃��[�_�[�I�u�W�F�N�g��j������
		MasterServerConect.SessionRPCManager.Rpc_DestroyLeaderObject(roomTemp.LeaderPlayerRef);
		bool isLeader = nextLeaderPlayer == GateOfFusion.Instance.NetworkRunner.LocalPlayer;
		if (isLeader)
		{
			InstantiateLeaderObject();
		}
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
		bool isLeader = myRoom.LeaderPlayerRef == myPlayerRef;
		_rooms.Clear();
		_rooms.Add(myRoomKey, myRoom);
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
		foreach (Room room in _rooms.Values)
		{
			XKumaDebugSystem.LogWarning(
				$"RoomData::,NextSessionName:{room.NextSessionName}" +
				$"Leader:{room.LeaderPlayerRef}," +
				$"PlayerCount{room.JoinRoomPlayer.Count}", KumaDebugColor.InformationColor);
			foreach (RoomPlayer roomPlayer in room.JoinRoomPlayer)
			{
				XKumaDebugSystem.LogWarning($"RoomPlayer:{roomPlayer.PlayerData}" +
					$"PlayerSessionData:{roomPlayer.SessionName}", KumaDebugColor.InformationColor);
			}
		}
	}
}
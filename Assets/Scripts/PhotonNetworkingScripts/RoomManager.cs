using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;

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
		XDebug.LogWarning($"DisableRoomManager", KumaDebugColor.ErrorColor);
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
		if (_rooms.Count <= 0)
		{
			return null;
		}

		var temp =
			_rooms.Where(room => room.Value.JoinRoomPlayer.Contains(new RoomPlayer(playerRef)));
		if (temp.Count() <= 0)
		{
			return null;
		}
		return temp.First().Value;
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

	/// <summary>
	/// �A�N�e�B�r�e�B�̃��[���ɎQ������܂��̓��[�����쐬����
	/// </summary>
	/// <param name="sceneNameType">���肽��Activity</param>
	/// <param name="playerRef">����l�̏��</param>
	/// <param name="roomNumber">���肽�������̔ԍ��@�}�C�i�X�̏ꍇ�͓���镔���ɓ���</param>
	/// <returns>Join�܂���Create�܂���Fail</returns>
	public JoinOrCreateResult JoinOrCreate(SceneNameType sceneNameType, PlayerRef playerRef, string currentSessionName, int roomNumber = -1)
	{
		Room myRoom = GetCurrentRoom(playerRef);
		if (myRoom != null)
		{
			XDebug.LogWarning($"���łɃ��[���ɎQ�����Ă��܂�", KumaDebugColor.WarningColor);
			LeftOrClose(playerRef);
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
			roomTemp = _rooms[_rooms.Keys.FirstOrDefault(room => room == roomNumber)];
		}

		if (roomTemp == null)
		{
			if (roomNumber < 0) { roomNumber = 0; }
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

		roomTemp.Join(playerRef, currentSessionName);

		XDebug.LogWarning($"Join:{sceneNameType}," +
			$"Result:{result}\n," +
			$"Player:{playerRef}",
			KumaDebugColor.InformationColor);

		return result;
	}

	public void InstantiateLeaderObject()
	{
		if (_leaderObject) { return; }
		XDebug.LogWarning($"InstanceLeaderObject:{MasterServerConect.Runner.LocalPlayer}", KumaDebugColor.SuccessColor);
		_leaderObject = Instantiate(_leaderObjectPrefab);
	}

	public void DestroyLeaderObject()
	{
		if (!_leaderObject) { return; }
		XDebug.LogWarning($"DestoryLeaderObject:{MasterServerConect.Runner.LocalPlayer}", KumaDebugColor.SuccessColor);
		Destroy(_leaderObject);
	}

	/// <summary>
	/// ���[������ޏo����܂��̓��[�������
	/// </summary>
	/// <param name="playerRef">�ޏo����v���C���[���</param>
	public void LeftOrClose(PlayerRef playerRef)
	{
		Room joinedRoom = GetCurrentRoom(playerRef);
		if (joinedRoom is null) { return; }
		XDebug.LogWarning(
			$"Left:{joinedRoom.SceneNameType}" +
			$"\nRoomNum:{joinedRoom}" +
			$"Player:{playerRef}", KumaDebugColor.InformationColor);
		LeftResult result = joinedRoom.Left(playerRef);
		if (result == LeftResult.Closable)
		{
			_rooms.Remove(_rooms.FirstOrDefault(room => room.Value == joinedRoom).Key);
		}
		else if (result == LeftResult.Fail)
		{
			XDebug.LogError("���[���ɎQ�����Ă��܂���", KumaDebugColor.ErrorColor);
		}
	}

	private Room Create(SceneNameType activityType, int roomNumber)
	{
		string nextSessionName = activityType + ":" + roomNumber;
		_rooms.Add(roomNumber, new Room(activityType, roomNumber, nextSessionName));
		return _rooms.Values.LastOrDefault();
	}

	public void ChangeSessionName(PlayerRef playerRef, string currentSessionName)
	{
		Room room = GetCurrentRoom(playerRef);
		if (room == null)
		{
			XDebug.LogWarning("���[����������܂���ł���", KumaDebugColor.ErrorColor);
			return;
		}
		room.ChangeSessionName(playerRef, currentSessionName);
	}

	public void LeaderChange(PlayerRef leaderPlayer)
	{
		XDebug.LogWarning($"NewLeader{leaderPlayer}", KumaDebugColor.InformationColor);
		Room roomTemp = GetCurrentRoom(leaderPlayer);
		//�O�̃��[�_�[�̃��[�_�[�I�u�W�F�N�g��j������
		MasterServerConect.SessionRPCManager.Rpc_DestroyLeaderObject(roomTemp.LeaderPlayerRef);
		if (leaderPlayer == GateOfFusion.Instance.NetworkRunner.LocalPlayer
			&& roomTemp.SceneNameType != SceneNameType.TestPhotonScene)
		{
			InstantiateLeaderObject();
		}
		roomTemp.ChangeLeader(leaderPlayer);
	}

	/// <summary>
	/// �����ȊO�̃f�[�^��j������
	/// </summary>
	public void Initialize(PlayerRef myPlayerRef)
	{
		Room myRoom = GetCurrentRoom(myPlayerRef);
		if (myRoom == null)
		{
			XDebug.LogWarning("���[���ɎQ�����Ă܂���", KumaDebugColor.WarningColor);
			_rooms.Clear();
			return;
		}

		IEnumerable<KeyValuePair<int, Room>> deleteRooms = _rooms.Where(room => room.Value != myRoom);
		foreach (KeyValuePair<int, Room> room in deleteRooms)
		{
			_rooms.Remove(room.Key);
		}

	}

	[ContextMenu("DebugRoomData")]
	public void Test()
	{
		if (_rooms.Count <= 0)
		{
			XDebug.LogWarning("���[��������܂���", KumaDebugColor.MessageColor);
			return;
		}
		foreach (Room room in _rooms.Values)
		{
			XDebug.LogWarning(
				$"RoomData::,NextSessionName:{room.NextSessionName}" +
				$"Leader:{room.LeaderPlayerRef}," +
				$"PlayerCount{room.JoinRoomPlayer.Count}", KumaDebugColor.InformationColor);
			foreach (RoomPlayer roomPlayer in room.JoinRoomPlayer)
			{
				XDebug.LogWarning($"RoomPlayer:{roomPlayer.PlayerData}" +
					$"PlayerSessionData:{roomPlayer.SessionName}", KumaDebugColor.InformationColor);
			}
		}
	}
}
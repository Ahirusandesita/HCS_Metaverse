using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;

public class RoomManager : MonoBehaviour
{
	

	[SerializeField]
	private GameObject _leaderObjectPrefab;
	private GameObject _leaderObject;
	private static RoomManager _instance = default;
	private List<Room> _rooms = new();
	private int[] _roomCounter = default;
	private MasterServerConect _masterServer = default;
	public static RoomManager Instance { get => _instance; }
	private MasterServerConect MasterServerConect 
	{
		get
		{
			if(_masterServer == null)
			{
				_masterServer = GetComponent<MasterServerConect>();
			}
			return _masterServer;
		}
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
		_roomCounter = new int[System.Enum.GetValues(typeof(WorldType)).Length];
	}

	public Room GetCurrentRoom(PlayerRef playerRef)
	{
		if (_rooms.Count <= 0)
		{
			return null;
		}

		IEnumerable<Room> temp =
			_rooms.Where(room => room.JoinRoomPlayer.Contains(new RoomPlayer(playerRef)));
		if (temp.Count() <= 0)
		{
			return null;
		}
		return temp.First();
	}

	/// <summary>
	/// �A�N�e�B�r�e�B�̃��[���ɎQ������܂��̓��[�����쐬����
	/// </summary>
	/// <param name="worldType">���肽��Activity</param>
	/// <param name="playerRef">����l�̏��</param>
	/// <param name="roomNumber">���肽�������̔ԍ��@�}�C�i�X�̏ꍇ�͓���镔���ɓ���</param>
	/// <returns>Join�܂���Create�܂���Fail</returns>
	public JoinOrCreateResult JoinOrCreate(WorldType worldType, PlayerRef playerRef, string currentSessionName, int roomNumber = -1)
	{
		Room roomTemp = default;

		if (_rooms.Count > 0)
		{
			//�����ԍ��w��Ȃ��̏ꍇ
			if (roomNumber < 0)
			{
				roomTemp = _rooms.Where(room => !room.IsEndJoining).First();
			}
			else
			{
				roomTemp = _rooms.Where(room => room.RoomNumber == roomNumber).First();
				//�w�肵�������ԍ��ɓ���Ȃ����ߎ��s
				if (roomTemp.IsEndJoining) { return JoinOrCreateResult.Fail; }
			}
		}
		JoinOrCreateResult result = default;

		if (roomTemp == null)
		{
			if (roomNumber < 0) { roomNumber = _rooms.Count; }
			roomTemp = Create(worldType, roomNumber, currentSessionName);
			InstantiateLeaderObject();
			result = JoinOrCreateResult.Create;
		}
		else
		{
			result = JoinOrCreateResult.Join;
		}

		roomTemp.Join(playerRef, currentSessionName);

		XDebug.LogWarning($"Join:{worldType}," +
			$"Result:{result}\nRoomNum:{roomTemp.RoomNumber}," +
			$"Player:{playerRef}",
			KumaDebugColor.InformationColor);

		return result;
	}

	public void InstantiateLeaderObject()
	{
		if (_leaderObject) { return; }
		_leaderObject = Instantiate(_leaderObjectPrefab);
	}

	public void DestroyLeaderObject()
	{
		if (!_leaderObject) { return; }
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
			$"Left:{joinedRoom.WorldType}" +
			$"\nRoomNum:{joinedRoom.RoomNumber}" +
			$"Player:{playerRef}", KumaDebugColor.InformationColor);
		LeftResult result = joinedRoom.Left(playerRef);
		if (result == LeftResult.Closable)
		{
			_roomCounter[(int)joinedRoom.WorldType]--;
			_rooms.Remove(joinedRoom);
		}
		else if (result == LeftResult.Fail)
		{
			XDebug.LogError("���[���ɎQ�����Ă��܂���", KumaDebugColor.ErrorColor);
		}
	}

	private Room Create(WorldType activityType, int roomNumber, string leaderSessionName)
	{
		string nextSessionName = activityType + ":" + _roomCounter[(int)activityType];
		_rooms.Add(new Room(activityType, roomNumber, nextSessionName));
		_roomCounter[(int)activityType]++;
		return _rooms.LastOrDefault();
	}

	public void ChengeSessionName(PlayerRef playerRef, string currentSessionName)
	{
		Room room = GetCurrentRoom(playerRef);
		if (room == null)
		{
			XDebug.LogError("���[����������܂���ł���", KumaDebugColor.ErrorColor);
			return;
		}
		room.ChangeSessionName(playerRef, currentSessionName);
	}

	public void LeaderChange(PlayerRef leaderPlayer)
	{
		XDebug.LogWarning($"NewLeader{leaderPlayer}", KumaDebugColor.InformationColor);
		Room roomTemp = GetCurrentRoom(leaderPlayer);
		XDebug.LogWarning(MasterServerConect,KumaDebugColor.SuccessColor);
		XDebug.LogWarning(MasterServerConect.SessionRPCManager,KumaDebugColor.SuccessColor);
		//�O�̃��[�_�[�̃��[�_�[�I�u�W�F�N�g��j������
		MasterServerConect.SessionRPCManager.Rpc_DestroyLeaderObject(roomTemp.LeaderPlayerRef);
		if (leaderPlayer == GateOfFusion.Instance.NetworkRunner.LocalPlayer)
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
		_roomCounter = new int[System.Enum.GetValues(typeof(WorldType)).Length];

		Room myRoom = GetCurrentRoom(myPlayerRef);
		if (myRoom == null)
		{
			_rooms.Clear();
			return;
		}

		_roomCounter[(int)myRoom.WorldType]++;

		IEnumerable<Room> deleteRooms = _rooms.Where(room => room != myRoom);
		foreach (Room room in deleteRooms)
		{
			_rooms.Remove(room);
		}

	}

	[ContextMenu("DebugRoomData")]
	public void Test()
	{
		foreach (Room room in _rooms)
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
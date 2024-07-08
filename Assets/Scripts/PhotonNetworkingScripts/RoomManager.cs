using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;
public class Room
{
	private bool _isEndJoining = default;
	private int _leaderIndex = default;
	private int _number = default;
	private readonly string _sessionName = default;
	private WorldType _worldType = default;
	private List<PlayerRef> _joinedPlayer = new();

	public int Number { get => _number; }
	public bool IsEndJoining { get => _isEndJoining; }
	public string SessionName { get => _sessionName; }
	public WorldType WorldType { get => _worldType; }
	public IReadOnlyList<PlayerRef> JoinPlayer { get => _joinedPlayer; }

	public Room(WorldType activityType, int roomNumber,string sessionName)
	{
		this._worldType = activityType;
		this._number = roomNumber;
		this._leaderIndex = 0;
		this._sessionName = sessionName;
	}
	public void Join(PlayerRef playerRef)
	{
		_joinedPlayer.Add(playerRef);
	}

	/// <summary>
	/// ��������ޏo����
	/// </summary>
	/// <param name="playerRef">�ޏo����v���C���[</param>
	/// <returns>���U���g</returns>
	public RoomManager.LeftResult Left(PlayerRef playerRef)
	{
		int index = _joinedPlayer.IndexOf(playerRef);
		//�Q�����Ă��Ȃ������ꍇ
		if (index < 0) { return RoomManager.LeftResult.Fail; }
		_joinedPlayer.RemoveAt(index);

		//�����̃����o�[�����Ȃ��ꍇ
		if (_joinedPlayer.Count <= 0) { return RoomManager.LeftResult.Closable; }
		//�����[�_�[�̏ꍇ
		if (_leaderIndex == index)
		{
			int nextLeaderIndex = Random.Range(0, _joinedPlayer.Count);
			_leaderIndex = nextLeaderIndex;
		}

		return RoomManager.LeftResult.Success;
	}
}
public class RoomManager : NetworkBehaviour
{
	public enum JoinOrCreateResult
	{
		Create,
		Join,
		Fail
	}

	public enum LeftResult
	{
		Closable,
		Success,
		Fail
	}

	private static RoomManager _instance = default;
	private int[] _roomCounter;
	private List<Room> _rooms = new();


	public static RoomManager Instance { get => _instance; }
	private void Awake()
	{
		if (_instance is null)
		{
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		_roomCounter = new int[System.Enum.GetValues(typeof(WorldType)).Length];
	}

	public Room GetMyJoinedRoom(PlayerRef playerRef)
	{
		return _rooms.Where(room => room.JoinPlayer.Contains(playerRef)).First();
	}

	/// <summary>
	/// �A�N�e�B�r�e�B�̃��[���ɎQ������܂��̓��[�����쐬����
	/// </summary>
	/// <param name="activityType">���肽��Activity</param>
	/// <param name="playerRef">����l�̏��</param>
	/// <param name="roomNumber">���肽�������̔ԍ��@�}�C�i�X�̏ꍇ�͓���镔���ɓ���</param>
	/// <returns>Join�܂���Create�܂���Fail</returns>
	public JoinOrCreateResult JoinOrCreate(WorldType activityType, PlayerRef playerRef, int roomNumber)
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
				roomTemp = _rooms.Where(room => room.Number == roomNumber).First();
				//�w�肵�������ԍ��ɓ���Ȃ����ߎ��s
				if (roomTemp.IsEndJoining) { return JoinOrCreateResult.Fail; }
			}
		}
		JoinOrCreateResult result = default;

		if (roomTemp is null)
		{
			roomTemp = Create(activityType, roomNumber);
			result = JoinOrCreateResult.Create;
		}
		else
		{
			result = JoinOrCreateResult.Join;
		}

		roomTemp.Join(playerRef);

		return result;
	}

	/// <summary>
	/// ���[������ޏo����܂��̓��[�������
	/// </summary>
	/// <param name="playerRef">�ޏo����v���C���[���</param>
	public void LeftOrClose(PlayerRef playerRef)
	{
		Room joinedRoom = GetMyJoinedRoom(playerRef);
		LeftResult result = joinedRoom.Left(playerRef);
		if (result == LeftResult.Closable)
		{
			_roomCounter[(int)joinedRoom.WorldType]--;
			_rooms.Remove(joinedRoom);
		}
		else if (result == LeftResult.Fail)
		{
			Debug.LogError("�Q�����Ă��܂���");
		}
	}

	private Room Create(WorldType activityType, int roomNumber)
	{
		string sessionName = activityType +":"+ _roomCounter[(int)activityType];
		_rooms.Add(new Room(activityType, roomNumber,sessionName));
		_roomCounter[(int)activityType]++;
		return _rooms.LastOrDefault();
	}
}

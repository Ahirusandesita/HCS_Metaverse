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

	public int this[PlayerRef playerRef] { get => _joinedPlayer.IndexOf(playerRef); }
	public int LeaderIndex { get => _leaderIndex; }
	public int Number { get => _number; }
	public bool IsEndJoining { get => _isEndJoining; }
	public string SessionName { get => _sessionName; }
	public WorldType WorldType { get => _worldType; }
	public IReadOnlyList<PlayerRef> JoinPlayer { get => _joinedPlayer; }

	public Room(WorldType activityType, int roomNumber, string sessionName)
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

		RoomManager.LeftResult result = RoomManager.LeftResult.Success;

		//�����[�_�[�̏ꍇ
		if (_leaderIndex == index)
		{
			int nextLeaderIndex = Random.Range(0, _joinedPlayer.Count);
			_leaderIndex = nextLeaderIndex;
			result = RoomManager.LeftResult.LeaderChanged;
		}

		return result;
	}

	public void ChengeLeader(PlayerRef nextLeaderPlayer)
	{
		_leaderIndex = _joinedPlayer.IndexOf(nextLeaderPlayer);
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
		LeaderChanged,
		Success,
		Fail
	}

	private static RoomManager _instance = default;
	private List<Room> _rooms = new();
	private int[] _roomCounter = default;
	public static RoomManager Instance { get => _instance; }

	private void Awake()
	{
		if (_instance is null)
		{
			_instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		_roomCounter = new int[System.Enum.GetValues(typeof(WorldType)).Length];
	}


	public Room GetCurrentRoom(PlayerRef playerRef)
	{
		if (_rooms.Count <= 0)
		{
			return null;
		}

		IEnumerable<Room> temp = _rooms.Where(room => room.JoinPlayer.Contains(playerRef));
		if(temp.Count() <= 0)
		{
			return null;
		}
		return temp.First();
	}

	/// <summary>
	/// �A�N�e�B�r�e�B�̃��[���ɎQ������܂��̓��[�����쐬����
	/// </summary>
	/// <param name="activityType">���肽��Activity</param>
	/// <param name="playerRef">����l�̏��</param>
	/// <param name="roomNumber">���肽�������̔ԍ��@�}�C�i�X�̏ꍇ�͓���镔���ɓ���</param>
	/// <returns>Join�܂���Create�܂���Fail</returns>
	public JoinOrCreateResult JoinOrCreate(WorldType activityType, PlayerRef playerRef, int roomNumber = -1)
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
		Debug.LogWarning("join:" + activityType + "\nroomNum:" + roomNumber + "\nPlayer:" + playerRef);

		return result;
	}

	/// <summary>
	/// ���[������ޏo����܂��̓��[�������
	/// </summary>
	/// <param name="playerRef">�ޏo����v���C���[���</param>
	public void LeftOrClose(PlayerRef playerRef)
	{
		Room joinedRoom = GetCurrentRoom(playerRef);
		if (joinedRoom is null) { return; }
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
		string sessionName = activityType + ":" + _roomCounter[(int)activityType];
		_rooms.Add(new Room(activityType, roomNumber, sessionName));
		_roomCounter[(int)activityType]++; ;
		return _rooms.LastOrDefault();
	}
	public void LeaderChenge(PlayerRef leaderPlayer)
	{
		Room roomTemp = GetCurrentRoom(leaderPlayer);

		roomTemp.ChengeLeader(leaderPlayer);
	}

	[ContextMenu("count")]
	private void Test()
	{
		Room roomTemp = GetCurrentRoom(Runner.LocalPlayer);
		if(roomTemp is not null)
		{
			Debug.LogWarning("�ELeader:" + (roomTemp[Runner.LocalPlayer] == roomTemp.LeaderIndex));
		}
		Debug.LogWarning(_rooms.Count);
	}
}

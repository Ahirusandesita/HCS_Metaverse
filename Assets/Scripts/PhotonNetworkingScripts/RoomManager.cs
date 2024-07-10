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
	/// 部屋から退出する
	/// </summary>
	/// <param name="playerRef">退出するプレイヤー</param>
	/// <returns>リザルト</returns>
	public RoomManager.LeftResult Left(PlayerRef playerRef)
	{
		int index = _joinedPlayer.IndexOf(playerRef);
		//参加していなかった場合
		if (index < 0) { return RoomManager.LeftResult.Fail; }
		_joinedPlayer.RemoveAt(index);

		//部屋のメンバーがいない場合
		if (_joinedPlayer.Count <= 0) { return RoomManager.LeftResult.Closable; }

		RoomManager.LeftResult result = RoomManager.LeftResult.Success;

		//↓リーダーの場合
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
	/// アクティビティのルームに参加するまたはルームを作成する
	/// </summary>
	/// <param name="activityType">入りたいActivity</param>
	/// <param name="playerRef">入る人の情報</param>
	/// <param name="roomNumber">入りたい部屋の番号　マイナスの場合は入れる部屋に入る</param>
	/// <returns>JoinまたはCreateまたはFail</returns>
	public JoinOrCreateResult JoinOrCreate(WorldType activityType, PlayerRef playerRef, int roomNumber = -1)
	{
		Room roomTemp = default;

		if (_rooms.Count > 0)
		{
			//部屋番号指定なしの場合
			if (roomNumber < 0)
			{
				roomTemp = _rooms.Where(room => !room.IsEndJoining).First();
			}
			else
			{
				roomTemp = _rooms.Where(room => room.Number == roomNumber).First();
				//指定した部屋番号に入れないため失敗
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
	/// ルームから退出するまたはルームを閉じる
	/// </summary>
	/// <param name="playerRef">退出するプレイヤー情報</param>
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
			Debug.LogError("参加していません");
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
			Debug.LogWarning("・Leader:" + (roomTemp[Runner.LocalPlayer] == roomTemp.LeaderIndex));
		}
		Debug.LogWarning(_rooms.Count);
	}
}

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;

public static partial class ExtensionList
{
	public static int IndexOf(this List<Room.RoomPlayer> list, PlayerRef playerRef)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].PlayerData != playerRef) { continue; }
			return i;
		}
		return -1;
	}
}

public class Room
{
	public class RoomPlayer
	{
		private string _sessionName;
		private PlayerRef _playerData;
		public RoomPlayer(PlayerRef playerData, string sessionName = "")
		{
			_sessionName = sessionName;
			_playerData = playerData;
		}
		public string SessionName { get => _sessionName; set => _sessionName = value; }
		public PlayerRef PlayerData { get => _playerData; set => _playerData = value; }

		public static bool operator ==(RoomPlayer roomPlayer, RoomPlayer roomPlayer1)
			=> roomPlayer._playerData == roomPlayer1._playerData;

		public static bool operator !=(RoomPlayer roomPlayer, RoomPlayer roomPlayer1)
			=> roomPlayer._playerData != roomPlayer1._playerData;

		public static bool operator ==(RoomPlayer roomPlayer, PlayerRef playerData)
			=> roomPlayer._playerData == playerData;
		public static bool operator !=(RoomPlayer roomPlayer, PlayerRef playerData)
			=> roomPlayer._playerData == playerData;
		public override bool Equals(object obj)
		{
			if (obj is PlayerRef playerRef)
			{
				return _playerData == playerRef;
			}
			else if (obj is RoomPlayer roomPlayer)
			{
				return this == roomPlayer;
			}
			return false;
		}
		public override int GetHashCode() => base.GetHashCode();
	}

	private bool _isEndJoining = default;
	private int _leaderIndex = default;
	private int _number = default;
	private readonly string _nextSessionName = default;
	private WorldType _worldType = default;
	private List<RoomPlayer> _roomPlayers = new();


	public int this[PlayerRef playerRef] { get => _roomPlayers.IndexOf(playerRef); }
	public int LeaderIndex { get => _leaderIndex; }
	public PlayerRef LeaderPlayerRef { get => _roomPlayers[_leaderIndex].PlayerData; }
	/// <summary>
	/// リーダーは含まれない
	/// </summary>
	public int WithLeaderSessionCount
	{
		get
		{
			int count = 0;
			string leaderSessionName = _roomPlayers[_leaderIndex].SessionName;

			for (int i = 0; i < _roomPlayers.Count; i++)
			{
				if (i == _leaderIndex) { continue; }
				if (_roomPlayers[i].SessionName == leaderSessionName) { count++; }
			}
			return count;
		}
	}
	public int Number { get => _number; }
	public bool IsEndJoining { get => _isEndJoining; }
	public string NextSessionName { get => _nextSessionName; }
	public WorldType WorldType { get => _worldType; }
	public List<RoomPlayer> JoinRoomPlayer { get => _roomPlayers; }

	public Room(WorldType activityType, int roomNumber, string nextSessionName)
	{
		this._worldType = activityType;
		this._number = roomNumber;
		this._leaderIndex = 0;
		this._nextSessionName = nextSessionName;
	}

	public void Join(PlayerRef playerRef, string sessionName)
	{
		_roomPlayers.Add(new RoomPlayer(playerRef, sessionName));
	}

	/// <summary>
	/// 部屋から退出する
	/// </summary>
	/// <param name="playerRef">退出するプレイヤー</param>
	/// <returns>リザルト</returns>
	public RoomManager.LeftResult Left(PlayerRef playerRef)
	{

		int index = _roomPlayers.IndexOf(playerRef);
		//参加していなかった場合
		if (index < 0) { return RoomManager.LeftResult.Fail; }
		_roomPlayers.RemoveAt(index);

		//部屋のメンバーがいない場合
		if (_roomPlayers.Count <= 0) { return RoomManager.LeftResult.Closable; }

		RoomManager.LeftResult result = RoomManager.LeftResult.Success;

		//↓リーダーの場合
		if (_leaderIndex == index)
		{
			int nextLeaderIndex = Random.Range(0, _roomPlayers.Count);
			RoomManager.Instance.DestroyLeaderObject();
			ChangeLeader(nextLeaderIndex);

			result = RoomManager.LeftResult.LeaderChanged;
		}

		return result;
	}

	public void ChangeLeader(PlayerRef nextLeaderPlayer)
	{
		_leaderIndex = _roomPlayers.IndexOf(nextLeaderPlayer);
	}
	private void ChangeLeader(int nextLeaderIndex)
	{
		_leaderIndex = nextLeaderIndex;
	}

	public void ChengeSessionName(PlayerRef playerRef, string sessionName)
	{
		int index = _roomPlayers.IndexOf(playerRef);
		if (index < 0)
		{
			Debug.LogError("ルームに参加していません");
			return;
		}
		_roomPlayers[index].SessionName = sessionName;
	}
}
public class RoomManager : MonoBehaviour
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

	[SerializeField]
	private GameObject _leaderObjectPrefab;
	private GameObject _leaderObject;
	private static RoomManager _instance = default;
	private List<Room> _rooms = new();
	private int[] _roomCounter = default;
	public static RoomManager Instance { get => _instance; }

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
			_rooms.Where(room => room.JoinRoomPlayer.Contains(new Room.RoomPlayer(playerRef)));
		if (temp.Count() <= 0)
		{
			return null;
		}
		return temp.First();
	}

	/// <summary>
	/// アクティビティのルームに参加するまたはルームを作成する
	/// </summary>
	/// <param name="worldType">入りたいActivity</param>
	/// <param name="playerRef">入る人の情報</param>
	/// <param name="roomNumber">入りたい部屋の番号　マイナスの場合は入れる部屋に入る</param>
	/// <returns>JoinまたはCreateまたはFail</returns>
	public JoinOrCreateResult JoinOrCreate(WorldType worldType, PlayerRef playerRef, string currentSessionName, int roomNumber = -1)
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
			$"Result:{result}\nRoomNum:{roomTemp.Number}," +
			$"Player:{playerRef}",
			KumaDebugColor.NotificationColor);

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
	/// ルームから退出するまたはルームを閉じる
	/// </summary>
	/// <param name="playerRef">退出するプレイヤー情報</param>
	public void LeftOrClose(PlayerRef playerRef)
	{
		Room joinedRoom = GetCurrentRoom(playerRef);
		if (joinedRoom is null) { return; }
		XDebug.LogWarning(
			$"Left:{joinedRoom.WorldType}" +
			$"\nRoomNum:{joinedRoom.Number}" +
			$"Player:{playerRef}", KumaDebugColor.NotificationColor);
		LeftResult result = joinedRoom.Left(playerRef);
		if (result == LeftResult.Closable)
		{
			_roomCounter[(int)joinedRoom.WorldType]--;
			_rooms.Remove(joinedRoom);
		}
		else if (result == LeftResult.Fail)
		{
			XDebug.LogError("ルームに参加していません", KumaDebugColor.ErrorColor);
		}
	}

	private Room Create(WorldType activityType, int roomNumber, string leaderSessionName)
	{
		string nextSessionName = activityType + ":" + _roomCounter[(int)activityType];
		_rooms.Add(new Room(activityType, roomNumber, nextSessionName));
		_roomCounter[(int)activityType]++; ;
		return _rooms.LastOrDefault();
	}

	public void ChengeSessionName(PlayerRef playerRef, string currentSessionName)
	{
		Room room = GetCurrentRoom(playerRef);
		if (room == null)
		{
			XDebug.LogError("ルームが見つかりませんでした", KumaDebugColor.ErrorColor);
			return;
		}
		room.ChengeSessionName(playerRef, currentSessionName);
	}

	public void LeaderChange(PlayerRef leaderPlayer)
	{
		XDebug.LogWarning($"NewLeader{leaderPlayer}", KumaDebugColor.NotificationColor);
		Room roomTemp = GetCurrentRoom(leaderPlayer);
		//前のリーダーのリーダーオブジェクトを破棄する
		RPCManager.Instance.Rpc_DestroyLeaderObject(roomTemp.LeaderPlayerRef);
		if(leaderPlayer == GateOfFusion.Instance.NetworkRunner.LocalPlayer)
		{
			InstantiateLeaderObject();
		}
		roomTemp.ChangeLeader(leaderPlayer);
	}

	/// <summary>
	/// 自分以外のデータを破棄する
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
				$",LeaderWithCount:{room.WithLeaderSessionCount}\n" +
				$"Leader:{room.LeaderPlayerRef}," +
				$"PlayerCount{room.JoinRoomPlayer.Count}",KumaDebugColor.InformationColor);
			foreach (Room.RoomPlayer roomPlayer in room.JoinRoomPlayer)
			{
				XDebug.LogWarning($"RoomPlayer:{roomPlayer.PlayerData}" +
					$"PlayerSessionData:{roomPlayer.SessionName}",KumaDebugColor.InformationColor);
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Room
{
	private bool _isEndJoining = default;
	private int _leaderIndex = default;
	private int _roomNumber = default;
	private readonly string _nextSessionName = default;
	private WorldType _worldType = default;
	private List<RoomPlayer> _roomPlayers = new();
	private int _maxMemberCount = default;


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

	public int WithNextSessionCount
	{
		get
		{
			int count = 0;

			for (int i = 0; i < _roomPlayers.Count; i++)
			{
				if (_roomPlayers[i].SessionName == _nextSessionName) { count++; }
			}
			return count;
		}
	}
	public int RoomNumber { get => _roomNumber; }
	public bool IsEndJoining { get => _isEndJoining; }
	public string NextSessionName { get => _nextSessionName; }
	public WorldType WorldType { get => _worldType; }
	public List<RoomPlayer> JoinRoomPlayer { get => _roomPlayers; }

	public Room(WorldType activityType, int roomNumber, string nextSessionName)
	{
		this._worldType = activityType;
		this._roomNumber = roomNumber;
		this._leaderIndex = 0;
		this._nextSessionName = nextSessionName;
		switch (activityType)
		{
			case WorldType.CentralCity:
				{
					_maxMemberCount = -1;
					break;
				}
			case WorldType.UnderCook:
				{
					_maxMemberCount = -1;
					break;
				}
		}
	}

	public int GetPlayerIndex(PlayerRef playerRef)
	{
		return _roomPlayers.IndexOf(playerRef);
	}

	public void Join(PlayerRef playerRef, string sessionName)
	{
		_roomPlayers.Add(new RoomPlayer(playerRef, sessionName));
		if(_maxMemberCount < 0) { return; }
		if(_roomPlayers.Count >= _maxMemberCount)
		{
			Debug.LogError($"{_roomPlayers.Count}:{_maxMemberCount}");
			_isEndJoining = true;
		}
	}

	/// <summary>
	/// 部屋から退出する
	/// </summary>
	/// <param name="playerRef">退出するプレイヤー</param>
	/// <returns>リザルト</returns>
	public LeftResult Left(PlayerRef playerRef)
	{

		int index = _roomPlayers.IndexOf(playerRef);
		//参加していなかった場合
		if (index < 0) { return LeftResult.Fail; }
		_roomPlayers.RemoveAt(index);

		//部屋のメンバーがいない場合
		if (_roomPlayers.Count <= 0) { return LeftResult.Closable; }

		LeftResult result = LeftResult.Success;

		//↓リーダーの場合
		if (_leaderIndex == index)
		{
			int nextLeaderIndex = Random.Range(0, _roomPlayers.Count);
			RoomManager.Instance.DestroyLeaderObject();
			ChangeLeader(nextLeaderIndex);

			result = LeftResult.LeaderChanged;
		}

		if (_isEndJoining)
		{
			_isEndJoining = false;
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

	public void ChangeSessionName(PlayerRef playerRef, string sessionName)
	{
		int index = _roomPlayers.IndexOf(playerRef);
		if (index < 0)
		{
			XDebug.LogWarning("ルームに参加していません", KumaDebugColor.ErrorColor);
			return;
		}
		_roomPlayers[index].SessionName = sessionName;
	}
}
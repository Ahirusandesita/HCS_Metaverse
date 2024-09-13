using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
using Cysharp.Threading.Tasks;
using KumaDebug;
public class Room
{
	private bool _isEndJoining = default;
	private PlayerRef _leader = default;
	private readonly string _nextSessionName = default;
	private SceneNameType _worldType = default;
	private List<PlayerRef> _roomPlayers = new();
	private int _maxMemberCount = default;
	public int LeaderIndex { get => _roomPlayers.IndexOf(LeaderPlayerRef); }
	public PlayerRef LeaderPlayerRef => _leader;
	public bool IsEndJoining { get => _isEndJoining; }
	public string NextSessionName { get => _nextSessionName; }
	public SceneNameType SceneNameType { get => _worldType; }
	public List<PlayerRef> JoinRoomPlayer { get => _roomPlayers; }

	public Room(SceneNameType activityType, string nextSessionName)
	{
		this._worldType = activityType;
		this._nextSessionName = nextSessionName;

		_maxMemberCount = activityType switch
		{
			SceneNameType.TestPhotonScene => -1,
			SceneNameType.CookActivity => -1,
			SceneNameType.KumaKumaTest => -1,
			_ => -1,
		};
	}

	public int GetPlayerIndex(PlayerRef playerRef)
	{
		return _roomPlayers.IndexOf(playerRef);
	}

	public void Join(PlayerRef playerRef)
	{
		if(_roomPlayers.Count <= 0)
		{
			_leader = playerRef;
		}
		_roomPlayers.Add(playerRef);
		if (_maxMemberCount < 0) { return; }
		if (_roomPlayers.Count >= _maxMemberCount)
		{
			Debug.LogError($"{_roomPlayers.Count}:{_maxMemberCount}");
			_isEndJoining = true;
		}
	}

	/// <summary>
	/// 部屋から退出する
	/// </summary>
	/// <param name="leftPlayer">退出するプレイヤー</param>
	/// <returns>リザルト</returns>
	public async UniTask<LeftResult> Left(PlayerRef leftPlayer)
	{
		const int NEXT_LEADER_INDEX = 0;
		XKumaDebugSystem.LogWarning($"{leftPlayer}が退出しました:{LeaderIndex}", KumaDebugColor.ErrorColor);

		//参加していなかった場合
		if (!_roomPlayers.Contains(leftPlayer)) { return LeftResult.Fail; }
		//部屋のメンバーがいない場合
		if (_roomPlayers.Count <= 1) { return LeftResult.Closable; }

		LeftResult result = LeftResult.Success;

		//↓リーダーの場合
		if (_leader == leftPlayer)
		{
			_leader = JoinRoomPlayer[NEXT_LEADER_INDEX];
			RoomManager.Instance.DestroyLeaderObject();
			MasterServerConect masterServer = Object.FindObjectOfType<MasterServerConect>();
			await UniTask.WaitUntil(() => masterServer.Runner.ActivePlayers.Contains(_leader));
			masterServer.SessionRPCManager.Rpc_ChangeLeader(_leader);
			result = LeftResult.LeaderChanged;
		}
		_roomPlayers.Remove(leftPlayer);

		if (_isEndJoining)
		{
			_isEndJoining = false;
		}

		return result;
	}

	public void ChangeLeader(PlayerRef nextLeaderPlayer)
	{
		_leader = nextLeaderPlayer;
	}

	public void ChangeSessionName(PlayerRef playerRef, string sessionName)
	{
		int index = _roomPlayers.IndexOf(playerRef);
		if (index < 0)
		{
			XKumaDebugSystem.LogWarning("ルームに参加していません", KumaDebugColor.ErrorColor);
			return;
		}
	}
}
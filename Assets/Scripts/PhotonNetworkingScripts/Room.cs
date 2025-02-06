using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
using Cysharp.Threading.Tasks;
using KumaDebug;
public class Room
{
	private bool _isNonLeader = false;
	private bool _isEndJoining = default;
	private PlayerRef _leader = default;
	private readonly string _nextSessionName = default;
	private string _worldType = default;
	private List<PlayerRef> _roomPlayers = new();
	private int _maxParticipantCount = default;
	public bool IsNonLeader => _isNonLeader;
	public int LeaderIndex { get => _roomPlayers.IndexOf(LeaderPlayerRef); }
	public PlayerRef LeaderPlayerRef => _leader;
	public bool IsEndJoining { get => _isEndJoining; }
	public string NextSessionName { get => _nextSessionName; }
	public string SceneNameType { get => _worldType; }
	public List<PlayerRef> JoinRoomPlayer { get => _roomPlayers; }

	public Room(string activityType, string nextSessionName)
	{
		this._worldType = activityType;
		this._nextSessionName = nextSessionName;

		if (activityType == "KumaKumaTest"
			|| activityType == "TestPhotonScene")
		{
			_isNonLeader = true;
		}
		_maxParticipantCount = activityType switch
		{
			"TestPhotonScene" => -1,
			"CookActivity" => -1,
			"KumaKumaTest" => -1,
			_ => -1,
		};
	}

	public int GetPlayerIndex(PlayerRef playerRef)
	{
		return _roomPlayers.IndexOf(playerRef);
	}

	public void Join(PlayerRef playerRef)
	{
		if (!_isNonLeader && _roomPlayers.Count <= 0)
		{
			XKumaDebugSystem.LogWarning($"Leader{playerRef}", KumaDebugColor.ErrorColor);
			_leader = playerRef;
		}
		_roomPlayers.Add(playerRef);
		if (_maxParticipantCount < 0) { return; }
		if (_roomPlayers.Count >= _maxParticipantCount)
		{
			Debug.LogError($"{_roomPlayers.Count}:{_maxParticipantCount}");
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
		int nextLeaderIndex = 0;
		XKumaDebugSystem.LogWarning($"{leftPlayer}が退出しました:{LeaderIndex}", KumaDebugColor.ErrorColor);

		//参加していなかった場合
		if (!_roomPlayers.Contains(leftPlayer)) { return LeftResult.Fail; }
		LeftResult result;
		//部屋のメンバーがいない場合
		if (_roomPlayers.Count <= 1)
		{
			result = LeftResult.Closable;
		}
		else
		{
			result = LeftResult.Success;
		}

		//↓リーダーの場合
		if (_leader == leftPlayer)
		{
			if (_roomPlayers.Count > 1)
			{
				if (LeaderIndex == nextLeaderIndex)
				{
					nextLeaderIndex++;
				}
				XKumaDebugSystem.LogWarning($"{nextLeaderIndex}:leaderindex", KumaDebugColor.InformationColor);
				_leader = JoinRoomPlayer[nextLeaderIndex];
				MasterServerConect masterServer = Object.FindObjectOfType<MasterServerConect>();
				await UniTask.WaitUntil(() => masterServer.Runner.ActivePlayers.Contains(_leader));
				masterServer.SessionRPCManager.Rpc_ChangeLeader(_leader);
			}
			RoomManager.Instance.DestroyLeaderObject();
			result = LeftResult.LeaderChanged;
		}
		if (GateOfFusion.Instance.NetworkRunner.LocalPlayer == leftPlayer)
		{
			RoomManager.Instance.DestroyActivityStartUI();
		}
		_roomPlayers.Remove(leftPlayer);

		return result;
	}

	public bool IsLeader(PlayerRef playerRef)
	{
		return _leader == playerRef;
	}

	public void ChangeLeader(PlayerRef nextLeaderPlayer)
	{
		XKumaDebugSystem.LogWarning($"Leader{nextLeaderPlayer}", KumaDebugColor.ErrorColor);
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
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
using Cysharp.Threading.Tasks;
using KumaDebug;
public class Room
{
	private bool _isEndJoining = default;
	private int _leaderIndex = default;
	private readonly string _nextSessionName = default;
	private SceneNameType _worldType = default;
	private List<RoomPlayer> _roomPlayers = new();
	private int _maxMemberCount = default;
	public int LeaderIndex { get => _leaderIndex; }
	public PlayerRef LeaderPlayerRef
	{
		get
		{
			XKumaDebugSystem.LogWarning($"{_leaderIndex}:{_roomPlayers.Count}:{_roomPlayers[0].PlayerData}:{_nextSessionName}", KumaDebugColor.ErrorColor);
			return _roomPlayers[_leaderIndex].PlayerData;
		}
	}
	public bool IsEndJoining { get => _isEndJoining; }
	public string NextSessionName { get => _nextSessionName; }
	public SceneNameType SceneNameType { get => _worldType; }
	public List<RoomPlayer> JoinRoomPlayer { get => _roomPlayers; }

	public Room(SceneNameType activityType, string nextSessionName)
	{
		this._worldType = activityType;
		this._leaderIndex = 0;
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

	public void Join(PlayerRef playerRef, string sessionName)
	{
		_roomPlayers.Add(new RoomPlayer(playerRef, sessionName));
		if (_maxMemberCount < 0) { return; }
		if (_roomPlayers.Count >= _maxMemberCount)
		{
			Debug.LogError($"{_roomPlayers.Count}:{_maxMemberCount}");
			_isEndJoining = true;
		}
	}

	/// <summary>
	/// ��������ޏo����
	/// </summary>
	/// <param name="playerRef">�ޏo����v���C���[</param>
	/// <returns>���U���g</returns>
	public async UniTask<LeftResult> Left(PlayerRef playerRef)
	{
		XKumaDebugSystem.LogWarning($"{playerRef}���ޏo���܂���:{LeaderIndex}", KumaDebugColor.ErrorColor);
		int index = _roomPlayers.IndexOf(playerRef);
		RoomPlayer leaderPlayer = _roomPlayers[LeaderIndex];
		//�Q�����Ă��Ȃ������ꍇ
		if (index < 0) { return LeftResult.Fail; }
		_roomPlayers.RemoveAt(index);

		//�����̃����o�[�����Ȃ��ꍇ
		if (_roomPlayers.Count <= 0) { return LeftResult.Closable; }

		LeftResult result = LeftResult.Success;

		//�����[�_�[�̏ꍇ
		if (_leaderIndex == index)
		{
			int nextLeaderIndex = Random.Range(0, _roomPlayers.Count);
			XKumaDebugSystem.LogWarning($"{nextLeaderIndex}", KumaDebugColor.MessageColor);
			leaderPlayer�@= new RoomPlayer(JoinRoomPlayer[nextLeaderIndex].PlayerData);
			RoomManager.Instance.DestroyLeaderObject();
			MasterServerConect masterServer = Object.FindObjectOfType<MasterServerConect>();
			await UniTask.WaitUntil(() => masterServer.Runner.ActivePlayers.Contains(leaderPlayer.PlayerData));
			masterServer.SessionRPCManager.Rpc_ChangeLeader(leaderPlayer.PlayerData);
			result = LeftResult.LeaderChanged;
		}
		_leaderIndex = _roomPlayers.IndexOf(leaderPlayer);


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
	public void ChangeSessionName(PlayerRef playerRef, string sessionName)
	{
		int index = _roomPlayers.IndexOf(playerRef);
		if (index < 0)
		{
			XKumaDebugSystem.LogWarning("���[���ɎQ�����Ă��܂���", KumaDebugColor.ErrorColor);
			return;
		}
		_roomPlayers[index].SessionName = sessionName;
	}
}
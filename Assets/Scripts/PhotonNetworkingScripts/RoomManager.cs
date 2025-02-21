using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;
using Cysharp.Threading.Tasks;
using KumaDebug;
using System;

/*
 * ���[���ɓ���Ƃ��ɊJ�n�����X�g��\�����ĉ������炻���Ɋϐ�œ���
 * �V�������[������邱�Ƃ��\
 */

public class RoomManager : MonoBehaviour
{
	[SerializeField]
	private GameObject _leaderObjectPrefab;
	[SerializeField]
	private GameObject _activityStartUIPrefab;
	private ActivityMemberTextController _activityMemberTextController;
	private List<Room> _rooms = new();
	private GameObject _leaderObject;
	private GameObject _activityStartUI;
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
		XKumaDebugSystem.LogWarning($"�폜�FRoomManager", KumaDebugColor.ErrorColor);
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

	public void OpenActivityStartUI()
	{
		if (_activityStartUI != null)
		{
			_activityStartUI.SetActive(true);
		}
	}

	public Room FindCurrentRoom(PlayerRef playerRef)
	{
		if (_rooms.Count < 1) { return null; }
		Room temp = _rooms.FirstOrDefault(room => room.JoinRoomPlayer.Contains(playerRef));
		return temp;
	}

	public async UniTask<JoinOrCreateResult> JoinOrCreate(string sceneNameType, PlayerRef joinPlayer, int sessionNum = 0)
	{
		Room myRoom = FindCurrentRoom(joinPlayer);
		if (myRoom != null)
		{
			XKumaDebugSystem.LogWarning($"{joinPlayer}�͂��łɃ��[���ɎQ�����Ă��܂�", KumaDebugColor.WarningColor);
			await Instance.LeftOrClose(joinPlayer);
		}

		JoinOrCreateResult result = default;
		Room roomTemp = _rooms.Where(room => room.NextSessionName == sceneNameType + sessionNum).FirstOrDefault();

		if (roomTemp == null)
		{
			roomTemp = Create(sceneNameType, sceneNameType + sessionNum);
			result = JoinOrCreateResult.Create;
		}
		else
		{
			result = JoinOrCreateResult.Join;
		}

		if (_activityMemberTextController != null)
		{
			_activityMemberTextController.UpdateText();
		}

		roomTemp.Join(joinPlayer);

		if (!roomTemp.IsNonLeader && joinPlayer == GateOfFusion.Instance.NetworkRunner.LocalPlayer)
		{
			InstantiateLeaderObject();
			InstantiateActivityStartUI();
		}
		XKumaDebugSystem.LogWarning($"���[���ɎQ��:{sceneNameType}," +
			$"Result:{result}\n," +
			$"Player:{joinPlayer}",
			KumaDebugColor.InformationColor);

		return result;
	}

	public void InstantiateLeaderObject()
	{
		XKumaDebugSystem.LogWarning($"InstanceLeaderObject:{MasterServerConect.Runner.LocalPlayer}", KumaDebugColor.SuccessColor);
		Leader leaderTemp = FindObjectOfType<Leader>();
		if (leaderTemp != null)
		{
			XKumaDebugSystem.LogWarning($"�G���[�F���[�_�[�I�u�W�F�N�g����������`");
		}
		_leaderObject = Instantiate(_leaderObjectPrefab);
	}

	public void InstantiateActivityStartUI()
	{
		_activityStartUI = Instantiate(_activityStartUIPrefab);
		Room room = FindCurrentRoom(GateOfFusion.Instance.NetworkRunner.LocalPlayer);
		XKumaDebugSystem.LogWarning($"{room}:{_activityStartUI},Player:{GateOfFusion.Instance.NetworkRunner.LocalPlayer}", KumaDebugColor.RpcColor);
		if (room.LeaderPlayerRef != GateOfFusion.Instance.NetworkRunner.LocalPlayer)
		{
			Destroy(FindObjectOfType<ActivityStartButton>().gameObject);
		}
		_activityStartUI.SetActive(false);
		XKumaDebugSystem.LogWarning($"���[�_�[UI����", KumaDebugColor.InformationColor);
	}

	public void DestroyActivityStartUI()
	{
		XKumaDebugSystem.LogWarning($"���[�_�[UI�폜", KumaDebugColor.InformationColor);
		if (_activityStartUI != null)
		{
			Destroy(_activityStartUI);
		}
		else
		{
			XKumaDebugSystem.LogWarning($"���[�_�[UI��Null�Ȃ̂Ŕj���ł��܂���ł����B", KumaDebugColor.WarningColor);
		}
	}

	public void DestroyLeaderObject()
	{
		XKumaDebugSystem.LogWarning($"DestoryLeaderObject:{MasterServerConect.Runner.LocalPlayer}", KumaDebugColor.SuccessColor);
		if (_leaderObject != null)
		{
			Destroy(_leaderObject);
		}
		else
		{
			XKumaDebugSystem.LogWarning($"���[�_�[�I�u�W�F�N�g��Null�Ȃ̂Ŕj���ł��܂���ł����B", KumaDebugColor.WarningColor);
		}

	}

	/// <summary>
	/// ���[������ޏo����܂��̓��[�������
	/// </summary>
	/// <param name="playerRef">�ޏo����v���C���[���</param>
	public async UniTask LeftOrClose(PlayerRef playerRef)
	{
		Room joinedRoom = FindCurrentRoom(playerRef);
		if (joinedRoom is null) { return; }
		if (joinedRoom.IsLeader(playerRef))
		{
			DestroyLeaderObject();
		}
		XKumaDebugSystem.LogWarning(
			$"�ޏo:{joinedRoom.SceneNameType}" +
			$"\nRoom:{joinedRoom.NextSessionName}" +
			$"Player:{playerRef}", KumaDebugColor.InformationColor);
		LeftResult result = await joinedRoom.Left(playerRef);
		if (result == LeftResult.Closable)
		{
			_rooms.Remove(joinedRoom);
		}
		else if (result == LeftResult.Fail)
		{
			XKumaDebugSystem.LogError("���[���ɎQ�����Ă��܂���", KumaDebugColor.ErrorColor);
		}
	}

	private Room Create(string activityType, string sessionName)
	{
		Room newRoom = new Room(activityType, sessionName);
		_rooms.Add(newRoom);
		return newRoom;
	}

	public void ChangeSessionName(PlayerRef playerRef, string currentSessionName)
	{
		Room room = FindCurrentRoom(playerRef);
		if (room == null)
		{
			XKumaDebugSystem.LogWarning("���[����������܂���ł���", KumaDebugColor.ErrorColor);
			return;
		}
		room.ChangeSessionName(playerRef, currentSessionName);
	}

	public async void LeaderChange(PlayerRef nextLeaderPlayer)
	{
		XKumaDebugSystem.LogWarning($"�V���[�_�[{nextLeaderPlayer}", KumaDebugColor.SuccessColor);
		Room roomTemp = FindCurrentRoom(nextLeaderPlayer);

		await UniTask.WaitUntil(() => MasterServerConect.Runner.ActivePlayers.Contains(nextLeaderPlayer));

		if (MasterServerConect.Runner.ActivePlayers.Count() > 1)
		{
			await UniTask.WaitUntil(() => MasterServerConect.Runner.ActivePlayers.Contains(roomTemp.LeaderPlayerRef));
		}
		//�O�̃��[�_�[�̃��[�_�[�I�u�W�F�N�g��j������
		bool isLeader = nextLeaderPlayer == GateOfFusion.Instance.NetworkRunner.LocalPlayer;
		if (roomTemp.IsNonLeader) { return; }
		roomTemp.ChangeLeader(nextLeaderPlayer);
		if (isLeader)
		{
			DestroyActivityStartUI();
			DestroyLeaderObject();
			InstantiateLeaderObject();
			InstantiateActivityStartUI();
		}

	}

	/// <summary>
	/// �����ȊO�̃f�[�^��j������
	/// </summary>
	public void Initialize(PlayerRef myPlayerRef)
	{
		XKumaDebugSystem.LogWarning($"���[���}�l�[�W���[������", KumaDebugColor.SuccessColor);
		Room myRoom = FindCurrentRoom(myPlayerRef);
		XKumaDebugSystem.LogWarning($"{myRoom}:{myPlayerRef}:", KumaDebugColor.MessageColor);
		bool isLeader = myRoom.LeaderPlayerRef == myPlayerRef;
		_rooms.Clear();
		XKumaDebugSystem.LogWarning($"�N���A");
		_rooms.Add(myRoom);
		XKumaDebugSystem.LogWarning($"add");
		if (myRoom.IsNonLeader) { return; }
		if (isLeader) { myRoom.ChangeLeader(myPlayerRef); }
	}

	[ContextMenu("DebugRoomData")]
	public void Test()
	{
		if (_rooms.Count <= 0)
		{
			XKumaDebugSystem.LogWarning("���[��������܂���", KumaDebugColor.MessageColor);
			return;
		}
		foreach (Room roomData in _rooms)
		{
			XKumaDebugSystem.LogWarning(
				$"RoomData::,NextSessionName:{roomData.NextSessionName}" +
				$"Leader:{roomData.LeaderPlayerRef}," +
				$"PlayerCount{roomData.JoinRoomPlayer.Count}"
				, KumaDebugColor.InformationColor);
			foreach (PlayerRef player in roomData.JoinRoomPlayer)
			{
				XKumaDebugSystem.LogWarning($"{player}", KumaDebugColor.InformationColor);
			}
		}
	}
}
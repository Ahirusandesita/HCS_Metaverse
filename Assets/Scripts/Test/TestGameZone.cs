using UnityEngine;
using Fusion;

namespace HCSMeta.Activity
{
	public class TestGameZone : MonoBehaviour, IInteraction, ISelectedNotification
	{
		public bool IsFiredTriggerStay { get; set; }
		public ISelectedNotification SelectedNotification => this;
		[SerializeField]
		private GameFrame gameFrame;
		[SerializeField, Header("�ړ�����A�N�e�B�r�e�B(���[���h)")]
		private RegisterSceneInInspector _sceneNameType;
		private MasterServerConect _masterServer;
		[SerializeField]
		private GameObject _roomSelecter;
		private NetworkRunner NetworkRunner => GateOfFusion.Instance.NetworkRunner;
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


		[ContextMenu("Close")]
		public void Close()
		{
			gameFrame.Close();
			if (_sceneNameType == "MyRoom")
			{
				_roomSelecter.SetActive(false);
				return;
			}
			MasterServerConect.SessionRPCManager.Rpc_LeftOrCloseRoom(NetworkRunner.LocalPlayer);
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.P))
			{
				Open();
			}
			else if (Input.GetKeyDown(KeyCode.Semicolon))
			{
				Close();
			}
#endif
		}

		[ContextMenu("Open")]
		public IInteraction.InteractionInfo Open()
		{
			gameFrame.GameStart();

			//if(_sceneNameType == "MyRoom")
			//{
			//	_roomSelecter ??= FindObjectOfType<MyRoomSelector>().gameObject;
			//	_roomSelecter.SetActive(true);
			//	return new IInteraction.NullInteractionInfo();
			//}

			//���[���ɎQ������
			if (MasterServerConect.IsUsePhoton && NetworkRunner.SessionInfo.PlayerCount > 1)
			{
				MasterServerConect.SessionRPCManager.Rpc_JoinOrCreateRoom(_sceneNameType, NetworkRunner.LocalPlayer);
			}
			else
			{
				_ = RoomManager.Instance.JoinOrCreate(
					_sceneNameType, NetworkRunner.LocalPlayer);
			}
			RoomManager roomManager = FindAnyObjectByType<RoomManager>();
			if (roomManager != null)
			{
				roomManager.OpenActivityStartUI();
			}

			// To Kuma: �����v���C���[�Ƃ��ɂ��̃^�C�~���O�ŏ��𑗂肽���ꍇ�́AIInteraction.InteractionInfo���p�������N���X��n���B
			return new IInteraction.NullInteractionInfo();
		}

		public void Select(SelectArgs selectArgs)
		{

		}

		public void Unselect(SelectArgs selectArgs)
		{

		}
	}
}

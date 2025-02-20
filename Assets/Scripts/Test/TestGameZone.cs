using UnityEngine;
using Fusion;
using Cysharp.Threading.Tasks;

namespace HCSMeta.Activity
{
	public class TestGameZone : MonoBehaviour, IInteraction, ISelectedNotification
	{
		public bool IsFiredTriggerStay { get; set; }
		public ISelectedNotification SelectedNotification => this;
		public enum ShopType
		{
			InteriorShop = 0,
			CostumeShop = 1
		}
		[SerializeField]
		private ShopType _shopType;
		[SerializeField]
		private GameFrame gameFrame;
		[SerializeField, Header("移動するアクティビティ(ワールド)")]
		private RegisterSceneInInspector _sceneNameType;
		private MasterServerConect _masterServer;
		[SerializeField]
		private MyRoomSelector _roomSelecter;
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
				_roomSelecter.gameObject.SetActive(false);
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

			if (_sceneNameType == "MyRoom")
			{
				_roomSelecter ??= FindAnyObjectByType<MyRoomSelector>(FindObjectsInactive.Include);
				_roomSelecter.gameObject.SetActive(true);
				return new IInteraction.NullInteractionInfo();
			}
			else if (_sceneNameType == "Shop")
			{
				if (_shopType == ShopType.InteriorShop)
				{
					PlayerDontDestroyData.Instance.MovableShopID = PlayerDontDestroyData.Instance.ShopIDs[0];
				}
				else if (_shopType == ShopType.CostumeShop)
				{
					PlayerDontDestroyData.Instance.MovableShopID = PlayerDontDestroyData.Instance.ShopIDs[1];
				}
			}

			//ルームに参加する
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

			// To Kuma: もしプレイヤーとかにこのタイミングで情報を送りたい場合は、IInteraction.InteractionInfoを継承したクラスを渡す。
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

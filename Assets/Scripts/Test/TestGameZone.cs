using UnityEngine;
using Fusion;

namespace HCSMeta.Activity
{
	public class TestGameZone : MonoBehaviour, IInteraction, ISelectedNotification
	{
		public ISelectedNotification SelectedNotification => this;
		[SerializeField]
		private GameFrame gameFrame;
		[SerializeField, Header("移動するアクティビティ(ワールド)")]
		private RegisterSceneInInspector _sceneNameType;
		private MasterServerConect _masterServer;
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
			MasterServerConect.SessionRPCManager.Rpc_LeftOrCloseRoom(NetworkRunner.LocalPlayer);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.P))
			{
				Open();
			}
			else if (Input.GetKeyDown(KeyCode.Semicolon))
			{
				Close();
			}
		}

		[ContextMenu("Open")]
		public IInteraction.InteractionInfo Open()
		{
			gameFrame.GameStart();

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

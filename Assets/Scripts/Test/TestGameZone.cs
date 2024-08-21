using UnityEngine;
using Fusion;
using HCSMeta.Network;
namespace HCSMeta.Activity
{
	public class TestGameZone : MonoBehaviour, IInteraction, ISelectedNotification
	{
		public ISelectedNotification SelectedNotification => this;
		[SerializeField]
		private GameFrame gameFrame;
		[SerializeField, Header("�A�N�e�B�r�e�B(���[���h)")]
		private WorldType _worldType;
		private NetworkRunner NetworkRunner => GateOfFusion.Instance.NetworkRunner;

		[ContextMenu("Close")]
		public void Close()
		{
			Debug.Log("Nishigaki");
			gameFrame.Close();
			RPCManager.Instance.Rpc_LeftOrCloseRoom(NetworkRunner.LocalPlayer);
		}
		[ContextMenu("Open")]
		public void Open()
		{
			gameFrame.GameStart();

			//���[���ɎQ������
			if (NetworkRunner.SessionInfo.PlayerCount > 1)
			{
				RPCManager.Instance.Rpc_JoinOrCreateRoom(_worldType, NetworkRunner.LocalPlayer);
			}
			else
			{
				RoomManager.Instance.JoinOrCreate(
					_worldType, NetworkRunner.LocalPlayer,
					NetworkRunner.SessionInfo.Name);
			}
		}

		public void Select(SelectArgs selectArgs)
		{

		}

		public void Unselect(SelectArgs selectArgs)
		{

		}
	}
}

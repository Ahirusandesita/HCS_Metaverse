using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cysharp.Threading.Tasks;
using Kuma;

namespace HCSMeta.Network
{
	public class GateOfFusion
	{
		private bool _canUsePhoton = default;
		private NetworkRunner _networkRunner = default;
		private static GateOfFusion _instance = default;
		private MasterServerConect _masterServer = default;

		public static GateOfFusion Instance => _instance ??= new GateOfFusion();
		private MasterServerConect MasterServer 
			=> _masterServer ??= Object.FindObjectOfType<MasterServerConect>();
		public NetworkRunner NetworkRunner
		{
			get
			{
				if (_networkRunner == null)
				{
					_networkRunner = Object.FindObjectOfType<NetworkRunner>();
				}
				return _networkRunner;
			}
			set
			{
				_networkRunner = value;
			}
		}
		public bool IsCanUsePhoton { get => _canUsePhoton; set => _canUsePhoton = value; }

		/// <summary>
		/// 掴むときに呼ぶ
		/// </summary>
		/// <param name="networkObject">掴んだオブジェクト</param>
		public void Grab(NetworkObject networkObject)
		{
			XDebug.LogWarning($"Grab:{networkObject.name}",KumaDebugColor.InformationColor);
			StateAuthorityData stateAuthorityData = networkObject.GetComponent<StateAuthorityData>();
			if (stateAuthorityData.IsNotReleaseStateAuthority)
			{
				XDebug.LogWarning($"権限がありませんでした",KumaDebugColor.ErrorColor);
				return;
			}
			if (networkObject.HasStateAuthority)
			{
				XDebug.LogWarning($"自分が権限を持っています",KumaDebugColor.WarningColor);
				return;
			}
			RPCManager.Instance.Rpc_GrabStateAuthorityChanged(networkObject);
			networkObject.RequestStateAuthority();

		}

		public void Release(NetworkObject networkObject)
		{
			RPCManager.Instance.Rpc_ReleseStateAuthorityChanged(networkObject);
		}

		public async void ActivityStart(string sceneName)
		{
			//アクティビティスタート
			Room currentRoom = RoomManager.Instance.GetCurrentRoom(NetworkRunner.LocalPlayer);
			if (currentRoom is null)
			{
				XDebug.LogWarning("どのルームにも入っていません", KumaDebugColor.ErrorColor);
				return;
			}
			if (currentRoom.LeaderPlayerRef != NetworkRunner.LocalPlayer)
			{
				XDebug.LogWarning("リーダーではありません", KumaDebugColor.ErrorColor);
				return;
			}
			string sessionName = currentRoom.NextSessionName;
			foreach (Room.RoomPlayer roomPlayer in currentRoom.JoinRoomPlayer)
			{
				if (roomPlayer.PlayerData == NetworkRunner.LocalPlayer) { continue; }
				RPCManager.Instance.Rpc_JoinSession(sessionName, roomPlayer.PlayerData);
			}
			await UniTask.WaitUntil(() => currentRoom.WithLeaderSessionCount <= 0);
			await MasterServer.JoinOrCreateSession(sessionName);
			if (currentRoom.LeaderPlayerRef == NetworkRunner.LocalPlayer)
			{
				await UniTask.WaitUntil(() => NetworkRunner.IsSharedModeMasterClient);
				await NetworkRunner.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
			}
			else if (NetworkRunner.IsSharedModeMasterClient)
			{
				NetworkRunner.SetMasterClient(currentRoom.LeaderPlayerRef);
			}
		}
	}
}

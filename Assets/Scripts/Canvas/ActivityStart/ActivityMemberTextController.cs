using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using KumaDebug;
using Cysharp.Threading.Tasks;

public class ActivityMemberTextController : MonoBehaviour
{
	private Room _currentRoom = default;
	[SerializeField]
	private Text _text = default;
	
	private async void OnEnable()
	{
		_text ??= GetComponent<Text>();
		bool isCanceled = await UniTask.WaitUntil(() 
			=> GateOfFusion.Instance.NetworkRunner != null,cancellationToken:destroyCancellationToken)
			.SuppressCancellationThrow();
		isCanceled = await UniTask.WaitUntil(()
			=> RoomManager.Instance.FindCurrentRoom(GateOfFusion.Instance.NetworkRunner.LocalPlayer) != null,
			cancellationToken:destroyCancellationToken).SuppressCancellationThrow();
		if (isCanceled)
		{
			XKumaDebugSystem.LogWarning("ƒLƒƒƒ“ƒZƒ‹‚³‚ê‚Ü‚µ‚½");
			return;
		}
		_currentRoom = RoomManager.Instance.FindCurrentRoom(GateOfFusion.Instance.NetworkRunner.LocalPlayer);
		DisplayTextData();
	}

	public void UpdateText()
	{
		DisplayTextData();
	}

	private void DisplayTextData()
	{

		_text.text = "";
		foreach (PlayerRef playerRef in _currentRoom.JoinRoomPlayer)
		{
			_text.text += playerRef.ToString();
			_text.text += "\n";
		}
	}	
}

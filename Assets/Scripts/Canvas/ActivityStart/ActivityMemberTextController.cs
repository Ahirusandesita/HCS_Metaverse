using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class ActivityMemberTextController : MonoBehaviour
{
	private Room _currentRoom = default;
	[SerializeField]
	private Text _text = default;

	private void OnEnable()
	{
		_text ??= GetComponent<Text>();
		_currentRoom = RoomManager.Instance.GetCurrentRoom(GateOfFusion.Instance.NetworkRunner.LocalPlayer);
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

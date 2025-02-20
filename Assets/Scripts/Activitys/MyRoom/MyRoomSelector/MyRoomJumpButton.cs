using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class MyRoomJumpButton : MonoBehaviour, IPointerClickHandler
{
	[SerializeField]
	private Text _playerNameText;
	private int _playerID;
	private string _playerName;

	public void Init(int playerID, string playerName)
	{
		this._playerID = playerID;
		this._playerName = playerName;
		_playerNameText.text = playerName;
	}
	public async void OnPointerClick(PointerEventData data)
	{
		PlayerDontDestroyData.Instance.MovableMyRoomUserID = _playerID;

		await RoomManager.Instance.JoinOrCreate(
			"MyRoom",
			GateOfFusion.Instance.NetworkRunner.LocalPlayer,
			_playerID);

		GateOfFusion.Instance.ActivityStart();
	}

	[ContextMenu("Click")]
	private void Test()
	{
		PlayerDontDestroyData.Instance.MovableMyRoomUserID = _playerID;

		RoomManager.Instance.JoinOrCreate(
			"MyRoom",
			GateOfFusion.Instance.NetworkRunner.LocalPlayer,
			_playerID)
			.Forget();

		GateOfFusion.Instance.ActivityStart();
	}
}

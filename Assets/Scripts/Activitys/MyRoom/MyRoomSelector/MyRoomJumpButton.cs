using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyRoomJumpButton : MonoBehaviour, IPointerClickRegistrable
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
	public void OnPointerClick(PointerEventData data)
	{

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoginManager : MonoBehaviour, ISendableMessage
{
	public enum Inputting
	{
		UserName,
		Password,
	}
	private TextMeshProUGUI _textMeshProUGUI;
	[SerializeField, HideAtPlaying]
	private string _userName;
	[SerializeField, HideAtPlaying]
	private string _password;
	[SerializeField, Hide]
	private Inputting _currentInputting;

	public Inputting CurrentInputting { set => _currentInputting = value; }
	private async void Start()
	{
		WebAPIRequester webAPIRequester = new WebAPIRequester();
		await webAPIRequester.PostLogin(_userName, _password);
		WebAPIRequester.OnCatchUserLocationData data = await webAPIRequester.GetUserLocation();
	}

	void ISendableMessage.SendMessage(string message)
	{
		if (_currentInputting == Inputting.UserName)
		{
			_userName = message;
		}
		else if (_currentInputting == Inputting.Password)
		{
			_password = message;
		}
	}
}

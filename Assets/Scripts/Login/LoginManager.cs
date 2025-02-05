using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
	[SerializeField, HideAtPlaying]
	private string _userName;
	[SerializeField, HideAtPlaying]
	private string _password;
	private async void Start()
	{
		WebAPIRequester webAPIRequester = new WebAPIRequester();
		await webAPIRequester.PostLogin(_userName, _password);
		WebAPIRequester.OnCatchUserLocationData data = await webAPIRequester.GetUserLocation();
		foreach(var item in data.SessionList)
		{
			XDebug.LogWarning(item.UserID);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
	public enum Inputting
	{
		UserName,
		Password,
	}
	[SerializeField]
	private TextMeshProUGUI _resultText;
	[SerializeField]
	private TextMeshProUGUI _inputNameText;
	[SerializeField]
	private TextMeshProUGUI _inputPasswordText;
	private const string _ERROR_LOGIN = _ERROR_PREFIX + "Login Failed";
	private const string _ERROR_PREFIX = "Error : ";
	private const string _ERROR_MESSAGE_OVERFLOW = _ERROR_PREFIX + "Characters Limit(10)";
	private readonly Color _ERROR_MESSAGE_COLOR = Color.red;
	private readonly int _MESSAGE_LIMIT = 15;
	[SerializeField, HideAtPlaying]
	private string _userName = "";
	[SerializeField, HideAtPlaying]
	private string _password = "";
	[SerializeField]
	private RegisterSceneInInspector _nextScene;
	public async UniTask ExecuteLogin(WebAPIRequester webAPIRequester)
	{
		if(_MESSAGE_LIMIT < _inputNameText.text.Length 
			&& _MESSAGE_LIMIT < _inputPasswordText.text.Length)
		{
			_resultText.color = _ERROR_MESSAGE_COLOR;
			_resultText.text = _ERROR_MESSAGE_OVERFLOW;
			return;
		}

		_userName = _inputNameText.text;
		_password = _inputPasswordText.text;

		bool loginResult = await webAPIRequester.PostLogin(_userName, _password);
		if (!loginResult)
		{
			_resultText.color = _ERROR_MESSAGE_COLOR;
			_resultText.text = _ERROR_LOGIN;
			return;
		}
		_resultText.text = "Success";
		_resultText.color = Color.green;

		await webAPIRequester.PostOnline();

		var costumeData = await webAPIRequester.GetCosutume();
		PlayerDontDestroyData.Instance.hair = costumeData.GetBody.Hair;
		PlayerDontDestroyData.Instance.face = costumeData.GetBody.Face;
		PlayerDontDestroyData.Instance.headGear = costumeData.GetBody.HeadGear;
		PlayerDontDestroyData.Instance.top = costumeData.GetBody.Top;
		PlayerDontDestroyData.Instance.bottom = costumeData.GetBody.Bottom;
		PlayerDontDestroyData.Instance.bag = costumeData.GetBody.Bag;
		PlayerDontDestroyData.Instance.shoes = costumeData.GetBody.Shoes;
		PlayerDontDestroyData.Instance.glove = costumeData.GetBody.Glove;
		PlayerDontDestroyData.Instance.eyeWear = costumeData.GetBody.EyeWear;
		PlayerDontDestroyData.Instance.body = costumeData.GetBody.BodyCos;

		SceneManager.LoadScene(_nextScene);
	}

	[ContextMenu("Login")]
	public async UniTask ExecuteLogin()
	{
		WebAPIRequester webAPIRequester = new WebAPIRequester();
		await ExecuteLogin(webAPIRequester);
	}

	private async void Update()
	{
		if (Input.GetKeyDown(KeyCode.RightShift))
		{
			await ExecuteLogin();
		}
	}
}

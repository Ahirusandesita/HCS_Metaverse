using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class LoginInputZone : MonoBehaviour,IPointerClickHandler
{
	[SerializeField]
	private FlickKeyboardManager _flickKeyboardManager;
	[SerializeField]
	private TextMeshProUGUI _textMeshProUGUI;
	[SerializeField]
	private LoginManager _loginManager;
	[SerializeField]
	private LoginManager.Inputting _inputting;
	private void Reset()
	{
		_flickKeyboardManager ??= FindAnyObjectByType<FlickKeyboardManager>();
		_textMeshProUGUI ??= GetComponentInChildren<TextMeshProUGUI>();
		_loginManager ??= FindAnyObjectByType<LoginManager>(); 
	}

	public void OnPointerClick(PointerEventData data)
	{
		_flickKeyboardManager.TextMeshProUGUI = _textMeshProUGUI;
	}

	[ContextMenu("Click")]
	public void Test()
	{
		_flickKeyboardManager.TextMeshProUGUI = _textMeshProUGUI;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UniRx;

public class GoldViewer : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI text = default;
	private bool isOpen = default;

	private void Awake()
	{
		Inputter.Player.Options.performed += OnOptions;
		isOpen = false;
	}

	private void Start()
	{
		PlayerDontDestroyData.Instance.MoneyRP.Subscribe(money => text.text = $"Gold: {money}").AddTo(this);
		gameObject.SetActive(false);
	}

	public void OnOptions(InputAction.CallbackContext context)
	{
		isOpen = !isOpen;
		gameObject.SetActive(isOpen);
	}
}

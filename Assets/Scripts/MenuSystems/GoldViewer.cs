using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class GoldViewer : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI text = default;

	private void Start()
	{
		PlayerDontDestroyData.Instance.MoneyRP.Subscribe(money => text.text = $"Gold: {money}").AddTo(this);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Text;

public class VendingMachineEditPriceDigitUI : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _text = default;
	private const int _MAX_VALUE = 9;

	public void Init(int digit,VendingMachineEditPriceUI vendingMachineEditPriceUI)
	{
		foreach(var item in GetComponentsInChildren<VendingMachineEditPriceControllButton>())
		{
			item.Init(digit, vendingMachineEditPriceUI);
		}
	}

	public void UpdateText(int digit)
	{
		if(digit > _MAX_VALUE)
		{
			XDebug.LogWarning("digit�̒l���傫�����܂��B1���̎��R��(0�܂�)�ɂ��Ă��������B");
			return; 
		}else if(digit < 0)
		{
			XDebug.LogWarning("digit�̒l�����������܂��B1���̎��R��(0�܂�)�ɂ��Ă��������B");
			return;
		}
		_text.text = digit.ToString();
	}
}

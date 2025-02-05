using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineEditPriceControllButton : MonoBehaviour, IPointerClickRegistrable
{
	private enum Sign
	{
		plus,
		minus
	}

	private VendingMachineEditPriceUI _vendingMachineEditPriceUI = default;
	[SerializeField,Hide]
	private int _price = default;
	[SerializeField, HideAtPlaying]
	private Sign _sign;

	public void Init(int price,VendingMachineEditPriceUI vendingMachineEditPriceUI)
	{
		this._price = price;
		this._vendingMachineEditPriceUI = vendingMachineEditPriceUI;
		if (_sign == Sign.minus)
		{
			_price *= -1;
		}
	}

	public void OnPointerClick(PointerEventData data)
	{
		_vendingMachineEditPriceUI.Price += _price;
		_vendingMachineEditPriceUI.TextUpdate();
	}

	[ContextMenu("Click")]
	private void Test()
	{
		_vendingMachineEditPriceUI.Price += _price;
		_vendingMachineEditPriceUI.TextUpdate();
	}
}

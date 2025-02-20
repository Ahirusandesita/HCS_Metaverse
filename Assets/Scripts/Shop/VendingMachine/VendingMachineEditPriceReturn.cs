using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineEditPriceReturn : MonoBehaviour, IPointerClickHandler
{
	[SerializeField]
	private VendingMachineEditPriceUI _vendingMachineEditPriceUI;
	public void OnPointerClick(PointerEventData data)
	{
		_vendingMachineEditPriceUI.Close();
	}

	[ContextMenu("Click")]
	private void Test()
	{
		_vendingMachineEditPriceUI.Close();
	}
}

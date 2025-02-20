using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineEditAddProductButton : MonoBehaviour, IPointerClickHandler
{
	[SerializeField]
	private VendingMachineUIManager _vendingMachineUI;
	public void OnPointerClick(PointerEventData data)
	{
		_vendingMachineUI.OpenEditPanel();
	}

	[ContextMenu("Click")]
	private void Test()
	{
		_vendingMachineUI.OpenEditPanel();
	}
}

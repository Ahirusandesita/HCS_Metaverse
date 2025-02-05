using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineEditReturnButton : MonoBehaviour, IPointerClickRegistrable
{
	[SerializeField]
	private VendingMachineUIManager _vendingMachineUIManager = default;
	public void OnPointerClick(PointerEventData data)
	{
		_vendingMachineUIManager.CloseEditPanel();
		_vendingMachineUIManager.OpenEditerButtons();
	}

	[ContextMenu("Click")]
	private void Test()
	{
		_vendingMachineUIManager.CloseEditPanel();
		_vendingMachineUIManager.OpenEditerButtons();
	}
}

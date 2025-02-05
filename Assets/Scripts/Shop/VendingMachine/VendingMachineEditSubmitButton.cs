using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineEditSubmitButton : MonoBehaviour, IPointerClickRegistrable
{
	[SerializeField]
	private VendingMachineEditUI _vendingMachineEditUI;
	public void OnPointerClick(PointerEventData data)
	{
		_vendingMachineEditUI.Submit();
	}

	[ContextMenu("Click")]
	private void Test()
	{
		_vendingMachineEditUI.Submit();
	}
}

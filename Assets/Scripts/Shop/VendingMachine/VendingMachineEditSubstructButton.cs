using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineEditSubstructButton : MonoBehaviour, IPointerClickHandler
{
	[SerializeField]
	private VendingMachineEditUI _vendingMachineEditUI;
	public void OnPointerClick(PointerEventData data)
	{
		_vendingMachineEditUI.SubtractCount();
	}

	[ContextMenu("Click")]
	private void Test()
	{
		_vendingMachineEditUI.SubtractCount();
	}
}

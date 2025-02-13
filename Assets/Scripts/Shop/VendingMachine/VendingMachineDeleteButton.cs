using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineDeleteButton : MonoBehaviour, IPointerClickRegistrable
{
	[SerializeField]
	private VendingMachineBuyUI _vendingMachineUI;
	public void OnPointerClick(PointerEventData data)
	{
		_vendingMachineUI.DeleteProduct();
	}

	[ContextMenu("click")]
	private void test()
	{
		_vendingMachineUI.DeleteProduct();
	}
}

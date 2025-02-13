using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineChangeProductButton : MonoBehaviour, IPointerClickRegistrable
{
	[SerializeField]
	private VendingMachineBuyUI _vendingMachineBuyUI;
	public void OnPointerClick(PointerEventData data)
	{
		_vendingMachineBuyUI.OpenProductChangeMode();
	}

	[ContextMenu("Click")]
	private void Test()
	{
		_vendingMachineBuyUI.OpenProductChangeMode();
	}
}

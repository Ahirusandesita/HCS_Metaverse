using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachinePageButton : MonoBehaviour, IPointerClickHandler
{
	private enum PageButtonMode
	{
		Next,
		Previous
	}
	[SerializeField]
	private PageButtonMode _mode;
	[SerializeField]
	private VendingMachineUIManager _vendingMachineUIManager = default;
	public void OnPointerClick(PointerEventData eventData)
	{
		if(_mode == PageButtonMode.Next)
		{
			_vendingMachineUIManager.NextPage();
		}
		else if(_mode == PageButtonMode.Previous)
		{
			_vendingMachineUIManager.PreviousPage();
		}
	}

	[ContextMenu("click")]
	private void OnPointerClickTest()
	{
		if (_mode == PageButtonMode.Next)
		{
			_vendingMachineUIManager.NextPage();
		}
		else if (_mode == PageButtonMode.Previous)
		{
			_vendingMachineUIManager.PreviousPage();
		}
	}
}

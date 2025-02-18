using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReturnMainCityButton : MonoBehaviour,IPointerClickRegistrable
{

	public void OnPointerClick(PointerEventData data)
	{
		GateOfFusion.Instance.ReturnMainCity();
	}

	[ContextMenu("Click")]
	private void Test()
	{
		GateOfFusion.Instance.ReturnMainCity();
	}
}

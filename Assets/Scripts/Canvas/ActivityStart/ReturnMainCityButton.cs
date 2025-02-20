using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

public class ReturnMainCityButton : MonoBehaviour,IPointerClickHandler
{
	[SerializeField]
	private MyRoomLoader _myRoomLoader;

	public async void OnPointerClick(PointerEventData data)
	{
		await _myRoomLoader.UnLoad();
		GateOfFusion.Instance.ReturnMainCity();
	}

	[ContextMenu("Click")]
	private async UniTaskVoid Test()
	{
		await _myRoomLoader.UnLoad();
		GateOfFusion.Instance.ReturnMainCity();
	}
}

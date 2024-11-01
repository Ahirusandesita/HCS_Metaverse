using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CartOutButton : MonoBehaviour
{
    [SerializeField]
    private UIIcon uiIconController = default;

    public void OnPointerClick(PointerEventData eventData)
    {
        uiIconController.Delete();
    }

	[ContextMenu("click")]
    private void Test()
	{
        uiIconController.Delete();
    }
}

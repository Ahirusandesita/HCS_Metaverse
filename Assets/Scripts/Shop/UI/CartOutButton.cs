using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CartOutButton : MonoBehaviour,IPointerClickHandler
{
    [SerializeField]
    private InCartItemUI uiIconController = default;
   
    public void OnPointerClick(PointerEventData eventData)
    {
        uiIconController.Delete();
    }
    [ContextMenu("test")]
    private void OnPointerClickTest()
	{
        uiIconController.Delete();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuyButton : MonoBehaviour
{
    [SerializeField]
    private ShopCartUIManager _shopCartUIManager = default;

    public void OnPointerClick(PointerEventData eventData)
    {
        _shopCartUIManager.BuyButtonPush();
    }

    [ContextMenu("test")]
    public void Test()
	{
        _shopCartUIManager.BuyButtonPush();
	}
}

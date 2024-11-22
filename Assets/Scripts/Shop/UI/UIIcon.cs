using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using KumaDebug;
using UnityEngine.UI;

public class UIIcon : MonoBehaviour
{	 
	[SerializeField]
	private TMP_Text _text = default;
	[SerializeField]
	private Image _image = default;
	private ShopCartUIManager _shopCartUIManager = default;
	private int _id;

	private void Update()
	{
		if(_id == 10001 && Input.GetKeyDown(KeyCode.Return))
		{
			Delete();
		}
	}

	public void Init(Sprite itemSprite,ShopCartUIManager shopCartUIManager,Vector2 popAnchoredPosition,int id)
	{
		_id = id;
		RectTransform myTransform = this.transform as RectTransform;
		_shopCartUIManager = shopCartUIManager;
		_image.sprite = itemSprite;
		myTransform.anchoredPosition = popAnchoredPosition;
	}

	public void UpdateCount(int count)
	{
		if(count == 0)
		{
			Destroy(this.gameObject);
			return;
		}
		_text.SetText(count.ToString("x#"));
	}

	public void Delete()
	{
		_shopCartUIManager.DestoryCartUI(_id);
	}
}

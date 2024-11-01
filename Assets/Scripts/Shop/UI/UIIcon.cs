using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIcon : MonoBehaviour
{

    [SerializeField]
    private Image _iconImage = default;
	private ShopCartUIManager _shopCartUIManager = default;
	[SerializeField]
	private TMP_Text _text = default;
	private int _id;

	private void Update()
	{
		if(_id == 10001 && Input.GetKeyDown(KeyCode.Return))
		{
			Delete();
		}
	}

	public void Init(Sprite icon,ShopCartUIManager shopCartUIManager,int id)
	{
		_id = id;
		_shopCartUIManager = shopCartUIManager;
		_iconImage.sprite = icon;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VendingMachineUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _discountedPriceText;
    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private Image _image;
    [SerializeField]
    private GameObject _soldoutImageObject;
    [SerializeField]
    private GameObject _buyButton;
    private VendingMachineUIManager _uiManager;
    private int _id = -1;
    public int ID => _id;
    public GameObject BuyButton => _buyButton;
    public void Init(int id,int discountedPrice,VendingMachineUIManager uiManager,Sprite sprite,string name)
	{
        _id = id;
        _image.sprite = sprite;
        _uiManager = uiManager;
        _nameText.text = name;
        _discountedPriceText.text = discountedPrice.ToString("c");
        _soldoutImageObject.SetActive(false);
    }

    public void SoldOut()
	{
        _soldoutImageObject.SetActive(true);
        _buyButton.SetActive(false);
	}

    public void Buy()
	{
        _uiManager.Buy(_id);
	}
}

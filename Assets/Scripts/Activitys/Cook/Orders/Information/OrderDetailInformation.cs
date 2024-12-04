using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderViewDetailImformation : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;
    [SerializeField]
    private Image orderImage;
    [SerializeField]
    private List<Image> orderDetailImages;
    [SerializeField]
    private Image slider;
    
    public Image OrderImage => orderImage;
    public IReadOnlyList<Image> OrderDetailImages => orderDetailImages;

    private CustomerInformation customerInformation;


    public void Disable()
    {
        textMeshProUGUI.enabled = false;
        orderImage.enabled = false;
        slider.enabled = false;
        foreach (Image image in orderDetailImages)
        {
            image.enabled = false;
        }
    }
    public void Active()
    {
        textMeshProUGUI.enabled = true;
        orderImage.enabled = true;
        slider.enabled = true;
        foreach (Image image in orderDetailImages)
        {
            image.enabled = true;
        }
    }

    public void View(CommodityAsset commodityAsset,CustomerInformation customerInformation)
    {
        Active();

        //textMeshProUGUI.text = commodityAsset.name;
        this.customerInformation = customerInformation;
        if (customerInformation.IsFirst)
        {
            customerInformation.RemainingTime = customerInformation.OrderWaitingTime;
            customerInformation.IsFirst = false;
        }
        orderImage.sprite = commodityAsset.CommodityAppearance.CommoditySprite;

        for (int i = 0; i < commodityAsset.Commodities.Count; i++)
        {
            if (i == orderDetailImages.Count)
            {
                Debug.LogError("View‚É•`‰æ‰Â”\‚È‘fÞ—v‘f”‚ð’´‚¦‚Ä‚¢‚Ü‚·B");
                break;
            }
            orderDetailImages[i].sprite = commodityAsset.Commodities[i].CommodityAsset.CommodityAppearance.CommoditySprite;
        }

        for (int i = commodityAsset.Commodities.Count; i < orderDetailImages.Count; i++)
        {
            orderDetailImages[i].enabled = false;
        }
    }

    public void Reset()
    {
        customerInformation = null;

        textMeshProUGUI.text = "";
        orderImage.sprite = null;
        slider.enabled = false;
        foreach (Image image in orderDetailImages)
        {
            image.sprite = null;
        }

        Disable();
    }

    private void Update()
    {
        if(customerInformation == null)
        {
            return;
        }
        if(customerInformation.OrderWaitingType == OrderWaitingType.Hide)
        {
            slider.enabled = false;
        }

        slider.fillAmount = customerInformation.RemainingTime / customerInformation.OrderWaitingTime;
        customerInformation.RemainingTime -= Time.deltaTime;
    }
}
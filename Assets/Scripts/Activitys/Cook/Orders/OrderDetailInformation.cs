using System.Collections;
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

    public Image OrderImage => orderImage;
    public IReadOnlyList<Image> OrderDetailImages => orderDetailImages;

    public void Disable()
    {
        textMeshProUGUI.enabled = false;
        orderImage.enabled = false;
        foreach (Image image in orderDetailImages)
        {
            image.enabled = false;
        }
    }
    public void Active()
    {
        textMeshProUGUI.enabled = true;
        orderImage.enabled = true;
        foreach (Image image in orderDetailImages)
        {
            image.enabled = true;
        }
    }

    public void View(CommodityAsset commodityAsset)
    {
        Active();

        textMeshProUGUI.text = commodityAsset.name;

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
        textMeshProUGUI.text = "";
        orderImage.sprite = null;
        foreach (Image image in orderDetailImages)
        {
            image.sprite = null;
        }

        Disable();
    }
}
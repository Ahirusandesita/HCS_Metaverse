using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CommodityDetailView : MonoBehaviour
{
    private FoodUI[] foodUIs;
    [SerializeField]
    private Commodity commodity;
    private void Awake()
    {
        foodUIs = this.GetComponentsInChildren<FoodUI>();
    }
    private void Start()
    {
        foreach (FoodUI foodUI in foodUIs)
        {
            foodUI.ImageEnable(false);
        }
        if (commodity.CommodityAsset.Commodities.Count == 0)
        {
            View(foodUIs[0], commodity.CommodityAsset.CommodityAppearance.CommoditySprite);
            return;
        }
        for (int i = 0; i < commodity.CommodityAsset.Commodities.Count; i++)
        {
            View(foodUIs[i], commodity.CommodityAsset.Commodities[i].CommodityAsset.CommodityAppearance.CommoditySprite);
        }
    }
    private void View(FoodUI foodUI, Sprite sprite)
    {
        foodUI.ImageEnable(true);
        foodUI.SetSprite(sprite);
    }
}

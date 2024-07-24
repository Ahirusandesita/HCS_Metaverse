using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodUI : MonoBehaviour
{
    private Image commodityImage;

    private void Awake()
    {
        commodityImage = this.GetComponent<Image>();
    }
    public void SetSprite(Sprite sprite)
    {
        commodityImage.sprite = sprite;
    }
    public void ImageEnable(bool isEnable)
    {
        commodityImage.enabled = isEnable;
    }

}

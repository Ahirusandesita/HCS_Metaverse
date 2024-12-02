using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SelectItem : MonoBehaviour
{
    [SerializeField]
    private Image selectImage;
    [SerializeField]
    private TextMeshProUGUI selectText;
    private NotExistIcon notExistIcon;

    [SerializeField]
    private TextMeshProUGUI message;
    private void Awake()
    {
        selectText.text = "";
        selectImage.sprite = null;
        selectImage.enabled = false;
        selectText.enabled = false;

        message.enabled = false;
    }
    public void Select(ItemAsset itemAsset)
    {
        selectImage.enabled = true;
        selectText.enabled = true;
        selectImage.sprite = itemAsset.ItemIcon == null ? notExistIcon.Icon : itemAsset.ItemIcon;
        selectText.text = itemAsset.Name;
    }

    public void UnSelect()
    {
        selectText.text = "";
        selectImage.sprite = null;
        selectImage.enabled = false;
        selectText.enabled = false;
    }
    public void NotAvailable(ItemAsset itemAsset)
    {
        message.enabled = true;
        message.text = "‚±‚±‚Å‚ÍŽg—p‚Å‚«‚Ü‚¹‚ñ";
    }

    public void NotExistIconInject(NotExistIcon notExistIcon)
    {
        this.notExistIcon = notExistIcon;
    }
}

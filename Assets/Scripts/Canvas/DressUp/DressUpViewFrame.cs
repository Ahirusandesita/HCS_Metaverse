using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DressUpViewFrame : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI textMesh;
    [SerializeField]
    private Sprite nonExistItemIcon;

    private List<ItemAsset> itemAssets;
    private int index;

    public event Action<int, string> OnDressUp;

    private void Awake()
    {
        index = 0;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Click_Next();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Click_Previous();
        }
    }

    public void InjectItemAsset(List<ItemAsset> itemAssets)
    {
        this.itemAssets = itemAssets;

        icon.sprite = itemAssets[index].ItemIcon;
        textMesh.text = itemAssets[index].Name;
    }
    public void NonExistItemAsset()
    {
        icon.sprite = nonExistItemIcon;
        textMesh.text = "ŠŽ‚µ‚Ä‚¢‚Ü‚¹‚ñB";
    }

    public void Click_Next()
    {
        if (itemAssets == null)
        {
            return;
        }

        index++;
        if (index > itemAssets.Count - 1)
        {
            index = 0;
        }
        OnDressUp?.Invoke(itemAssets[index].ID, itemAssets[index].Name);
        icon.sprite = itemAssets[index].ItemIcon;
        textMesh.text = itemAssets[index].Name;
    }
    public void Click_Previous()
    {
        if (itemAssets == null)
        {
            return;
        }

        index--;
        if (index < 0)
        {
            index = itemAssets.Count - 1;
        }
        OnDressUp?.Invoke(itemAssets[index].ID, itemAssets[index].Name);
        icon.sprite = itemAssets[index].ItemIcon;
        textMesh.text = itemAssets[index].Name;
    }
    private void OnDestroy()
    {
        OnDressUp = null;
    }
}

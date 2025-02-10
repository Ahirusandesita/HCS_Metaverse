using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Layer_lab._3D_Casual_Character;

public class DressUpViewFrame : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI textMesh;
    [SerializeField]
    private Sprite nonExistItemIcon;

    private List<ViewInfo> viewInfos = new List<ViewInfo>();
    private int index;

    public event Action<int, string> OnDressUp;

    private class ViewInfo
    {
        public readonly Sprite Icon;
        public readonly string Name;
        public readonly int ID;
        public ViewInfo(Sprite icon, string name, int id)
        {
            this.Icon = icon;
            this.Name = name;
            this.ID = id;
        }
    }

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
        foreach (ItemAsset item in itemAssets)
        {
            viewInfos.Add(new ViewInfo(item.ItemIcon, item.Name, item.ID));
        }

        icon.sprite = viewInfos[index].Icon;
        textMesh.text = viewInfos[index].Name;
    }
    public void NonExistItemAsset()
    {
        icon.sprite = nonExistItemIcon;
        textMesh.text = "ŠŽ‚µ‚Ä‚¢‚Ü‚¹‚ñB";
    }
    public void InjectEmptyAsset(string name, int id)
    {
        viewInfos.Add(new ViewInfo(nonExistItemIcon, name, id));
    }

    public void Click_Next()
    {
        if (viewInfos.Count == 0)
        {
            return;
        }

        index++;
        if (index > viewInfos.Count - 1)
        {
            index = 0;
        }
        OnDressUp?.Invoke(viewInfos[index].ID, viewInfos[index].Name);
        icon.sprite = viewInfos[index].Icon;
        textMesh.text = viewInfos[index].Name;
    }
    public void Click_Previous()
    {
        if (viewInfos.Count == 0)
        {
            return;
        }

        index--;
        if (index < 0)
        {
            index = viewInfos.Count - 1;
        }
        OnDressUp?.Invoke(viewInfos[index].ID, viewInfos[index].Name);
        icon.sprite = viewInfos[index].Icon;
        textMesh.text = viewInfos[index].Name;
    }
    public void DefaultDressUp()
    {
        if (viewInfos.Count != 0)
        {
            OnDressUp?.Invoke(viewInfos[0].ID, viewInfos[0].Name);
        }
    }
    private void OnDestroy()
    {
        OnDressUp = null;
    }
}

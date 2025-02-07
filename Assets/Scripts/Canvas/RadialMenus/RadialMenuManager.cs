using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuManager : MonoBehaviour, IDressUpEventVendor
{
    [SerializeField]
    private List<RadialMenu> radialMenus = new List<RadialMenu>();
    [SerializeField]
    private RadialInput radialInput;

    private void Awake()
    {
        for (int i = 0; i < radialMenus.Count; i++)
        {
            int angleIndex = i + 1;
            radialMenus[i].AngleRange = new AngleRange((72 * angleIndex) - 72, (72 * angleIndex));
        }

        radialInput.InjectRadialMenu(radialMenus.ToArray());
    }
    public void InjectItemAssets(List<ItemAsset> itemAssets)
    {
        if(itemAssets.Count > radialMenus.Count)
        {
            
        }

        for(int i = 0; i < itemAssets.Count; i++)
        {
            radialMenus[i].Inject(itemAssets[i].ID, itemAssets[i].Name);
            radialMenus[i].InjectSprite(itemAssets[i].ItemIcon);
        }
    }

    private class DressUpEventHelper : IDisposable
    {
        public Action<int, string> action;

        private DressUpEventHelper() { }
        public DressUpEventHelper(Action<int, string> action)
        {
            this.action = action;
        }
        public void Dispose()
        {
            action = null;
        }
    }
    public IDisposable SubscribeDressUpEvent(Action<int, string> action)
    {
        DressUpEventHelper dressUpEventHelper = new DressUpEventHelper(action);
        foreach (RadialMenu radialMenu in radialMenus)
        {
            radialMenu.OnSelect += dressUpEventHelper.action;
        }
        return dressUpEventHelper;
    }
}

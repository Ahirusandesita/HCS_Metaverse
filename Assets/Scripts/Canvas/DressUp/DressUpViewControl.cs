using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public interface IDressUpEventVendor
{
    IDisposable SubscribeDressUpEvent(Action<int> action);
}
public class DressUpInformation
{
    public readonly int Index;
    public readonly int ID;

    public DressUpInformation(int index, int id)
    {
        this.Index = index;
        this.ID = id;
    }
}
public class DressUpViewControl : MonoBehaviour, IDressUpEventVendor
{
    [SerializeField]
    private ItemBundleAsset allItemBundle;
    [SerializeField]
    private DressUpViewFrame dressUpViewFrame;
    [SerializeField]
    private Transform content;
    private List<DressUpInformation> dressUpInformations = new List<DressUpInformation>();
    private int maxIndex;
    private List<DressUpViewFrame> dressUpViewFrames = new List<DressUpViewFrame>();

    private class DressUpEventHelper : IDisposable
    {
        public Action<int> action;

        private DressUpEventHelper() { }
        public DressUpEventHelper(Action<int> action)
        {
            this.action = action;
        }
        public void Dispose()
        {
            action = null;
        }
    }

    public void InjectDressUpInformation(List<DressUpInformation> dressUpInformations)
    {
        this.dressUpInformations = dressUpInformations;

        for (int i = 0; i < dressUpInformations.Count; i++)
        {
            if (maxIndex < dressUpInformations[i].Index)
            {
                maxIndex = dressUpInformations[i].Index;
            }
        }

        SpawnFrame();
    }
    public IDisposable SubscribeDressUpEvent(Action<int> action)
    {
        DressUpEventHelper dressUpEventHelper = new DressUpEventHelper(action);
        foreach (DressUpViewFrame dressUpViewFrame in dressUpViewFrames)
        {
            dressUpViewFrame.OnDressUp += dressUpEventHelper.action;
        }
        return dressUpEventHelper;
    }

    private void SpawnFrame()
    {
        int index = 0;
        while (index <= maxIndex)
        {
            bool existItemAsset = false;

            List<ItemAsset> itemAssets = new List<ItemAsset>();
            foreach (DressUpInformation dressUpInformation in dressUpInformations)
            {
                if (dressUpInformation.Index == index)
                {
                    itemAssets.Add(allItemBundle.GetItemAssetByID(dressUpInformation.ID));
                    existItemAsset = true;
                }
            }

            if (itemAssets.Count > 0)
            {
                DressUpViewFrame instance = Instantiate(dressUpViewFrame, content, false);
                instance.InjectItemAsset(itemAssets);
                dressUpViewFrames.Add(instance);
            }

            if (!existItemAsset)
            {
                DressUpViewFrame instance = Instantiate(dressUpViewFrame, content, false);
                instance.NonExistItemAsset();
            }

            index++;
        }
    }
}

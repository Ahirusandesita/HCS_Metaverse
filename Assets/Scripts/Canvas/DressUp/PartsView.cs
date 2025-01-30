using Layer_lab._3D_Casual_Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsView : MonoBehaviour
{
    [SerializeField]
    private ItemBundleAsset allItemBundleAsset;
    private List<DressUpInformation> dressUpInformation = new List<DressUpInformation>();
    private DressUpViewControl dressUpViewControl;
    public void InjectItemAssetID(List<int> IDs)
    {
        dressUpViewControl = this.GetComponent<DressUpViewControl>();

        Action<PartsType, int, string> action = (partsType, id, partsTypeName) =>
        {
            if (allItemBundleAsset.GetItemAssetByID(id).Name.Contains(partsTypeName))
            {
                dressUpInformation.Add(new DressUpInformation((int)partsType, id));
            }
        };
        foreach (int id in IDs)
        {
            action(PartsType.Body, id, "Body");
            action(PartsType.Hair, id, "Hair");
            action(PartsType.Face, id, "Face");
            action(PartsType.Headgear, id, "HeadGear");
            action(PartsType.Top, id, "Top");
            action(PartsType.Bottom, id, "Bottom");
            action(PartsType.Eyewear, id, "Eyewear");
            action(PartsType.Bag, id, "Bag");
            action(PartsType.Shoes, id, "Shoes");
            action(PartsType.Glove, id, "Glove");
        }

        dressUpViewControl.InjectDressUpInformation(dressUpInformation);
    }
}

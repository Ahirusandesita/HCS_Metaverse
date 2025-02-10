using Layer_lab._3D_Casual_Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsView : DressUpViewBase
{
    protected override List<DressUpInformation> CreateDressUpInformation(in List<int> IDs)
    {
        List<DressUpInformation> dressUpInformations = new List<DressUpInformation>();

        Action<PartsType, int, string> action = (partsType, id, partsTypeName) =>
        {
            if (id == -1)
            {
                dressUpInformations.Add(new DressUpInformation((int)partsType, id, partsTypeName));
                return;
            }

            if (AllItemBundleAsset.GetItemAssetByID(id).Name.Contains(partsTypeName))
            {
                dressUpInformations.Add(new DressUpInformation((int)partsType, id, partsTypeName));
            }
        };
        action(PartsType.Headgear, -1, "Headgear");
        action(PartsType.Bag, -1, "Bag");
        action(PartsType.Eyewear, -1, "Eyewear");
        action(PartsType.Glove, -1, "Glove");
        action(PartsType.Shoes, -1, "Shoes");

        foreach (int id in IDs)
        {
            action(PartsType.Body, id, "Body");
            action(PartsType.Hair, id, "Hair");
            action(PartsType.Face, id, "Face");
            action(PartsType.Headgear, id, "Headgear");
            action(PartsType.Top, id, "Top");
            action(PartsType.Bottom, id, "Bottom");
            action(PartsType.Eyewear, id, "Eyewear");
            action(PartsType.Bag, id, "Bag");
            action(PartsType.Shoes, id, "Shoes");
            action(PartsType.Glove, id, "Glove");
        }


        return dressUpInformations;
    }
}

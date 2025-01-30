using Layer_lab._3D_Casual_Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeOfClothes : MonoBehaviour
{
    [SerializeField]
    private ItemBundleAsset itemBundleAsset;
    [SerializeField]
    private ButtonParts ButtonParts;
    public void InjectItemAsset(int[] ids)
    {
        Action<ItemAsset, string, List<GameObject>> action = (itemAsset, partsName, parts) =>
        {
            if (itemAsset.Name.Contains(partsName))
            {
                parts.Add(itemAsset.Prefab);
            }
        };

        foreach (int id in ids)
        {
            ItemAsset itemAsset = itemBundleAsset.GetItemAssetByID(id);

            action(itemAsset, "Body", CharacterControl.Instance.CharacterBase.PartsBody);
            action(itemAsset, "Hair", CharacterControl.Instance.CharacterBase.PartsHair);
            action(itemAsset, "Face", CharacterControl.Instance.CharacterBase.PartsFace);
            action(itemAsset, "HeadGear", CharacterControl.Instance.CharacterBase.PartsHeadGear);
            action(itemAsset, "Top", CharacterControl.Instance.CharacterBase.PartsTop);
            action(itemAsset, "Bottom", CharacterControl.Instance.CharacterBase.PartsBottom);
            action(itemAsset, "Eyewear", CharacterControl.Instance.CharacterBase.PartsEyewear);
            action(itemAsset, "Bag", CharacterControl.Instance.CharacterBase.PartsBag);
            action(itemAsset, "Shoes", CharacterControl.Instance.CharacterBase.PartsShoes);
            action(itemAsset, "Glove", CharacterControl.Instance.CharacterBase.PartsGlove);
        }

        GetComponentInChildren<CharacterBase>().Initialize();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ButtonParts.OnClick_Next();
        }
    }

}

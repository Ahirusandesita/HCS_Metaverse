using Layer_lab._3D_Casual_Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DressUpViewBase : MonoBehaviour
{
    [SerializeField]
    private ItemBundleAsset allItemBundleAsset;
    protected ItemBundleAsset AllItemBundleAsset => allItemBundleAsset;
    private DressUpViewControl dressUpViewControl;
    protected void Awake()
    {
        dressUpViewControl = this.GetComponent<DressUpViewControl>();
    }
    protected abstract List<DressUpInformation> CreateDressUpInformation(in List<int> IDs);
    public void InjectItemAssetID(List<int> IDs)
    {
        List<DressUpInformation> dressUpInformations = CreateDressUpInformation(IDs);
        dressUpViewControl.InjectDressUpInformation(dressUpInformations);
    }
}

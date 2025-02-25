
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class GrantCommodityID
{
    [MenuItem("Initialize/Activity/Cook/CommodityID %h")]
    public static void Initialize()
    {
        string[] guids = CommodityAssetDatabase.Find();

        for (int i = 0; i < guids.Length; i++)
        {
            CommodityAsset commodityAsset = CommodityAssetDatabase.LoadAssetAtPathFromGuid(guids[i]);
            IGrantableCommodityID grantable = commodityAsset;
            grantable.GrantID(i);
            UnityEditor.EditorUtility.SetDirty(commodityAsset);
        }
        Debug.Log("CommodityにIDを付与しました。");
    }
}
#endif
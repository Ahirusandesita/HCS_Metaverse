using HCSMeta.Activity.Cook.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HCSMeta.Activity.Cook
{
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
            Debug.Log("Commodity‚ÉID‚ð•t—^‚µ‚Ü‚µ‚½B");
        }
    }
#endif
}
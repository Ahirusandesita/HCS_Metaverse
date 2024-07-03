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
            IGrantableCommodityID grantable = CommodityAssetDatabase.LoadAssetAtPathFromGuid(guids[i]);
            grantable.GrantID(i);
        }
        Debug.Log("Commodity‚ÉID‚ð•t—^‚µ‚Ü‚µ‚½B");
    }
}
#endif
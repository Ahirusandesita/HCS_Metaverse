using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantCommodityID : InitializeBase
{ 
    public override void Initialize()
    {
#if UNITY_EDITOR
        string[] guids = CommodityAssetDatabase.Find();
        for (int i = 0; i < guids.Length; i++)
        {
            IGrantableCommodityID grantable = CommodityAssetDatabase.LoadAssetAtPathFromGuid(guids[i]);
            grantable.GrantID(i);
        }
#endif
    }
}

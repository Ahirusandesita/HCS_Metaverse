using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantCommodityID : MonoBehaviour
{
    private void Start()
    {
        string[] guids = CommodityAssetDatabase.Find();
        for (int i = 0; i < guids.Length; i++)
        {
            IGrantableCommodityID grantable = CommodityAssetDatabase.LoadAssetAtPathFromGuid(guids[i]);
            grantable.GrantID(i);
        }
    }
}

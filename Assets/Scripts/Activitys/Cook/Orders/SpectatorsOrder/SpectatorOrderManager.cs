using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorOrderManager : MonoBehaviour
{
    private OrderAsset orderAsset;
    private OrderMenuView menuView;
    public void OrderAssetInject(OrderAsset orderAsset)
    {
        this.orderAsset = orderAsset;
        Initialize();
    }

    private void Initialize()
    {
        foreach(OrderDetailInformation item in orderAsset.OrderDetailInformations)
        {
            OrderMenuView instance = Instantiate(menuView);         
        }
    }
}

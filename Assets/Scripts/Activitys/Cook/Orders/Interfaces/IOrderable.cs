using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOrderable
{
    void Order(CommodityAsset commodityAsset, CustomerInformation customer);
    void Cancel(CustomerInformation customerInformation);
}
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ãÔçﬁ
/// </summary>
public class Ingrodients : MonoBehaviour
{
    [SerializeField]
    private IngrodientsAsset ingrodientsAsset;
    public IngrodientsAsset IngrodientsAsset => ingrodientsAsset;

    private CommodityFactory commodityFactory;

    private void Awake()
    {
        this.commodityFactory = GameObject.FindObjectOfType<CommodityFactory>();
    }

    public void Inject(CommodityFactory commodityFactory)
    {
        this.commodityFactory = commodityFactory;
    }


    public Commodity ProcessingStart(ProcessingType processingType)
    {
        Commodity commodity = commodityFactory.Generate(this, processingType);
        Instantiate(commodity, this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);
        return commodity;
    }
}

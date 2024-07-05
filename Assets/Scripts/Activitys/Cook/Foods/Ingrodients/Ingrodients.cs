using System;
using System.Collections.Generic;
using UnityEngine;
public interface IIngrodientsModerator
{
    IngrodientsAsset IngrodientsAsset { set; }
}
/// <summary>
/// ãÔçﬁ
/// </summary>
public class Ingrodients : MonoBehaviour,IIngrodientsModerator
{
    [SerializeField]
    private IngrodientsAsset ingrodientsAsset;
    public IngrodientsAsset IngrodientsAsset => ingrodientsAsset;
    IngrodientsAsset IIngrodientsModerator.IngrodientsAsset
    {
        set
        {
            ingrodientsAsset = value;
        }
    }

    private CommodityFactory commodityFactory;

    private void Awake()
    {
        this.commodityFactory = GameObject.FindObjectOfType<CommodityFactory>();
    }

    public void Inject(CommodityFactory commodityFactory)
    {
        this.commodityFactory = commodityFactory;
    }


    public Commodity ProcessingStart(ProcessingType processingType,Transform machineTransform)
    {
        Commodity commodity = commodityFactory.Generate(this, processingType);
        Instantiate(commodity, this.transform.position, this.transform.rotation).transform.parent = machineTransform;
        Destroy(this.gameObject);
        return commodity;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
public interface IIngrodientsModerator
{
    IngrodientsAsset IngrodientsAsset { set; }
}
/// <summary>
/// 具材
/// </summary>
public class Ingrodients : MonoBehaviour,IIngrodientsModerator
{
    [SerializeField]
    private IngrodientsAsset ingrodientsAsset;

    private List<IngrodientsDetailInformation> ingrodientsDetailInformations = new List<IngrodientsDetailInformation>();

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

        if(ingrodientsAsset == null)
        {
            gameObject.hideFlags = HideFlags.None;
            GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Cube.transform.parent = this.gameObject.transform;
            Cube.gameObject.hideFlags = HideFlags.None;
            Debug.LogError(Cube.transform.root);
            this.gameObject.transform.position = new Vector3(0f, 10f, 0f);
            Debug.LogError(this.gameObject.scene.name);
            Debug.LogError("miss"+"ヒエラルキー"+this.gameObject.transform.position);
            return;
        }

        foreach (IngrodientsDetailInformation ingrodientsDetailInformation in ingrodientsAsset.IngrodientsDetailInformations)
        {
            ingrodientsDetailInformations.Add(ingrodientsDetailInformation);
        }
    }


    public void Inject(CommodityFactory commodityFactory)
    {
        this.commodityFactory = commodityFactory;
    }

    public bool SubToIngrodientsDetailInformationsTimeItTakes(ProcessingType processableType, float subValue)
    {
        foreach (IngrodientsDetailInformation information in ingrodientsDetailInformations)
        {
            if (information.ProcessingType == processableType)
            {
                return information.SubToTimeItTakes(subValue);
            }
        }

        Debug.Log("ProcessingTypeが指定外");
        return default;
    }


    public Commodity ProcessingStart(ProcessingType processingType,Transform machineTransform)
    {
        Commodity commodity = commodityFactory.Generate(this, processingType);
        Instantiate(commodity, this.transform.position, this.transform.rotation)/*.transform.parent = machineTransform*/;
        Destroy(this.gameObject);
        return commodity;
    }
}

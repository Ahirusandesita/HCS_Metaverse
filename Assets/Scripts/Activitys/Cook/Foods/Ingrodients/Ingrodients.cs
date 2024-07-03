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

    private List<IngrodientsDetailInformation> workIngrodientsDetailInformations = new List<IngrodientsDetailInformation>();
    private Action processingAction;
    private float timeItTakes;

    private void Awake()
    {
        foreach(IngrodientsDetailInformation ingrodientsDetailInformation in IngrodientsAsset.IngrodientsDetailInformations)
        {
            this.workIngrodientsDetailInformations.Add(ingrodientsDetailInformation);
        }

        this.commodityFactory = GameObject.FindObjectOfType<CommodityFactory>();
    }

    public void Inject(CommodityFactory commodityFactory)
    {
        this.commodityFactory = commodityFactory;
    }

    private void Update()
    {
        processingAction?.Invoke();
    }

    public void ProcessingStart(ProcessingType processingType)
    {
        Commodity commodity = default;
        foreach(IngrodientsDetailInformation ingrodientsDetailInformation in workIngrodientsDetailInformations)
        {
            if(ingrodientsDetailInformation.ProcessingType == processingType)
            {
                timeItTakes = ingrodientsDetailInformation.TimeItTakes;

                processingAction += () =>
                {
                    timeItTakes -= Time.deltaTime;

                    if(timeItTakes <= 0f)
                    {
                        //â¡çHÇ∑ÇÈã@äBÇ©ÇÁÇ≈Ç‡Ç¢Ç¢
                        commodity =Å@commodityFactory.Generate(this, processingType);
                        processingAction = null;

                        Instantiate(commodity, this.transform.position, this.transform.rotation);
                        Debug.Log("â¡çHäÆóπ");
                        Destroy(this.gameObject);
                    }
                };
            }
        }
    }
    public void ProcessingInterruption()
    {
        processingAction = null;
    }
}

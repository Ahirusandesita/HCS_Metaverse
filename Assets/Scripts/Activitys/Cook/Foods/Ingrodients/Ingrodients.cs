using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���
/// </summary>
public class Ingrodients : MonoBehaviour
{
    [SerializeField]
    private IngrodientsAsset ingrodientsAsset;
    public IngrodientsAsset IngrodientsAsset => ingrodientsAsset;

    private CommodityFactory processedGoodsFactory;

    private List<IngrodientsDetailInformation> workIngrodientsDetailInformations = new List<IngrodientsDetailInformation>();
    private Action processingAction;
    private float timeItTakes;

    private void Awake()
    {
        foreach(IngrodientsDetailInformation ingrodientsDetailInformation in IngrodientsAsset.IngrodientsDetailInformations)
        {
            this.workIngrodientsDetailInformations.Add(ingrodientsDetailInformation);
        }
    }

    public void Inject(CommodityFactory processedGoodsFactory)
    {
        this.processedGoodsFactory = processedGoodsFactory;
    }

    private void Update()
    {
        processingAction?.Invoke();
    }

    public void ProcessingStart(ProcessingType processingType)
    {
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
                        //���H����@�B����ł�����
                        processedGoodsFactory.Generate(this, processingType);
                        Debug.Log("���H����");
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

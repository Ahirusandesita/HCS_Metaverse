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

    private ProcessedGoodsFactory processedGoodsFactory;

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

    public void Inject(ProcessedGoodsFactory processedGoodsFactory)
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
                        //â¡çHÇ∑ÇÈã@äBÇ©ÇÁÇ≈Ç‡Ç¢Ç¢
                        processedGoodsFactory.Generate(this, processingType);
                        Debug.Log("â¡çHäÆóπ");
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

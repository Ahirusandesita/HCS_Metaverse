using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMachine : Machine
{
    private Action processingAction;

    private void Update()
    {
        processingAction?.Invoke();
    }
    public override void StartProcessed()
    {
        foreach (IngrodientsDetailInformation ingrodientsDetailInformation in ingrodients.IngrodientsAsset.IngrodientsDetailInformations)
        {
            if (ingrodientsDetailInformation.ProcessingType == ProcessingType)
            {
                timeItTakes = ingrodientsDetailInformation.TimeItTakes;

                processingAction += () =>
                {
                    timeItTakes -= Time.deltaTime;

                    if (timeItTakes <= 0f)
                    {
                        ingrodients.ProcessingStart(ProcessingType,this.transform);
                        processingAction = null;
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

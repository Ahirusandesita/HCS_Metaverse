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
    public override void StartProcessed(IngrodientsDetailInformation ingrodientsDetailInformation)
    {
        timeItTakes = ingrodientsDetailInformation.TimeItTakes;

        processingAction += () =>
        {
            timeItTakes -= Time.deltaTime;

            if (timeItTakes <= 0f)
            {
                ingrodients.ProcessingStart(ProcessingType, this.transform);
                processingAction = null;
                Debug.Log("‰ÁHŠ®—¹");
            }
        };


    }
    public void ProcessingInterruption()
    {
        processingAction = null;
    }
}

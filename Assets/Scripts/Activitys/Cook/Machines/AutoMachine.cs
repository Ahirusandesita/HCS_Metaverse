using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMachine : Machine
{
    private bool canProcessing = false;
    public bool CanProcessing
    {
        get
        {
            return canProcessing;
        }
        set
        {
            canProcessing = value;
        }
    }

    public bool isGrab = false;
    public bool IsGrab
    {
        get
        {
            return isGrab;
        }
        set
        {
            isGrab = value;
        }
    }

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
            if (!canProcessing)
            {
                return;
            }
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

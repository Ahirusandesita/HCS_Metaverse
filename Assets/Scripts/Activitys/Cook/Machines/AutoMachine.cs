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
        //timeItTakes = ingrodientsDetailInformation.TimeItTakes;

        processingAction += () =>
        {
            if (!canProcessing)
            {
                return;
            }

            if (ingrodients.SubToIngrodientsDetailInformationsTimeItTakes(ProcessingType, Time.deltaTime))
            {
                ingrodients.ProcessingStart(ProcessingType, this.transform);
                processingAction = null;
                Debug.Log("â¡çHäÆóπ");
            }
        };


    }
    public void ProcessingInterruption()
    {
        processingAction = null;
    }
}

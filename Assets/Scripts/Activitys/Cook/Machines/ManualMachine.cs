using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualMachine : Machine, IManualProcess
{
    public virtual void ManualProcessEvent()
    {
        //bool isEndProcessing = ProcessingAction(processingType, processingValue, out Commodity createdCommodity);
    }
}

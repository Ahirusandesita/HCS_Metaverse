using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualMachine : Machine, IManualProcessing
{
    public virtual void ProcessingEvent()
    {
        //bool isEndProcessing = ProcessingAction(processingType, processingValue, out Commodity createdCommodity);
    }
}

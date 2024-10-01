using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualMachine : Machine, IManualProcessing
{
    public virtual void ProcessingEvent(ProcessingType processingType, float processingValue)
    {
        bool isEndProcessing = ProcessingAction(processingType, processingValue, out Commodity createdCommodity);
    }
}

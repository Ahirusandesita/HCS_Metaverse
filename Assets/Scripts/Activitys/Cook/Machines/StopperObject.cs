using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class StopperObject : NetworkBehaviour/*, IManualProcessing*/
{
    public virtual void ProcessingEvent()
    {
        return;
    }
}

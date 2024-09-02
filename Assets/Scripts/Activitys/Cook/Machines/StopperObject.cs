using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class StopperObject : NetworkBehaviour, IKnifeHitEvent
{
    public virtual void KnifeHitEvent()
    {
        return;
    }
}

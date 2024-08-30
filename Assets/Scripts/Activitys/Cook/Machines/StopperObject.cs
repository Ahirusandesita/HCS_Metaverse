using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopperObject : MonoBehaviour, IKnifeHitEvent
{
    public virtual void KnifeHitEvent()
    {
        return;
    }
}

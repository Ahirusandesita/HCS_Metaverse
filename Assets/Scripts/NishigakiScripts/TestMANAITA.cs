using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMANAITA : MonoBehaviour, IKnifeHitEvent
{
    [SerializeField, Tooltip("きるよん！！！！！！！")]
    private Ingrodients _cutObject = default;

    void IKnifeHitEvent.KnifeHitEvent()
    {
        _cutObject.ProcessingStart(ProcessingType.Cut,this.transform);
    }
}

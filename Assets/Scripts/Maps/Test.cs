using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public void Push()
    {
        RelatedParties.Instance.ActivityRelatedPartiesState = ActivityRelatedPartiesState.Spectators;
    }
}

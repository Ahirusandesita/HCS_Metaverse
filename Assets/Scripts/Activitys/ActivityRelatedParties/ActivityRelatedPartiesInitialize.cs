using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChangeRelatedPartiesEventArgs : System.EventArgs
{
    public readonly ActivityRelatedPartiesState ActivityRelatedPartiesState;
    public ChangeRelatedPartiesEventArgs(ActivityRelatedPartiesState activityRelatedPartiesState)
    {
        ActivityRelatedPartiesState = activityRelatedPartiesState;
    }
}
public delegate void ChangeRelatedRartiesHandler(ChangeRelatedPartiesEventArgs changeRelatedPartiesEventArgs);
public class ActivityRelatedPartiesInitialize : MonoBehaviour
{

}

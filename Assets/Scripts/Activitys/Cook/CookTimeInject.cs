using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookTimeInject : MonoBehaviour
{
    [SerializeField]
    private ActivityProgressManagement activityProgressManagement;
    private enum TimeType
    {
        Ready,
        Main
    }
    [SerializeField]
    private TimeType timeType;

    private void Start()
    {
        switch (timeType)
        {
            case TimeType.Ready:
                activityProgressManagement.InjectTimeManager_Ready(this.GetComponent<ITimeManager>());
                break;
            case TimeType.Main:
                activityProgressManagement.InjectTimeManager_Activity(this.GetComponent<ITimeManager>());
                break;
        }
    }
}

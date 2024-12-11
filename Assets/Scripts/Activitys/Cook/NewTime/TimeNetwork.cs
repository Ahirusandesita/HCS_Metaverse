using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public enum ActivitiState
{
    Ready,
    Start,
    End
}
public class TimeNetwork : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(Count))]
    public int Time { get; set; }
    [Networked]
    public ActivitiState ActivitiState { get; set; }

    public event Action<int> OnTime;
    public event Action OnFinish;

    [Networked, OnChangedRender(nameof(SetTime))]
    public float StartTime { get; set; }
    public float countDownTime_s;
    private float lastTime_s;

    private bool canInvoke = true;
    private bool canProsess = false;
    private void SetTime()
    {
        countDownTime_s = StartTime;
        canProsess = true;
        Time = (int)countDownTime_s;
        lastTime_s = countDownTime_s;
    }
    private void Count()
    {
        OnTime?.Invoke(Time);
    }
    private void Update()
    {
        Debug.LogError($"{Time}  {canInvoke}  {canProsess}");
        if (Time <= 0 && canInvoke && canProsess)
        {
            Debug.LogError("Finish");
            OnFinish?.Invoke();
            OnTime = null;
            canInvoke = false;
        }

        if (!this.GetComponent<NetworkObject>().HasStateAuthority)
        {
            return;
        }

        countDownTime_s -= UnityEngine.Time.deltaTime;

        if (lastTime_s - countDownTime_s >= 1f && Time > 0)
        {
            lastTime_s = countDownTime_s;
            Time = (int)countDownTime_s;
            Debug.LogError("CountDown");
        }

    }
}

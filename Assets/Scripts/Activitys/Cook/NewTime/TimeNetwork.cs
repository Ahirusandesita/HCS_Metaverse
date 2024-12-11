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
public class TimeNetwork : NetworkBehaviour,IAfterSpawned
{
    public bool IsSpawned = false;
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
    [Networked]
    private bool CanProsess { get; set; }
    private void SetTime()
    {
        countDownTime_s = StartTime;
        CanProsess = true;
        Time = (int)countDownTime_s;
        lastTime_s = countDownTime_s;
    }
    private void Count()
    {
        OnTime?.Invoke(Time);
    }
    private void Update()
    {
        Debug.LogError(CanProsess);
        if (Time <= 0 && canInvoke && CanProsess)
        {
            OnFinish?.Invoke();
            OnTime = null;
            canInvoke = false;
        }

        if (!this.GetComponent<NetworkObject>().HasStateAuthority)
        {
            return;
        }

        countDownTime_s -= UnityEngine.Time.deltaTime;

        if (lastTime_s - countDownTime_s >= 1f && canInvoke)
        {
            lastTime_s = countDownTime_s;
            Time = (int)countDownTime_s;
        }

    }

    public void AfterSpawned()
    {
        if (CanProsess)
        {
            SetTime();
        }
    }
}

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
public class TimeNetwork : NetworkBehaviour, IStateAuthorityChanged
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

    private bool isCountStart = false;
    private bool canInvoke = false;
    private void SetTime()
    {
        countDownTime_s = StartTime;
        Time = (int)countDownTime_s;
        lastTime_s = countDownTime_s;
        canInvoke = true;
    }
    private void Count()
    {
        OnTime?.Invoke(Time);
        isCountStart = true;
    }
    private void Update()
    {
        if (Time <= 0 && isCountStart && canInvoke)
        {
            OnFinish?.Invoke();
            OnFinish = null;
            OnTime = null;
            canInvoke = false;
            isCountStart = false;
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

    public void StateAuthorityChanged()
    {
        countDownTime_s = Time;
        lastTime_s = countDownTime_s;
        canInvoke = true;
    }
}

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
    private bool canProsess = false;
    private void SetTime()
    {
        countDownTime_s = StartTime;
        canProsess = true;
        Time = (int)countDownTime_s;
        lastTime_s = countDownTime_s;
    }
    public void SetStartTime(float time)
    {
        StartTime = time;
        RPC_SetTime();
    }
    private void Count()
    {
        OnTime?.Invoke(Time);
    }
    private void Update()
    {
        Debug.LogError(canProsess);
        if (Time <= 0 && canInvoke && canProsess)
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
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    private void RPC_SetTime()
    {
        Debug.LogError("RPC");
        SetTime();
    }

    public void AfterSpawned()
    {
        IsSpawned = true;
    }
}

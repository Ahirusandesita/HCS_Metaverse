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
    public event Action OnMasterFinish;

    [Networked, OnChangedRender(nameof(SetTime))]
    public float StartTime { get; set; }
    public float countDownTime_s;
    private float lastTime_s;

    private bool isCountStart = false;
    private bool canInvoke = false;
    private bool isFirstInvoke = true;
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
        Debug.LogError($"{Time}  iscountstart{isCountStart}   caninvoke{canInvoke}   isfirstinvoke{isFirstInvoke}");
        if (Time <= 0 && canInvoke && isFirstInvoke)
        {
            OnMasterFinish?.Invoke();
            OnMasterFinish = null;
            OnFinish?.Invoke();
            OnFinish = null;
            OnTime = null;
            canInvoke = false;
            isCountStart = false;
            isFirstInvoke = false;
        }
        if(Time <= 0 && isFirstInvoke)
        {
            Debug.LogError("ƒƒ“ƒo[‚ÌFinish");
            OnFinish?.Invoke();
            OnFinish = null;
            OnMasterFinish = null;
            OnTime = null;
            canInvoke = false;
            isCountStart = false;
            isFirstInvoke = false;
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookTimeManager : MonoBehaviour, ITimeManager
{
    public event Action<int> OnTime;

    [SerializeField]
    private float countDownTime_s;
    private float lastTime_s;

    private bool isCountDownEnd = false;
    private bool isCountDownBegins = false;

    private void Update()
    {
        if (!isCountDownBegins)
        {
            return;
        }

        countDownTime_s -= Time.deltaTime;
        if (countDownTime_s <= 0f)
        {
            isCountDownEnd = true;
        }

        if (lastTime_s - countDownTime_s >= 1f)
        {
            lastTime_s = countDownTime_s;
            OnTime?.Invoke((int)lastTime_s);
        }
    }

    public void CountDownBegins()
    {
        isCountDownBegins = true;
        lastTime_s = countDownTime_s;
    }

    public bool IsCountdownEnds()
    {
        return isCountDownEnd;
    }

    public void Dispose()
    {
        isCountDownBegins = false;
        OnTime = null;
    }
}

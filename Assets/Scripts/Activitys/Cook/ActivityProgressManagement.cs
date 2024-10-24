using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
public interface ITimeManager
{
    void CountDownBegins();
    bool IsCountdownEnds();

    void Dispose();
}

public class ActivityProgressManagement : MonoBehaviour
{
    /// <summary>
    /// Activity���J�n���鏀������������Ɣ��s�����
    /// </summary>
    public event Action OnReady;
    /// <summary>
    /// Activity���J�n����Ƃ��ɔ��s�����
    /// </summary>
    public event Action OnStart;
    /// <summary>
    /// Activity���I������Ƃ��ɔ��s�����
    /// </summary>
    public event Action OnFinish;

    private ITimeManager timeManager_ready;
    private ITimeManager timeManager_main;
    private void Awake()
    {
        GateOfFusion.Instance.OnActivityConnected += async () =>
        {
            await UniTask.Delay(1000);
            OnReady?.Invoke();

            timeManager_ready.CountDownBegins();
            await UniTask.WaitUntil(() => timeManager_ready.IsCountdownEnds());
            OnStart?.Invoke();
            timeManager_ready.Dispose();
        };

        OnStart += async () =>
        {
            timeManager_main.CountDownBegins();
            await UniTask.WaitUntil(() => timeManager_main.IsCountdownEnds());
            OnFinish?.Invoke();
            timeManager_main.Dispose();
        };
    }


    public void InjectTimeManager_Ready(ITimeManager timeManager)
    {
        this.timeManager_ready = timeManager;
    }
    public void InjectTimeManager_Activity(ITimeManager timeManager)
    {
        this.timeManager_main = timeManager;
    }
}
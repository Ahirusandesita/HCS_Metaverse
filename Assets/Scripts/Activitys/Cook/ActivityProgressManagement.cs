using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Linq;

public interface ITimeManager
{
    void CountDownBegins();
    bool IsCountdownEnds();

    void Dispose();
}

public class ActivityProgressManagement : MonoBehaviour
{
    /// <summary>
    /// Activityを開始する準備が完了すると発行される
    /// </summary>
    public event Action OnReady;
    /// <summary>
    /// Activityを開始するときに発行される
    /// </summary>
    public event Action OnStart;
    /// <summary>
    /// Activityを終了するときに発行される
    /// </summary>
    public event WaitWithHandler OnFinish;
    public delegate UniTask WaitWithHandler();

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
            timeManager_main.Dispose();
            ActivityFinish().Forget();
        };
    }

    private async UniTaskVoid ActivityFinish()
    {
        await UniTask.WhenAll(
            OnFinish?.GetInvocationList()
               .OfType<WaitWithHandler>()
               .Select(async (OnAysncEvent) => await OnAysncEvent.Invoke()));

        GateOfFusion.Instance.ReturnMainRoom();
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
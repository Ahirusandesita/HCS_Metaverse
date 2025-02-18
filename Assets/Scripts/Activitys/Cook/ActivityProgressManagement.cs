using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Linq;
using Fusion;

public interface ITimeManager
{
    void CountDownBegins();
    bool IsCountdownEnds();

    void Dispose();
}

public interface INetworkTimeInjectable
{
    void Inject(TimeNetwork timeNetwork);
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
    public event Action OnStart_All;
    /// <summary>
    /// Activityを終了するときに発行される
    /// </summary>
    public event WaitWithHandler OnWaitFinish;
    public event Action OnFinish;
    public delegate UniTask WaitWithHandler();
    [SerializeField]
    private TimeNetwork timeNetwork;
    [SerializeField]
    private ActivityManagementRPC activityManagementRPC;
    [SerializeField, InterfaceType(typeof(INetworkTimeInjectable))]
    private List<UnityEngine.Object> ReadyTimeInjectable = new List<UnityEngine.Object>();
    [SerializeField, InterfaceType(typeof(INetworkTimeInjectable))]
    private List<UnityEngine.Object> MainTimeInjectable = new List<UnityEngine.Object>();
    private List<INetworkTimeInjectable> readyTimeInjectable => ReadyTimeInjectable.OfType<INetworkTimeInjectable>().ToList();
    private List<INetworkTimeInjectable> mainTimeInjectable => MainTimeInjectable.OfType<INetworkTimeInjectable>().ToList();

    TimeNetwork readyTimeInstance;
    TimeNetwork mainTimeInstance;
    private ActivityManagementRPC rpcInstance;
    public TimeNetwork ReadyTimeInstance => readyTimeInstance;
    public TimeNetwork MainTimeInstance => mainTimeInstance;
    [SerializeField]
    private AllSpawn test;

    private void Awake()
    {
        //leaderのみ
        GateOfFusion.Instance.OnActivityConnected += async () =>
        {
            Debug.LogError("Awake実行");
            if (!GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
            {
                return;
            }
            rpcInstance = await GateOfFusion.Instance.SpawnAsync(activityManagementRPC);
            readyTimeInstance = await GateOfFusion.Instance.SpawnAsync(timeNetwork);
            AllSpawn allSpawn = await GateOfFusion.Instance.SpawnAsync(test);
            foreach (INetworkTimeInjectable networkTimeInjectable in readyTimeInjectable)
            {
                networkTimeInjectable.Inject(readyTimeInstance);
            }
            readyTimeInstance.OnMasterFinish += () =>
            {
                ActivityStart();
            };
            await allSpawn.Async();
            GateOfFusion.Instance.Despawn(allSpawn);
            rpcInstance.AllSpawn = test;
            rpcInstance.RPC_RPCInstanceInject();
            readyTimeInstance.StartTime = 3;
            rpcInstance.RPC_ReadyTimeInject(readyTimeInstance.GetComponent<NetworkObject>());
            await UniTask.Delay(1000);
            OnReady?.Invoke();
        };
    }



    public async void ActivityStart()
    {
        Debug.LogError("1度のみ");
        OnStart?.Invoke();
        OnStart_All?.Invoke();
        mainTimeInstance = await GateOfFusion.Instance.SpawnAsync(timeNetwork);
        AllSpawn allSpawn = await GateOfFusion.Instance.SpawnAsync(test);
        await allSpawn.Async();
        GateOfFusion.Instance.Despawn(allSpawn);
        mainTimeInstance.StartTime = 180f;
        mainTimeInstance.OnMasterFinish += () =>
        {
            ActivityFinish().Forget();
        };
        foreach (INetworkTimeInjectable networkTimeInjectable in mainTimeInjectable)
        {
            networkTimeInjectable.Inject(mainTimeInstance);
        }
        rpcInstance.RPC_MainTimeInject(mainTimeInstance.GetComponent<NetworkObject>());
    }

    public async UniTaskVoid ActivityFinish()
    {
        OnFinish?.Invoke();
        await UniTask.WhenAll(
            OnWaitFinish?.GetInvocationList()
               .OfType<WaitWithHandler>()
               .Select(async (OnAysncEvent) => await OnAysncEvent.Invoke()));
        WebAPIRequester webAPIRequester = new WebAPIRequester();
        //webAPIRequester.PostScore()

        GateOfFusion.Instance.ReturnMainCity();
    }

    public void RPC_ReadyInjectable(TimeNetwork timeNetwork)
    {
        Debug.LogError("よばれてね");
        if (readyTimeInstance != null)
        {
            return;
        }
        OnReady?.Invoke();
        readyTimeInstance = timeNetwork;
        readyTimeInstance.OnMasterFinish += () =>
        {
            OnStart?.Invoke();
        };
        readyTimeInstance.OnFinish += () =>
        {
            OnStart_All?.Invoke();
        };

        foreach (INetworkTimeInjectable networkTimeInjectable in readyTimeInjectable)
        {
            networkTimeInjectable.Inject(timeNetwork);
        }
    }
    public void RPC_MainInjectable(TimeNetwork timeNetwork)
    {
        if (mainTimeInstance != null)
        {
            return;
        }
        mainTimeInstance = timeNetwork;
        mainTimeInstance.OnFinish += () =>
        {
            ActivityFinish().Forget();
        };


        foreach (INetworkTimeInjectable networkTimeInjectable in mainTimeInjectable)
        {
            networkTimeInjectable.Inject(timeNetwork);
        }
    }
    public void RPC_RPCInstance(ActivityManagementRPC rpcInstance)
    {
        this.rpcInstance = rpcInstance;
    }

    public void RPC_Joined(PlayerRef playerRef)
    {
        rpcInstance.RPC_NetworkTimeInject(playerRef, readyTimeInstance.GetComponent<NetworkObject>(), mainTimeInstance.GetComponent<NetworkObject>(),rpcInstance.GetComponent<NetworkObject>());
    }
}
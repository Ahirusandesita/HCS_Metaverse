using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IceMilkTea.StateMachine;
using HCSMeta.Function;

namespace HCSMeta.Activity.Fishing
{
    public partial class FishingManager
    {
        private class Swim : ImtStateMachine<FishingManager, EventID>.State
        {
            private SyncWaiter syncWaiter = default;
            private float waitTime = default;

            protected internal override void Enter()
            {
                syncWaiter ??= new SyncWaiter();

                GetType().Name.Print("red");
                // 魚プールからランダムで選出
                Context.LotteryForFish();

                waitTime = Random.Range(Context.minTimeToHit, Context.maxTimeToHit);
            }

            protected internal override void Update()
            {
                bool result = syncWaiter.WaitSecounds(waitTime);
                if (result)
                {
                    syncWaiter.Reset();
                    stateMachine.SendEvent(EventID.Hit);
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IceMilkTea.StateMachine;

public partial class FishingManager
{
    private class Tired : ImtStateMachine<FishingManager, EventID>.State
    {
        protected internal override void Enter()
        {
            XDebug.Log(GetType().Name, "red");
        }
    }
}
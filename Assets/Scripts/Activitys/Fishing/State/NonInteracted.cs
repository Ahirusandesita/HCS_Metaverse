using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IceMilkTea.StateMachine;

public partial class FishingManager
{
    private class NonInteracted : ImtStateMachine<FishingManager, EventID>.State
    {
        protected internal override void Enter()
        {
            XDebug.Log(GetType().Name, "red");
        }

        protected internal override void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                stateMachine.SendEvent(EventID.Start);
            }
        }
    }
}

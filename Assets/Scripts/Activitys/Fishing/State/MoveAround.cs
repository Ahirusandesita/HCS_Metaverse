using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IceMilkTea.StateMachine;

public partial class FishingManager
{
    private class MoveAround : ImtStateMachine<FishingManager, EventID>.State
    {
        protected internal override void Enter()
        {
            GetType().Name.Print("red");
        }
    }
}
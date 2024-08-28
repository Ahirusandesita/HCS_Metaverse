using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
public interface IInjectPracticableRPCEvent
{
    void Inject(IPracticableRPCEvent practicableRPCEvent);
}
public interface IAction : IInjectPracticableRPCEvent
{
    void Action();
}
public interface IAction<T> : IInjectPracticableRPCEvent
{
    void Action(T t);
}


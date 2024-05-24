using System;
using UnityEngine;
public class FixedEventArgs : EventArgs
{
    public bool IsFixed;
}
public interface ICanvasFixable
{
    void Fixed(bool isFixed);
}

public delegate void CanvasFixedHandler(FixedEventArgs eventArgs);
public interface ICanvasFixedHandler
{
    public event CanvasFixedHandler OnFixed;
}

public class CanvasFixed : MonoBehaviour, ICanvasFixedHandler
{
    public event CanvasFixedHandler OnFixed;

    public void Fixed(bool isFixed)
    {
        FixedEventArgs fixedEventArgs = new FixedEventArgs();
        fixedEventArgs.IsFixed = isFixed;
        OnFixed?.Invoke(fixedEventArgs);
    }
}

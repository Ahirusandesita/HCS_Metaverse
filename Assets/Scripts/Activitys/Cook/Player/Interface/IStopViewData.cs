using UnityEngine;

public interface IStopViewData
{
    public Transform GetVisualObjectTransform { get; }

    public HandType GetDetailHandType { get; }
}

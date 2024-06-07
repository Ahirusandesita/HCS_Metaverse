using UnityEngine;

public interface IInteraction
{
    GameObject gameObject { get; }
    ISelectedNotification SelectedNotification { get; }
    void Open();
    void Close();
}

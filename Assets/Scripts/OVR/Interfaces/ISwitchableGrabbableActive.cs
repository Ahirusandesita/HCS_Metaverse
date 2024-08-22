using UnityEngine;
public interface ISwitchableGrabbableActive
{
    void Active();
    void Inactive();
    GameObject gameObject { get; }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivatableDistance
{
    float ActiveDistance { get; }
    void Active();
    void Passive();

    GameObject gameObject { get; }
}

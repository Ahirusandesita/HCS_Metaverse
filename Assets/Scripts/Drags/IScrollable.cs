using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScrollable
{
    void Scroll(Vector2 move);
}

public interface IVerticalOnlyScrollable : IScrollable
{
    new void Scroll(Vector2 move);
}

public interface IBesideOnlyScrollable : IScrollable
{
    new void Scroll(Vector2 move);
}

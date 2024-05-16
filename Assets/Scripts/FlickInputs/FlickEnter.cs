using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlickEnter : FlickButton
{
    protected override void PointerEnter()
    {
        EnterButton();
    }
    protected override void PointerExit()
    {
        ExitButton();
    }

    protected abstract void EnterButton();
    protected abstract void ExitButton();
}

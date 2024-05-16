using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlickPush : FlickButton
{
    private bool isStartPush = true;
    protected override void PointerDown()
    {
        if (isStartPush)
        {
            isStartPush = false;
            StartPushButton();
        }

        PushButton();
    }

    protected override void PointerUp()
    {
        Debug.Log(FlickManager.IsPushScreen);
        //if (!FlickManager.IsPushScreen)
        //{
            LetButtonMe();
            isStartPush = true;
        //}
        LetButton();
    }

    protected abstract void LetButton();
    protected abstract void LetButtonMe();

    protected abstract void StartPushButton();
    protected abstract void PushButton();
}

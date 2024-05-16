using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickButtonParent : FlickPush, IFlickButtonClosure
{
    private IFlickButtonDeployment[] flickButtonDeployments;
    private bool canButtonPush = true;

    protected override void Awake()
    {
        base.Awake();

        flickButtonDeployments = this.transform.GetComponentsInChildren<IFlickButtonDeployment>(true);
        foreach(IFlickButtonDeployment deployment in flickButtonDeployments)
        {
            deployment.ButtonClose();
        }
    }
    void IFlickButtonClosure.ButtonClose()
    {
        canButtonPush = false;
    }
    void IFlickButtonClosure.ButtonOpen()
    {
        canButtonPush = true;
    }

    protected override void StartPushButton()
    {
        if (canButtonPush)
        {

            FlickManager.StartFlickInput(this);
            foreach (IFlickButtonDeployment deployment in flickButtonDeployments)
            {
                deployment.ButtonDeployment();
            }
        }
    }

    protected override void PushButton()
    {

    }

    protected override void LetButton()
    {

    }

    protected override void LetButtonMe()
    {

    }

    protected override void EndClickTouch()
    {
        if (canButtonPush)
        {
            foreach (IFlickButtonDeployment deployment in flickButtonDeployments)
            {
                deployment.ButtonClose();
            }
            FlickManager.SendChar(keyChar);
        }
    }

    protected override void EndEnterTouch()
    {
        if (canButtonPush)
        {
            foreach (IFlickButtonDeployment deployment in flickButtonDeployments)
            {
                deployment.ButtonClose();
            }
        }

        FlickManager.FinishFlickInput(this);
    }
    protected override void EndTouch()
    {
        if (canButtonPush)
        {
            foreach (IFlickButtonDeployment deployment in flickButtonDeployments)
            {
                deployment.ButtonClose();
            }
        }

        FlickManager.FinishFlickInput(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickButtonParent : FlickButton, IFlickButtonClosure
{
    private IFlickButtonDeployment[] flickButtonDeployments;

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

    }
    protected override void StartPushButton()
    {
        FlickManager.StartFlickInput(this);
        foreach (IFlickButtonDeployment deployment in flickButtonDeployments)
        {
            deployment.ButtonDeployment();
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
        foreach (IFlickButtonDeployment deployment in flickButtonDeployments)
        {
            deployment.ButtonClose();
        }
        FlickManager.SendChar(keyChar);
    }
}

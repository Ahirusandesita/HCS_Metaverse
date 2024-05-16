using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlickButtonChild : FlickEnter, IFlickButtonDeployment
{
    private bool canSendMessage = true;

    void IFlickButtonDeployment.ButtonClose()
    {
        this.GetComponent<Image>().enabled = false;
        this.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
    }

    void IFlickButtonDeployment.ButtonDeployment()
    {
        canSendMessage = true;
        this.GetComponent<Image>().enabled = true;
        this.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
    }

    protected override void EnterButton()
    {

    }

    protected override void ExitButton()
    {

    }

    protected override void EndClickTouch()
    {
    }
    protected override void EndTouch()
    {
    }

    protected override void EndEnterTouch()
    {
        if (canSendMessage)
        {
            FlickManager.SendChar(keyChar);
            canSendMessage = false;
        }
    }
}

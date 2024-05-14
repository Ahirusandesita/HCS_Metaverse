using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlickButtonChild : FlickButton, IFlickButtonDeployment
{
    void IFlickButtonDeployment.ButtonClose()
    {
        this.GetComponent<Image>().enabled = false;
        this.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
    }

    void IFlickButtonDeployment.ButtonDeployment()
    {
        this.GetComponent<Image>().enabled = true;
        this.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
    }

    protected override void LetButton()
    {

    }

    protected override void PushButton()
    {

    }

    protected override void StartPushButton()
    {

    }

    protected override void LetButtonMe()
    {
        FlickManager.SendChar(keyChar);
    }
}

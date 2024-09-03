using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailMenu : MonoBehaviour
{
    public void Deployment()
    {
        this.gameObject.SetActive(true);
        if (this.GetComponent<MenuDeploymentAnimation>())
        {
            this.GetComponent<MenuDeploymentAnimation>().AnimationStart();
        }
    }
    public void UnDeployment()
    {
        if (this.GetComponent<MenuDeploymentAnimation>())
        {
            this.GetComponent<MenuDeploymentAnimation>().UnDeployMentStart(() => { this.gameObject.SetActive(false); });
        }

    }
}

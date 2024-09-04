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

        foreach(IDetailMenuInitialize item in this.GetComponentsInChildren<IDetailMenuInitialize>())
        {
            item.Initialize();
        }
    }
    public void UnDeployment()
    {
        foreach (IDetailMenuInitialize item in this.GetComponentsInChildren<IDetailMenuInitialize>())
        {
            item.Dispose();
        }

        if (this.GetComponent<MenuDeploymentAnimation>())
        {
            this.GetComponent<MenuDeploymentAnimation>().UnDeployMentStart(() => { this.gameObject.SetActive(false); });
        }
    }
}

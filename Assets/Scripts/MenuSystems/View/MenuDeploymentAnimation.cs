using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDeploymentAnimation : MonoBehaviour
{
    private Vector3 size;
    private void Awake()
    {
        size = this.transform.localScale;
    }

    public void AnimationStart()
    {
        StartCoroutine(DeploymentAnimation());
    }
    public void UnDeployMentStart(Action action)
    {
        action();
    }
    //test
    private IEnumerator DeploymentAnimation()
    {
        Vector3 nowSize = this.transform.localScale;
        nowSize.x = 0f;
        this.transform.localScale = nowSize;
        while (true)
        {
            yield return new WaitForSeconds(0.001f);
            nowSize.x += size.x / 20f;
            this.transform.localScale = nowSize;

            if(this.transform.localScale.x > size.x)
            {
                this.transform.localScale = size;
                break;
            }
        }
    }
}

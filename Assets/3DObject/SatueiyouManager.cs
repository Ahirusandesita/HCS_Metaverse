using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatueiyouManager : MonoBehaviour
{
    SatueiyouAnimation[] a;
    private void Start()
    {
        a = FindObjectsOfType<SatueiyouAnimation>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            foreach(SatueiyouAnimation satueiyouAnimation in a)
            {
                satueiyouAnimation.Play();
            }
        }
    }

}

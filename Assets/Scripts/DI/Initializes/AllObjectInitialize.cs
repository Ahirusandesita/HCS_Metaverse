using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllObjectInitialize : InitializeBase
{
    public override void Initialize()
    {
        foreach(InitializeBase initialize in GetComponentsInChildren<InitializeBase>())
        {
            if(initialize == this)
            {
                continue;
            }
            initialize.Initialize();
        }
    }
}

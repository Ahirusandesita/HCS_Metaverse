using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : MonoBehaviour
{
    public bool IsLeader => true;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}

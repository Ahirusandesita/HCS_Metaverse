using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransSet : MonoBehaviour
{
    [SerializeField]
    private DebRota debRota;

    private void LateUpdate()
    {
        this.transform.rotation = debRota.ObjectQuaternion;
    }
}

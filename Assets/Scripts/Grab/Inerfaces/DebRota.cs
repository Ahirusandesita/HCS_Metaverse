using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebRota : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Quaternion ObjectQuaternion => this.transform.rotation;
}

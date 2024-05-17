using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebutButton : MonoBehaviour
{
    [SerializeField]
    Transform centerAye;
    [SerializeField]
    Transform button;
    public void Selected()
    {
        button.transform.position = centerAye.transform.position;
        button.transform.rotation = centerAye.transform.rotation;
        button.transform.Translate(new Vector3(0f, 0f, 0.3f));
    }
}

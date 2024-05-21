using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRInputManager : MonoBehaviour
{
    [SerializeField, InterfaceType(typeof(IItem))]
    private UnityEngine.Object IItem;
    private IItem item => IItem as IItem;


    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            
        }
    }
}

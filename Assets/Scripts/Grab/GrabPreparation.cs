using Grab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPreparation : MonoBehaviour, Grab.IGrabable, Grab.IGrabableSelect
{
    private BoxCollider myCollider;
    private Rigidbody MyRigidbody;
    private bool canGrab = true;

    private void Awake()
    {
        myCollider = this.GetComponent<BoxCollider>();
        MyRigidbody = this.GetComponent<Rigidbody>();
    }
    bool IGrabableSelect.CanGrab => canGrab;

    void IGrabable.Grab()
    {
        
    }

    void IGrabableSelect.Select()
    {
        myCollider.size *= 100f;
        //MyRigidbody.isKinematic = true;
        myCollider.isTrigger = true;
    }

    void IGrabableSelect.UnSelect()
    {
        myCollider.size /= 100f;
        //MyRigidbody.isKinematic = false;
        myCollider.isTrigger = false;
    }

    public void Grab()
    {
        canGrab = false;
    }
    public void UnGarb()
    {
        canGrab = true;
    }
}

using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IGrabbableActive
{
    void Enable();
    void Disable();
}
public class YanagiZako : MonoBehaviour,IGrabbableActive
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.transform.GetChild(0).TryGetComponent<Machine>(out Machine machine))
        {
            Disable();
            this.transform.rotation = machine.transform.rotation;
            this.transform.parent = machine.transform;
            machine.ProcessedCertification(this.GetComponent<Ingrodients>()).Processing(this.GetComponent<Ingrodients>());
        }
    }

    public void Enable()
    {
        this.GetComponent<Grabbable>().enabled = true;
    }
    public void Disable()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<Grabbable>().enabled = false;
    }
}

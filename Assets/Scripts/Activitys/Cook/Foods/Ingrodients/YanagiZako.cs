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
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Machine>(out Machine machine))
        {
            machine.ProcessedCertification(this.GetComponent<Ingrodients>()).Processing(this.GetComponent<Ingrodients>());
            Debug.Log("ABBBB");
        }
    }

    public void Enable()
    {
        this.GetComponent<Grabbable>();
    }
    public void Disable()
    {

    }
}

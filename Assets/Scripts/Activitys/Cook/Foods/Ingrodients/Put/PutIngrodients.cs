//using Oculus.Interaction;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class PutIngrodients : MonoBehaviour, ISwitchableGrabbableActive
//{

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.transform.GetChild(0).TryGetComponent<Machine>(out Machine machine))
//        {
//            Inactive();
//            this.transform.rotation = machine.transform.rotation;
//            this.transform.parent = machine.transform;
//            machine.ProcessedCertification(this.GetComponent<Ingrodients>()).Processing(this.GetComponent<Ingrodients>());
//        }
//    }

//    public void Active()
//    {
//        this.GetComponent<Grabbable>().enabled = true;
//    }
//    public void Inactive()
//    {
//        this.GetComponent<Rigidbody>().isKinematic = true;
//        this.GetComponent<Grabbable>().enabled = false;
//    }
//}

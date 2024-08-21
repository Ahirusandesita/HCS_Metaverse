using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HCSMeta.Activity.Cook
{
    public class Stove : MonoBehaviour
    {
        [SerializeField]
        private Transform fixedTransform;

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.TryGetComponent<AutoMachine>(out AutoMachine autoMachine))
            {
                if (autoMachine.IsGrab)
                {
                    autoMachine.CanProcessing = false;
                }
                else
                {
                    autoMachine.transform.root.transform.position = fixedTransform.position;
                    autoMachine.transform.root.transform.rotation = Quaternion.Euler(new Vector3(0f, autoMachine.transform.root.transform.rotation.eulerAngles.y, 0f));
                    autoMachine.transform.root.GetComponent<Rigidbody>().isKinematic = true;
                    autoMachine.CanProcessing = true;
                }
            }
        }
    }
}

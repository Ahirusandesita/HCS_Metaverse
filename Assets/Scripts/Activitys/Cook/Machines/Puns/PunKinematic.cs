using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
namespace HCSMeta.Activity.Cook
{
    public class PunKinematic : MonoBehaviour
    {
        private PointableUnityEventWrapper pointableUnityEventWrapper;
        private Rigidbody myRigidbody;
        private AutoMachine autoMachine;
        private void Awake()
        {
            pointableUnityEventWrapper = this.GetComponent<PointableUnityEventWrapper>();
            autoMachine = this.GetComponentInChildren<AutoMachine>();
            myRigidbody = this.GetComponent<Rigidbody>();
        }

        private void Start()
        {
            pointableUnityEventWrapper.WhenSelect.AddListener((data) => myRigidbody.isKinematic = true);
            pointableUnityEventWrapper.WhenUnselect.AddListener((data) => myRigidbody.isKinematic = false);

            pointableUnityEventWrapper.WhenSelect.AddListener((data) => autoMachine.IsGrab = true);
            pointableUnityEventWrapper.WhenUnselect.AddListener((data) => autoMachine.IsGrab = false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class PunKinematic : MonoBehaviour
{
    private PointableUnityEventWrapper pointableUnityEventWrapper;
    private Rigidbody rigidbody;
    private AutoMachine autoMachine;
    private void Awake()
    {
        pointableUnityEventWrapper = this.GetComponent<PointableUnityEventWrapper>();
        autoMachine = this.GetComponentInChildren<AutoMachine>();
        rigidbody = this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        pointableUnityEventWrapper.WhenSelect.AddListener((data) => rigidbody.isKinematic = true);
        pointableUnityEventWrapper.WhenUnselect.AddListener((data) => rigidbody.isKinematic = false);

        pointableUnityEventWrapper.WhenSelect.AddListener((data) => autoMachine.IsGrab = true);
        pointableUnityEventWrapper.WhenUnselect.AddListener((data) => autoMachine.IsGrab = false);
    }
}

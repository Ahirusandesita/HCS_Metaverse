using System;
using UnityEngine;
using VContainer;

public class LocalAvatarLogic : MonoBehaviour
{
    private RemoteView remoteView;

    private Action action;
    [Inject]
    public void Inject(RemoteView remoteView)
    {
        this.remoteView = remoteView;
        Vector3 position = this.transform.position;
        position.z -= 1f;
        remoteView.transform.position = position;

        remoteView.GetComponent<MeshRenderer>().enabled = false;

        action += () => remoteView.transform.position = transform.position;
    }

    private void Update()
    {

        //remoteView.transform.Translate(6f * Time.deltaTime * input.normalized);
        //Vector3 position = this.transform.position;
        //position.z -= 1f;
        //remoteView.transform.position = position;

        action?.Invoke();
        /* + (transform.forward * -0.5f)*/;
    }
}

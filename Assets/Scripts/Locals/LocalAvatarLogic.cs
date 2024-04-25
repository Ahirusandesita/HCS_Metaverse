using UnityEngine;
using VContainer;

public class LocalAvatarLogic : MonoBehaviour
{
    private RemoteView remoteView;

    [Inject]
    public void Inject(RemoteView remoteView)
    {
        this.remoteView = remoteView;
        Vector3 position = this.transform.position;
        position.z -= 1f;
        remoteView.transform.position = position;

        remoteView.GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {

        //remoteView.transform.Translate(6f * Time.deltaTime * input.normalized);
        //Vector3 position = this.transform.position;
        //position.z -= 1f;
        //remoteView.transform.position = position;

        remoteView.transform.position = transform.position/* + (transform.forward * -0.5f)*/;
    }
}

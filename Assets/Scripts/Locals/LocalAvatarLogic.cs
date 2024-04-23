using UnityEngine;
using VContainer;

public class LocalAvatarLogic : MonoBehaviour
{
    private RemoteView remoteView;

    [Inject]
    public void Inject(RemoteView remoteView)
    {
        this.remoteView = remoteView;
    }

    private void Update()
    {
        var input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
        transform.Translate(6f * Time.deltaTime * input.normalized);

        remoteView.transform.Translate(6f * Time.deltaTime * input.normalized);
    }
}

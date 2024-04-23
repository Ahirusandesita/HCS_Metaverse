using Photon.Pun;
using UnityEngine;

public class LocalRemoteSeparation : MonoBehaviour
{
    [SerializeField]
    private SeparationLifetimeScope separationLifetimeScope;

    [SerializeField]
    private GameObject localGameObject;

    [SerializeField]
    private string remoteViewName;

    // Update is called once per frame
    private void Start()
    {
        RemoteView remoteView = PhotonNetwork.Instantiate(remoteViewName, Vector3.zero, Quaternion.identity).GetComponent<RemoteView>();
        GameObject localObject = Instantiate(localGameObject, Vector3.zero, Quaternion.identity);

        Instantiate(separationLifetimeScope).SeparationSetup(localObject, remoteView).Build();
    }
}

using UnityEngine;

public class LocalRemoteSeparation : MonoBehaviour
{
    [SerializeField]
    private SeparationLifetimeScope separationLifetimeScope;

    [SerializeField]
    private GameObject localGameObject;

    [SerializeField]
    private RemoteView remoteViewObject;

    // Update is called once per frame
    public void RemoteViewCreate()
    {                                   //FusionÇÃê∂ê¨
        RemoteView remoteView = null;/*PhotonNetwork.Instantiate(remoteViewObject.name, Vector3.zero, Quaternion.identity).GetComponent<RemoteView>();*/

        //GameObject localObject = Instantiate(localGameObject, Vector3.zero, Quaternion.identity);

        Instantiate(separationLifetimeScope).SeparationSetup(localGameObject, remoteView).Build();

        Destroy(this);
    }
}

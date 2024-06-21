using UnityEngine;
using Fusion;

public class LocalRemoteSeparation : MonoBehaviour
{
    [SerializeField]
    private SeparationLifetimeScope separationLifetimeScope;

    [SerializeField]
    private GameObject localGameObject;

    [SerializeField]
    private NetworkPrefabRef remoteViewObject;


   
	// Update is called once per frame
	public void RemoteViewCreate(NetworkRunner networkRunner,PlayerRef playerRef)
    {                                   //FusionÇÃê∂ê¨
        RemoteView remoteView = networkRunner.Spawn(remoteViewObject,Vector3.zero,Quaternion.identity,playerRef).GetComponent<RemoteView>();/*PhotonNetwork.Instantiate(remoteViewObject.name, Vector3.zero, Quaternion.identity).GetComponent<RemoteView>();*/

        //GameObject localObject = Instantiate(localGameObject, Vector3.zero, Quaternion.identity);

        Instantiate(separationLifetimeScope).SeparationSetup(localGameObject, remoteView).Build();

        Destroy(this);
    }
}

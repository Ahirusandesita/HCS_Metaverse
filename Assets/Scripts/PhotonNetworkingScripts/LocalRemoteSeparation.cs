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
        RemoteView remoteView = networkRunner.Spawn(remoteViewObject,Vector3.zero,Quaternion.identity,playerRef).GetComponent<RemoteView>();

        Instantiate(separationLifetimeScope).SeparationSetup(localGameObject, remoteView).Build();

        Destroy(this);
    }
}

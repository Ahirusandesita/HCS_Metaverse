using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LocalView : MonoBehaviour, IGrabbableActiveChangeRequester
{
    private new NetworkView networkView;
    public NetworkView NetworkView => networkView;
    private ISwitchableGrabbableActive switchableGrabbable;
    [SerializeField]
    private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    void Start()
    {
        switchableGrabbable = this.GetComponentInChildren<ISwitchableGrabbableActive>();
    }
    public void Grab()
    {
        networkView.RPC_ExcludeOthersInactive();
    }
    public void Release()
    {
        networkView.RPC_ExcludeOthersActive();
    }

    public void Active(Vector3 position,Vector3 rotation)
    {
        switchableGrabbable.Active(this);
        foreach(MeshRenderer item in meshRenderers)
        {
            item.enabled = true;
        }
        this.transform.position = position;
        this.transform.rotation = Quaternion.Euler(rotation);
    }
    public void Inactive()
    {
        switchableGrabbable.Inactive(this);
        foreach (MeshRenderer item in meshRenderers)
        {
            item.enabled = false;
        }
    }

    public void NetworkViewInject(NetworkView networkView)
    {
        this.networkView = networkView;

        this.networkView.LocalViewInject(this);
    }
}

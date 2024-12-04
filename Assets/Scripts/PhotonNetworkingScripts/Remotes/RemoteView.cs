using UnityEngine;
using Fusion;

public class RemoteView : NetworkBehaviour,IDependencyInjector<PlayerBodyDependencyInformation>
{
	private Transform _playerTransform;
	private Transform _viewTransform;
	private PlayerBodyDependencyInformation information;
	public override void Spawned()
	{
		base.Spawned();
		_playerTransform = FindObjectOfType<VRPlayerController>().transform;
		_viewTransform = transform;

		PlayerInitialize.ConsignmentInject_static(this);
	}

	public void SetVector3(Vector3 vector)
	{
	}

	public override void FixedUpdateNetwork()
	{
		base.FixedUpdateNetwork();
		_viewTransform.position = _playerTransform.position;

        Quaternion rotation = _viewTransform.rotation;
        rotation.y = information.Head.Rotation.y;
        _viewTransform.rotation = rotation;

    }

    public void Inject(PlayerBodyDependencyInformation information)
    {
		this.information = information;
    }
}

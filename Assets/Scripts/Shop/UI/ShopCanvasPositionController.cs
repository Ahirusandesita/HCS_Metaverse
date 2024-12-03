using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KumaDebug;

public class ShopCanvasPositionController : MonoBehaviour,IDependencyInjector<PlayerBodyDependencyInformation>
{
	[SerializeField]
	private Vector3 _offset = Vector3.forward;
	private Transform _myTransform = default;
	private IReadonlyPositionAdapter _playerPosition = default;

	public void Inject(PlayerBodyDependencyInformation information)
	{
		_playerPosition = information.PlayerBody;
	}

	private void OnEnable()
	{
		PlayerInitialize.ConsignmentInject_static(this);
		_myTransform = transform;
	}
	private void Update()
	{
		_myTransform.position = _playerPosition.Position + _offset;
	}
}
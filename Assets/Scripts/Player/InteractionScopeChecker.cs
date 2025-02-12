using System;
using UniRx;
using UnityEngine;

public class InteractionScopeChecker : MonoBehaviour
{
	[SerializeField]
	private Transform centerEye = default;
	private bool isFiredTriggerStay = false;

	private readonly Subject<IInteraction> onInteractionEnter = new Subject<IInteraction>();
	private readonly Subject<IInteraction> onInteractionEnterLooking = new Subject<IInteraction>();
	private readonly Subject<Unit> onInteractionExit = new Subject<Unit>();

	public IObservable<IInteraction> OnInteractionEnter => onInteractionEnter;
	public IObservable<IInteraction> OnInteractionEnterLooking => onInteractionEnterLooking;
	public IObservable<Unit> OnInteractionExit => onInteractionExit;


	private void Awake()
	{
		onInteractionEnter.AddTo(this);
		onInteractionEnterLooking.AddTo(this);
		onInteractionExit.AddTo(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out IInteraction interaction))
		{
			onInteractionEnter.OnNext(interaction);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.TryGetComponent(out IInteraction interaction))
		{
			Vector3 tempDir = other.bounds.center - centerEye.position;
			Vector3 targetDir = new Vector3(tempDir.x, 0f, tempDir.z);
			float cosHalf = Mathf.Cos(90f / 2 * Mathf.Deg2Rad);
			float innerProduct = Vector3.Dot(centerEye.forward, targetDir.normalized);
			if (innerProduct > cosHalf)
			{
				if (isFiredTriggerStay)
				{
					return;
				}
				isFiredTriggerStay = true;
				onInteractionEnterLooking.OnNext(interaction);
			}
			else
			{
				if (!isFiredTriggerStay)
				{
					return;
				}
				isFiredTriggerStay = false;
				onInteractionExit.OnNext(Unit.Default);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out IInteraction _))
		{
			isFiredTriggerStay = false;
			onInteractionExit.OnNext(Unit.Default);
		}
	}
}
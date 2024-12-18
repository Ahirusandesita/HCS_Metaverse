using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerState : MonoBehaviour, IInputControllable, IInteractionInfoReceiver
{
	[SerializeField] private PlayerInteraction playerInteraction = default;
	[SerializeField] private BoolReactiveProperty placingMode = new BoolReactiveProperty();

	public IReadOnlyReactiveProperty<bool> PlacingMode => placingMode;


#if UNITY_EDITOR
	private void Reset()
	{
		playerInteraction ??= FindObjectOfType<PlayerInteraction>();
	}
#endif

	private void Awake()
	{
		playerInteraction.Add(this);
	}

	private void Start()
	{
		placingMode.Subscribe(_ =>
		{
			if (placingMode.Value)
			{
				// Debug
				Inputter.ChangeInputPreset(this, Inputter.InputActionPreset.Placing);
			}
			else
			{
				// Debug
				Inputter.ChangeInputPreset(this, Inputter.InputActionPreset.Default);
			}
		}).AddTo(this);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			placingMode.Value = !placingMode.Value;
		}
	}

	void IInteractionInfoReceiver.SetInfo(IInteraction.InteractionInfo interactionInfo)
	{
		if (interactionInfo is Shelf.ShelfInteractionInfo shelfInteractionInfo)
		{
			shelfInteractionInfo.OnSafetyOpenAction += () => placingMode.Value = true;
			shelfInteractionInfo.OnSafetyCloseAction += () => placingMode.Value = false;
		}
	}
}

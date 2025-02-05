using UniRx;
using UnityEngine;

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
		placingMode.AddTo(this);
		// IInteractionから送られてくる情報を受け取るために、受け取り者リストをAddする
		playerInteraction.Add(this);
	}

	private void Start()
	{
		placingMode.Subscribe(_ =>
		{
			if (placingMode.Value)
			{
				Inputter.ChangeInputPreset(this, Inputter.InputActionPreset.Placing);
			}
			else
			{
				Inputter.ChangeInputPreset(this, Inputter.InputActionPreset.Default);
			}
		});
	}

	void IInteractionInfoReceiver.SetInfo(IInteraction.InteractionInfo interactionInfo)
	{
		// IInteractionから受け取ったクラスがShelf（棚）のものだったら
		if (interactionInfo is Shelf.ShelfInteractionInfo shelfInteractionInfo)
		{
			// Actionを受け取る。各アクションはSafetyOpenとSafetyCloseのタイミングで発火する
			// つまり棚のSafetyOpenが発火したときPlacingModeをONにし、SafetyCloseが発火したときPlacingModeをOFFにする
			shelfInteractionInfo.OnSafetyOpenAction += data => placingMode.Value = true;
			shelfInteractionInfo.OnSafetyCloseAction += data => placingMode.Value = false;
		}
	}

	public void ChangePlacingMode()
	{
		placingMode.Value = !placingMode.Value;
	}
}

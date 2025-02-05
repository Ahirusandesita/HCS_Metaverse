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
		// IInteraction���瑗���Ă�������󂯎�邽�߂ɁA�󂯎��҃��X�g��Add����
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
		// IInteraction����󂯎�����N���X��Shelf�i�I�j�̂��̂�������
		if (interactionInfo is Shelf.ShelfInteractionInfo shelfInteractionInfo)
		{
			// Action���󂯎��B�e�A�N�V������SafetyOpen��SafetyClose�̃^�C�~���O�Ŕ��΂���
			// �܂�I��SafetyOpen�����΂����Ƃ�PlacingMode��ON�ɂ��ASafetyClose�����΂����Ƃ�PlacingMode��OFF�ɂ���
			shelfInteractionInfo.OnSafetyOpenAction += data => placingMode.Value = true;
			shelfInteractionInfo.OnSafetyCloseAction += data => placingMode.Value = false;
		}
	}

	public void ChangePlacingMode()
	{
		placingMode.Value = !placingMode.Value;
	}
}

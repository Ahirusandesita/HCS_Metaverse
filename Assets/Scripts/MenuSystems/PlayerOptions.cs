using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerOptions : MonoBehaviour, IPointerClickHandler
{
	private enum OptionType
	{
		EnableWarpMovement,
		EnableAnalogRotation,
		EnableTunnelingVignette,
	}

	[SerializeField]
	private OptionType optionType = default;
	[SerializeField]
	private Image checkBox = default;
	[SerializeField]
	private Sprite checkSprite = default;
	[SerializeField]
	private Sprite boxSprite = default;
	[SerializeField]
	private bool defaultValue = default;
	private bool currentValue = default;

	private void Start()
	{
		checkBox.sprite = defaultValue ? checkSprite : boxSprite;
		currentValue = defaultValue;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		var playerController = FindAnyObjectByType<VRPlayerController>();
		var vignetteManager = FindAnyObjectByType<HCSMeta.Player.View.TunnelingVignetteManager>();
		if (playerController is null || vignetteManager is null)
		{
			return;
		}

		currentValue = !currentValue;
		checkBox.sprite = currentValue ? checkSprite : boxSprite;

		switch (optionType)
		{
			case OptionType.EnableWarpMovement:
				var moveType = currentValue ? VRMoveType.Warp : VRMoveType.Natural;
				playerController.ChangeMoveType(moveType);
				break;

			case OptionType.EnableAnalogRotation:
				var rotateType = currentValue ? VRRotateType.Analog : VRRotateType.Degital;
				playerController.ChangeRotateType(rotateType);
				break;

			case OptionType.EnableTunnelingVignette:
				vignetteManager.SetEnableVignette(currentValue);
				break;

			default:
				break;
		}
	}
}

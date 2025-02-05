using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HousingCanvas : MonoBehaviour, IPointerClickRegistrable, IInteraction
{
	[SerializeField] private GameObject displayedContent = default;

	ISelectedNotification IInteraction.SelectedNotification => throw new System.NotImplementedException();

	void IPointerClickRegistrable.OnPointerClick(PointerEventData data)
	{
		var player = FindAnyObjectByType<PlayerState>();
		player.ChangePlacingMode();
	}

	IInteraction.InteractionInfo IInteraction.Open()
	{
		displayedContent.SetActive(true);
		return new IInteraction.NullInteractionInfo();
	}

	void IInteraction.Close()
	{
		displayedContent.SetActive(false);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HousingCanvas : MonoBehaviour, IInteraction
{
	[SerializeField] private GameObject displayedContent = default;

	public bool IsFiredTriggerStay { get; set; }
	ISelectedNotification IInteraction.SelectedNotification => throw new System.NotImplementedException();

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActivityStartButton : MonoBehaviour
{
	[ContextMenu("Click")]
	public void Click()
	{
		GateOfFusion.Instance.ActivityStart();
	}
}

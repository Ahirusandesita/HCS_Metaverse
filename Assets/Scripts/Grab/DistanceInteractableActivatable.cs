using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction;
using Fusion;

public class DistanceInteractableActivatable : MonoBehaviour, IActivatableDistance, IGrabbableActiveChangeRequester
{
    [SerializeField]
    private float activeDistance;
    public float ActiveDistance => activeDistance;
    ISwitchableGrabbableActive grabbableActive;

    private void Start()
    {
        grabbableActive = this.GetComponent<ISwitchableGrabbableActive>();
        if (grabbableActive == null)
        {
            Debug.LogError($"ISwitchableGrabbableActiveがアタッチされていません" + this.gameObject.name);
            return;
        }
        grabbableActive.Regist(this);
        GameObject.FindObjectOfType<DistanceInteractableChecker>().Add(this);
    }

    void IActivatableDistance.Active()
    {
        grabbableActive.Active(this);
    }

    void IActivatableDistance.Passive()
    {
        grabbableActive.Inactive(this);
    }

    public void SetActiveDistance(float activeDistance)
	{
        this.activeDistance = activeDistance;
	}

    private void OnDestroy()
    {
        if (GameObject.FindObjectOfType<DistanceInteractableChecker>())
        {
            GameObject.FindObjectOfType<DistanceInteractableChecker>().Remove(this);
        }
    }
}
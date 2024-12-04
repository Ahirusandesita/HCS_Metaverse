using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DistanceInteractableChecker : MonoBehaviour
{
    private List<IActivatableDistance> activeDistances;
    private void Awake()
    {
        activeDistances = InterfaceUtils.FindObjectOfInterfaces<IActivatableDistance>().ToList();
    }
    public void Add(IActivatableDistance activatableDistance)
    {
        foreach(IActivatableDistance item in activeDistances)
        {
            if(item == activatableDistance)
            {
                return;
            }
        }
        activeDistances.Add(activatableDistance);
    }
    private void Update()
    {
        CheckDistance();
    }

    private void CheckDistance()
    {
        foreach (IActivatableDistance activeDistance in activeDistances)
        {
            if (Vector3.Distance(this.transform.position, activeDistance.gameObject.transform.position) < activeDistance.ActiveDistance)
            {
                activeDistance.Active();
            }
            else
            {
                activeDistance.Passive();
            }
        }
    }
}

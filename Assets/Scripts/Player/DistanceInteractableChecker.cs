using UnityEngine;

public class DistanceInteractableChecker : MonoBehaviour
{
    private IActivatableDistance[] activeDistances;
    private void Awake()
    {
        activeDistances = InterfaceUtils.FindObjectOfInterfaces<IActivatableDistance>();
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

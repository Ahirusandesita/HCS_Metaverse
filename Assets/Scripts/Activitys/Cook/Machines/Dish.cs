using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IPutableOnDish
{
    void PutCommodity(ISwitchableGrabbableActive switchable);
}
public class NullPutableOnDish : IPutableOnDish
{
    public void PutCommodity(ISwitchableGrabbableActive switchable)
    {

    }
}

public class Dish : MonoBehaviour, IPutableOnDish
{
    private Transform fixedTransform;

    public void PutCommodity(ISwitchableGrabbableActive switchable)
    {
        switchable.Inactive();
        switchable.gameObject.transform.parent = this.transform;
        switchable.gameObject.transform.position = fixedTransform.position;
    }
}

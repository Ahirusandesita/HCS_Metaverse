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
    [SerializeField]
    private Transform fixedTransform;
    private ISwitchableGrabbableActive switchable;
    bool canPut = true;
    public void PutCommodity(ISwitchableGrabbableActive switchable)
    {
        if (!canPut)
        {
            return;
        }
        if(this.switchable != null)
        {
            return;
        }


        this.switchable = switchable;
        switchable.Inactive();
        switchable.gameObject.transform.parent = this.transform;
        switchable.gameObject.transform.rotation = this.transform.rotation;
        switchable.gameObject.transform.position = fixedTransform.position;

        switchable.gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void Update()
    {
        if(switchable == null)
        {
            return;
        }

        Vector3 vector3 = transform.rotation.eulerAngles;
        if (vector3.x > 70f && vector3.x < 290f)
        {
            switchable.Active();
            switchable.gameObject.GetComponent<Commodity>().InjectPutableOnDish(new NullPutableOnDish());
            switchable.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            switchable.gameObject.transform.parent = null;
            switchable = null;
            canPut = false;
        }

        else if(vector3.z > 70f && vector3.z < 290f)
        {
            switchable.Active();
            switchable.gameObject.GetComponent<Commodity>().InjectPutableOnDish(new NullPutableOnDish());
            switchable.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            switchable.gameObject.transform.parent = null;
            switchable = null;
            canPut = false;
        }
        else
        {
            canPut = true;
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<SubmisionTable>(out SubmisionTable table))
        {
            if (switchable is null)
            {
                return;
            }
            table.Submit(switchable.gameObject.GetComponent<Commodity>());
            switchable = null;
        }
    }
}

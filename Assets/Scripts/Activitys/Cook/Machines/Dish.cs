using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NullPutableOnDish : IPutableOnDish
{
    [Rpc]
    public void Rpc_PutCommodity(NetworkObject putObject)
    {

    }
    public void CommodityReset() { }
}
public class Dish : NetworkBehaviour, IPutableOnDish
{
    [SerializeField]
    private Transform fixedTransform;
    private ISwitchableGrabbableActive switchable;
    bool canPut = true;

    [Rpc]
    public void Rpc_PutCommodity(NetworkObject putObject)
    {
        Debug.Log($"<color=red>Put:{putObject.name}</color>");

        ISwitchableGrabbableActive switchable = putObject.GetComponent<ISwitchableGrabbableActive>();

        if (this.switchable != null)
        {
            return;
        }
        if (!canPut)
        {
            return;
        }
        
        this.switchable = switchable;
        switchable.Inactive();

        if (putObject.StateAuthority == Runner.LocalPlayer)
        {
            putObject.transform.parent = this.transform;
            putObject.transform.rotation = this.transform.rotation;
            putObject.transform.position = fixedTransform.position;
        }

        putObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void Update()
    {

        if (switchable == null)
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
            StartCoroutine(NotPutOn());
        }
        else if (vector3.z > 70f && vector3.z < 290f)
        {
            switchable.Active();
            switchable.gameObject.GetComponent<Commodity>().InjectPutableOnDish(new NullPutableOnDish());
            switchable.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            switchable.gameObject.transform.parent = null;
            switchable = null;
            StartCoroutine(NotPutOn());
        }
    }

    private IEnumerator NotPutOn()
    {
        canPut = false;
        yield return new WaitForSeconds(1f);
        canPut = true;
    }

    public void CommodityReset()
    {
        switchable = null;
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

    public void Inject(ISwitchableGrabbableActive t)
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Oculus.Interaction;
using Cysharp.Threading.Tasks;

public class NullPutableOnDish : IPutableOnDish
{
    [Rpc]
    public void Rpc_PutCommodity(NetworkObject putObject)
    {

    }
    public void CommodityReset() { }
}
public class Dish : NetworkBehaviour, IPutableOnDish,IGrabbableActiveChangeRequester
{
    [SerializeField]
    private Transform fixedTransform;
    private Vector3 fixedPosition;
    private ISwitchableGrabbableActive switchable;
    bool canPut = true;
    NetworkObject _myNetwork;

    [Rpc]
    public void Rpc_PutCommodity(NetworkObject putObject)
    {
        Debug.Log($"<color=red>Put:{putObject.name}</color>");

        ISwitchableGrabbableActive switchable = putObject.GetComponent<ISwitchableGrabbableActive>();

        switchable.Regist(this);

        if (this.switchable != null)
        {
            return;
        }
        if (!canPut)
        {
            return;
        }
        
        this.switchable = switchable;
        switchable.Inactive(this);

        putObject.transform.parent = this.transform;

        if (putObject.StateAuthority == Runner.LocalPlayer)
        {
            putObject.transform.rotation = this.transform.rotation;
            putObject.transform.localPosition = fixedPosition;
        }

        putObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void Start()
    {
        PointableUnityEventWrapper pointableUnityEventWrapper;

        pointableUnityEventWrapper = GetComponent<PointableUnityEventWrapper>();

        fixedPosition = fixedTransform.localPosition;

        _myNetwork = GetComponent<NetworkObject>();

    }

    private void Update()
    {
        if (switchable == null || !_myNetwork.HasStateAuthority)
        {
            return;
        }

        Vector3 vector3 = transform.rotation.eulerAngles;

        if (vector3.x > 70f && vector3.x < 290f)
        {
            switchable.Active(this);
            switchable.gameObject.GetComponent<Commodity>().InjectPutableOnDish(new NullPutableOnDish());
            switchable.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            switchable.gameObject.transform.parent = null;
            switchable.Cancellation(this);
            switchable = null;
            canPut = false;
            StartCoroutine(NotPutOn());
        }
        else if (vector3.z > 70f && vector3.z < 290f)
        {
            switchable.Active(this);
            switchable.gameObject.GetComponent<Commodity>().InjectPutableOnDish(new NullPutableOnDish());
            switchable.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            switchable.gameObject.transform.parent = null;
            switchable.Cancellation(this);
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
        switchable.Cancellation(this);
        switchable = null;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (_myNetwork.HasStateAuthority)
        {
            if (collision.gameObject.TryGetComponent<SubmisionTable>(out SubmisionTable table))
            {
                if (switchable is null)
                {
                    return;
                }
                table.Submit(switchable.gameObject.GetComponent<Commodity>());
                switchable = null;

                GameObject.FindObjectOfType<DishManager>().InstanceNewDish(GetComponent<NetworkObject>());
            }
        }
    }

    public void Inject(ISwitchableGrabbableActive t)
    {
        
    }
}
